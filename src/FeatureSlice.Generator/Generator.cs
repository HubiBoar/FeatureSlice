using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeatureSlice.Generator;

[Generator]
internal sealed class Generator : IIncrementalGenerator
{
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

            if (symbol is not null && symbol.AllInterfaces.Any(x => x.ToDisplayString() == "FeatureSlice.Handle2.IFeatureSliceBase"))
            {
                var result = GetType(symbol, type, compilation);
                
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
                if (x.ContainingNamespace.ToDisplayString() != "FeatureSlice.Handle2" || x.Name != "FeatureSliceBuilder")
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
            .Distinct();

        var interfacesString = string.Join(",\n    ", interfaces.Select(x => x.ToDisplayString()));

        var members = string.Join(",\n    ", interfaces.SelectMany(x => {

            return x.GetMembers().Where(m => m.IsAbstract).Select(m => m.ToDisplayString());
        }));

        var result = $$"""

        namespace {{symbol.ContainingNamespace.ToDisplayString()}};

        public partial class {{symbol.Name}} :
            {{interfacesString}}
        {
            {{members}}
        }
        """;

        return (result, $"{fullName}.g.cs");
    }
}

