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

        var nodes = string.Join("\n", type.DescendantNodes()
                .OfType<PrimaryConstructorBaseTypeSyntax>()
                .SelectMany(x => x
                    .ChildNodes()
                    .OfType<ArgumentListSyntax>()
                    .SelectMany(y => y
                        .ChildNodes()
                        .SelectMany(z => z
                            .ChildNodes()
                            .SelectMany(z => z
                                .ChildNodes()
                                .Select(z =>
                                {
                                    return z.GetType().ToString();
                                }))))));

        var creation = type.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(x => 
        {
            if (x is null)
            {
                return "//null";
            }

            if (x.SyntaxTree is null)
            {
                return "//null1";
            }

            var methodSymbol = compilation
                .GetSemanticModel(x.SyntaxTree)
                .GetSymbolInfo(x).Symbol as IMethodSymbol;

            if (methodSymbol is null)
            {
                return "//null2";
            }
            else if (methodSymbol.ReturnType is null)
            {
                return $"//null3 {methodSymbol.ToDisplayString()}";
            }

            return "//" + methodSymbol.ReturnType.ToDisplayString();

        }).ToArray();

        var creationString = string.Join("\n", creation);

        var result = $$"""

        public partial class {{symbol.Name}}
        {
        {{nodes}}
        }
        """;

        return (result, $"{fullName}.g.cs");
    }
}

