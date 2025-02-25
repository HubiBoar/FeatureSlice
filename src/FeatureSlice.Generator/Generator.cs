using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeatureSlice.Generator;

[Generator]
internal sealed class Generator : IIncrementalGenerator
{
    private const string Namespace = "FeatureSlice.Handle2";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider
        (
            predicate: (c, _) =>
                c is TypeDeclarationSyntax type
                && type.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),

            transform: (n, _) => (n.Node as TypeDeclarationSyntax)!
        );

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, (spc, source) => Execute(spc, source.Left, source.Right)); 
    }

    private static void Execute
    (
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax> typeList
    )
    {
        Dictionary<string, string> classes = new ();

        foreach (var type in typeList)
        {
            var symbol = compilation
                .GetSemanticModel(type.SyntaxTree)
                .GetDeclaredSymbol(type) as INamedTypeSymbol;

            if (symbol is not null
                && symbol.IsAbstract is false
                && symbol.ToDisplayString() != $"{Namespace}.FeatureSlice<TRequest, TResponse>"
                && symbol.AllInterfaces.Any(x => x.ToDisplayString() == $"{Namespace}.IFeatureSliceBase"))
            {
                var result = GetType(context, symbol, type, compilation);
                
                if (classes.ContainsKey(result.ClassName) is false)
                {
                    classes.Add(result.ClassName, result.Code);
                }
            }
        }

        foreach (var result in classes)
        {
            context.AddSource(result.Key, result.Value);
        }
    }

    private static (string Code, string ClassName) GetType
    (
        SourceProductionContext context,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax type,
        Compilation compilation
    )
    {
        var fullName = symbol.ToDisplayString();

        var methods = symbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Select(x => $"\t//{x.Name} :: {x.ReturnType.ToDisplayString()}")
            .ToArray();

        var methodsString = string.Join("\n", methods);

        var invocations = type
            .DescendantNodes()
            .OfType<PrimaryConstructorBaseTypeSyntax>()
            .SelectMany(x => x
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(y => y
                    .Ancestors()
                    .OfType<ArgumentListSyntax>()
                    .Count() == 1));

        var types = invocations.Select(x => 
        {
            var methodSymbol = compilation
                .GetSemanticModel(x.SyntaxTree)
                .GetSymbolInfo(x).Symbol as IMethodSymbol;

            if (methodSymbol is null)
            {
                return null;
            }

            return methodSymbol.ReturnType;

        })
        .Where(x => x is not null)
        .Select(x => x!)
        .ToArray();

        var interfaces = types
            .Select(x =>
            {
                if (x.ContainingNamespace.ToDisplayString() != Namespace || x.Name != "Config")
                {
                    return null;
                }

                if (x is INamedTypeSymbol named is false)
                {
                    return null;
                }

                var arg = named.TypeArguments.First();

                return arg;
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToArray();

        var interfacesString = string.Join(",\n    ", interfaces.Select(x => x.ToDisplayString()));
        var abstractMembers = interfaces
            .SelectMany(x => x
                .AllInterfaces
                .SelectMany(y => y
                    .GetMembers()
                    .Where(m => m.IsAbstract)))
            .Concat(
                interfaces
                .SelectMany(x => x
                    .GetMembers()
                    .Where(m => m.IsAbstract)))
            .Distinct()
            .ToArray();

        var methodsMembers = abstractMembers
            .OfType<IMethodSymbol>()
            .Where(x => 
                x.MethodKind == MethodKind.Ordinary 
                && x.ToDisplayString() != $"{Namespace}.IFeatureSliceSetup.Configure(System.IServiceProvider)"
                && x.DeclaredAccessibility == Accessibility.Public)
            .ToArray();  

        var propertyMembers = abstractMembers.OfType<IPropertySymbol>().Where(x => x.DeclaredAccessibility == Accessibility.Public).ToArray();  

        var members = string.Join(",\n    ", abstractMembers.Select(x =>  $"// {x.GetType().Name} :: {x.ToDisplayString()}"));

        var methodMembersString = string.Join("\n", methodsMembers
            .Select(x => {
                var returnType = x.ReturnsVoid ? "void" : x.ReturnType.ToDisplayString();
                var containingType = x.ContainingType.ToDisplayString();
                var name = x.Name;
                var parametersDeclared = string.Join(", ", x.Parameters.Select(z => z.ToDisplayString()));
                var parameters = string.Join(", ", x.Parameters.Select(z => z.Name));

                return $"""
                    public {returnType} {name}({parametersDeclared})
                        => this.TryGetSetup<{containingType}>()!.{name}({parameters});

                """;
            }));

        var propertyMembersString = string.Join("\n", propertyMembers
            .Select(x => {
                var returnType = x.Type.ToDisplayString(); 
                var containingType = x.ContainingType.ToDisplayString();
                var name = x.Name;

                var get = x.GetMethod is null || x.DeclaredAccessibility != Accessibility.Public
                ? string.Empty 
                : $"get => this.TryGetSetup<{containingType}>()!.{name};";

                var set = x.SetMethod is null || x.DeclaredAccessibility != Accessibility.Public
                ? string.Empty 
                : $"set => this.TryGetSetup<{containingType}>()!.{name} = value;";

                return $$"""
                    public {{returnType}} {{name}}
                    {
                        {{get}}
                        {{set}}
                    }

                """;
            }));

        var symbolNamespace = symbol.ContainingNamespace.ToDisplayString();
        var symbolFullName = symbol.ToDisplayString();
        var symbolName = symbol.Name;

        var result = $$"""
        using System.Diagnostics.CodeAnalysis;
        using Microsoft.Extensions.DependencyInjection;

        namespace {{symbolNamespace}};

        public sealed partial record {{symbolName}} :
            {{interfacesString}}
        {
            [SetsRequiredMembers]
            public {{symbolName}}(IServiceProvider provider) : this()
            {
                Configuration.Builder.Configure(provider);
                Dispatch = request => Configuration.Builder.Dispatch(provider, request);
            }

            void {{Namespace}}.IFeatureSliceSetup.Configure(IServiceProvider provider)
            {
                Configuration.Builder.Configure(provider);
            }

            public static void Register(IServiceCollection services)
            {
                services.AddSingleton<{{symbolFullName}}>(provider => new {{symbolFullName}}(provider));
            }

            //Members
            
            {{members}}

            //Methods
            
        {{methodMembersString}}

            //Properties
            
        {{propertyMembersString}}
        }
        """;

        return (result, $"{fullName}.g.cs");
    }
}

