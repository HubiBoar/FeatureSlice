using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace FeatureSliceGenerator;

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

        var allTypes = GetMethods(syntaxes, compilation);
        
        AddMethodNames(allTypes, context);
        AddExtensions(allTypes, context);
    }

    private static IReadOnlyCollection<(INamedTypeSymbol symbol, IReadOnlyCollection<IMethodSymbol> methods)> GetMethods(
        ImmutableArray<ClassDeclarationSyntax> syntaxes,
        Compilation compilation)
    {
        return syntaxes.SelectMany(syntax => 
        {
            var symbol = compilation
                .GetSemanticModel(syntax.SyntaxTree)
                .GetDeclaredSymbol(syntax) as ITypeSymbol;

            return symbol.AllInterfaces
                .Where(x => x
                    .GetAttributes()
                    .Any(a => a.AttributeClass.ToDisplayString() == ExtensionAttribute))
                .Select(symbol => (symbol, (IReadOnlyCollection<IMethodSymbol>)symbol
                    .GetMembers()
                    .Where(m => m is IMethodSymbol)
                    .Where(m => m
                        .GetAttributes()
                        .Any(a => a.AttributeClass.ToDisplayString() == ExtensionMethodAttribute))
                    .Select(x => x as IMethodSymbol)
                    .ToArray()));
        }).ToArray();
    }

    private static string GetTypeParameterName(ITypeParameterSymbol symbol)
    {
        var constrains = new List<string>();
        if(symbol.HasReferenceTypeConstraint)
        {
            constrains.Add("class");
        }

        if(symbol.HasUnmanagedTypeConstraint)
        {
            constrains.Add("unmanaged");
        }

        if(symbol.HasValueTypeConstraint)
        {
            constrains.Add("struct");
        }

        if(symbol.HasNotNullConstraint)
        {
            constrains.Add("notnull");
        }

        constrains.AddRange(symbol.ConstraintTypes.Select(y => y.ToDisplayString()));

        return $"\t\twhere {symbol} : {string.Join(", ", constrains)}";
    }

    private static string ExtensionMethodBody(IMethodSymbol method, int index)
    {
        var attributeProperty = method.GetAttributes()
            .First(x => x.AttributeClass.ToDisplayString() == ExtensionMethodAttribute)
            .ConstructorArguments[0].Value;

        var methodNamespace = attributeProperty is null ? method.ContainingNamespace.ToDisplayString() : attributeProperty;

        var genericParameters = $"<{string.Join(", ", method
            .TypeParameters
            .Select(x => x.ToDisplayString())
            .ToArray())}>";

        var genericConstraints = $"{string.Join("\n\t", method
            .TypeParameters
            .Select(GetTypeParameterName)
            .ToArray())}";

        var arguments = string.Join(", ", method
            .Parameters
            .Select(x => x.ToDisplayString())
            .ToArray());

        var returnType = method.ReturnType.ToDisplayString();
        var methodName = method.Name;

        var returnBody = $"{method.ToDisplayString().Split('(')[0]}({string.Join(",", method.Parameters.Select(x => x.Name))})";
        var returns = method.ReturnsVoid ? returnBody : $"return {returnBody}";

        var methodBody = new StringBuilder()
            .AppendLine($"namespace {methodNamespace}")
            .AppendLine("{")
            .AppendLine($"\tpublic static class Extensions{index}")
            .AppendLine("\t{")
            .Append("\t\tpublic static ")
            .Append(returnType)
            .Append($" {methodName}")
            .Append(genericParameters)
            .Append("(this ")
            .Append(arguments)
            .Append(")")
            .AppendLine()
            .AppendLine($"\t{genericConstraints}")
            .AppendLine("\t\t{")
            .AppendLine($"\t\t\t{returns};")
            .AppendLine("\t\t}")
            .AppendLine("\t}")
            .AppendLine("}");

        return methodBody.ToString();
    }

    private static void AddExtensions(
        IReadOnlyCollection<(INamedTypeSymbol symbol, IReadOnlyCollection<IMethodSymbol> methods)> allTypes, 
        SourceProductionContext context)
    {
        int i = 0;

        var extensions = allTypes
            .Select(type => (
                FileName: GetFileName(type.symbol),
                Methods: type.methods.Select(method => ExtensionMethodBody(method, i++))))
            .GroupBy(x => x.FileName)
            .Select(x => (FileName: x.Key, Methods: x.ToArray().SelectMany(y => y.Methods).ToArray()))
            .ToArray();

        foreach(var extension in extensions)
        {
            context.AddSource(
                hintName: extension.FileName,
                source: string.Join("\n", extension.Methods));
        }
    }

    private static string GetFileName(INamedTypeSymbol symbol)
    {
        var attributeProperty = symbol.GetAttributes()
            .First(x => x.AttributeClass.ToDisplayString() == ExtensionAttribute)
            .ConstructorArguments[0].Value;

        return $"{symbol.ContainingNamespace}.extensions.{attributeProperty}.g.cs";
    }

    private static void AddMethodNames(
        IReadOnlyCollection<(INamedTypeSymbol symbol, IReadOnlyCollection<IMethodSymbol> methods)> allTypes, 
        SourceProductionContext context)
    {
        var lines = new List<string>();

        foreach(var (symbol, methods) in allTypes)
        {
            lines.Add($"type --> {symbol.ToDisplayString()}");

            foreach(var method in methods)
            {
                lines.Add($"\t method --> {method.ToDisplayString()}");
                lines.Add($"\t returnType --> {method.ReturnType.ToDisplayString()}");
                lines.Add($"\t name --> {method.Name}");

                var attribute = method.GetAttributes()
                    .First(x => x.AttributeClass.ToDisplayString() == ExtensionMethodAttribute);

                lines.Add($"\t attributeArguments --> {attribute.ConstructorArguments.Count()}");

                foreach(var attributeArgument in attribute.ConstructorArguments)
                {
                    lines.Add($"\t\t attribute --> {attributeArgument.Value}");
                }

                foreach(var argument in method.TypeArguments)
                {
                    lines.Add($"\t\t typeArguments --> {argument.ToDisplayString()}");
                }

                foreach(var parameter in method.Parameters)
                {
                    lines.Add($"\t\t parameter --> {parameter.ToDisplayString()}");
                }

                foreach(var typeParameter in method.TypeParameters)
                {
                    lines.Add($"\t\t typeParameter --> {typeParameter.ToDisplayString()}");

                    foreach(var constraintParameter in typeParameter.ConstraintTypes)
                    {
                        lines.Add($"\t\t\t constraintParameter --> {constraintParameter.ToDisplayString()}");
                    }
                }
            }
        }

        context.AddSource(
            hintName: "MethodNames.g.cs",
            source: $$"""
            namespace ClassesList;

            public static class ClassesNamesList
            {
                public static List<string> Names = new ()
                {
                    {{string.Join(",\n        ", lines.Select(x => $"\"{x}\""))}}
                };
            }
            """);
    }
}


