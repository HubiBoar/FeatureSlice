using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace OneOfGen;

//To show changes:
//   <PropertyGroup>
//     <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
//     <CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
//   </PropertyGroup>

//   <Target Name="CleanSourceGeneratedFiles" BeforeTargets="BeforeBuild" DependsOnTargets="$(BeforeBuildDependsOn)">
//     <RemoveDir Directories="Generated" />
//   </Target>

//   <ItemGroup>
//     <Compile Remove="Generated\**" />
//     <Content Include="Generated\**" />
//   </ItemGroup>


//To update changes:
//dotnet build-server shutdow
//dotnet build

public interface IOneOfElement
{
}

[AttributeUsage(AttributeTargets.Interface)]
public sealed class GenerateExtensionAttribute : Attribute
{
    public string FileName { get; }

    public GenerateExtensionAttribute(string fileName)
    {
        FileName = fileName;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateExtensionMethodAttribute : Attribute
{
    public string Namespace { get; }

    public GenerateExtensionMethodAttribute(string methodNamespace = null)
    {
        Namespace = methodNamespace;
    }
}

[Generator]
public class FeatureSliceGen : IIncrementalGenerator
{
    private static string ExtensionAttribute = typeof(GenerateExtensionAttribute).FullName;
    private static string ExtensionMethodAttribute = typeof(GenerateExtensionMethodAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(x => x is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<ClassDeclarationSyntax> syntaxes) tuple)
    {
        var (compilation, syntaxes) = tuple;

        var allTypes = GetOneOfElements(syntaxes, compilation);
        
        AddMethodNames(allTypes, context);
    }

    private static IReadOnlyCollection<ITypeSymbol> GetOneOfElements(
        ImmutableArray<ClassDeclarationSyntax> syntaxes,
        Compilation compilation)
    {
        return syntaxes.Select(syntax => compilation
                .GetSemanticModel(syntax.SyntaxTree)
                .GetDeclaredSymbol(syntax) as ITypeSymbol)
            .Where(x => x.AllInterfaces
                .Any(x => x.ToDisplayString() == typeof(IOneOfElement).FullName))
            .ToArray();
    }

    private static void AddMethodNames(
        IReadOnlyCollection<ITypeSymbol> oneOfElements, 
        SourceProductionContext context)
    {
        var elementNames = oneOfElements.Select(x => x.ToDisplayString()).ToArray();

        var lines = new List<string>();

        foreach(var element in oneOfElements)
        {
            var name = element.ToDisplayString();
        }

        context.AddSource(
            hintName: "OneOfIs.g.cs",
            source: $$"""
            namespace OneOfIs;

            public sealed record Is<T>(T Value)
                {{string.Join(",\n\t", lines)}}
                where T : IOneOfElement
            {
                public static explicit operator Is<T>(T value)
                {
                    return new Is<T>(value);
                }
            }
            """);
    }
}

// public sealed record Is<T>(T Value) :
//     IOneOf<T, T2Example, T3Example>,
//     IOneOf<T1Example, T, T3Example>,
//     IOneOf<T1Example, T2Example, T>
//     where T : IOneOfElement
// {
//     public static explicit operator Is<T>(T value)
//     {
//         return new Is<T>(value);
//     }
// }


