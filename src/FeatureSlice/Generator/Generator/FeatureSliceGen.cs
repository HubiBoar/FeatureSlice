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



[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class GenerateExtensionAttribute : Attribute
{
    public GenerateExtensionAttribute(string fileName)
    {
        
    }
}


[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateExtensionMethodAttribute : Attribute
{
    public string Namespace { get; }

    public GenerateExtensionMethodAttribute(string Namespace = null)
    {
        this.Namespace = Namespace;
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
            .SelectMany(type => type.methods
                .Select(method => ExtensionMethodBody(method, i++)));

        //set namespace to be the same as class implementing it
        context.AddSource(
            hintName: "extensionNameSpace.extensions.g.cs",
            source: string.Join("\n", extensions));
    }

    private static void AddMethodNames(
        IReadOnlyCollection<(INamedTypeSymbol symbol, IReadOnlyCollection<IMethodSymbol> methods)> allTypes, 
        SourceProductionContext context)
    {
        var methods = new List<string>();

        foreach(var type in allTypes)
        {
            methods.Add($"type --> {type.symbol.ToDisplayString()}");

            foreach(var method in type.methods)
            {
                methods.Add($"\t method --> {method.ToDisplayString()}");
                methods.Add($"\t returnType --> {method.ReturnType.ToDisplayString()}");
                methods.Add($"\t name --> {method.Name}");

                var attribute = method.GetAttributes()
                    .First(x => x.AttributeClass.ToDisplayString() == ExtensionMethodAttribute);

                methods.Add($"\t attributeArguments --> {attribute.ConstructorArguments.Count()}");

                foreach(var attributeArgument in attribute.ConstructorArguments)
                {
                    methods.Add($"\t\t attribute --> {attributeArgument.Value}");
                }

                foreach(var argument in method.TypeArguments)
                {
                    methods.Add($"\t\t typeArguments --> {argument.ToDisplayString()}");
                }

                foreach(var parameter in method.Parameters)
                {
                    methods.Add($"\t\t parameter --> {parameter.ToDisplayString()}");
                }

                foreach(var typeParameter in method.TypeParameters)
                {
                    methods.Add($"\t\t typeParameter --> {typeParameter.ToDisplayString()}");

                    foreach(var constraintParameter in typeParameter.ConstraintTypes)
                    {
                        methods.Add($"\t\t\t constraintParameter --> {constraintParameter.ToDisplayString()}");
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
                    {{string.Join(",\n        ", methods.Select(x => $"\"{x}\""))}}
                };
            }
            """);
    }
}


