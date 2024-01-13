using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

[Generator]
public class FeatureSliceGen : IIncrementalGenerator
{
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
        var (comilation, syntaxes) = tuple;

        var methods = new List<string>();

        foreach(var syntax in syntaxes)
        {
            var symbol = comilation
                .GetSemanticModel(syntax.SyntaxTree)
                .GetDeclaredSymbol(syntax) as ITypeSymbol;

            var implemented = symbol.AllInterfaces.Where(x => x.ToDisplayString().StartsWith("FeatureSlice.New.Generation.IMethod<")).Select(x => x.ToDisplayString()).ToArray();

            foreach(var i in implemented)
            {
                methods.Add(i.Replace("FeatureSlice.New.Generation.", string.Empty));
            }
        }

        var extensionsList = new List<string>();
        foreach(var i in methods)
        {
            var onlyTypes = i.Replace("IMethod<", string.Empty).Replace(">", string.Empty).Replace(" ", string.Empty);
            var types = onlyTypes.Split(',');
            var request = types[0];
            var response = types[1];
            var code = $$"""
                public static {{response}} Send<T>(this IDispatcher<T> dispatcher, {{request}} request)
                    where T : {{i}}
                {
                    return {{i}}.Send(dispatcher, request);
                }
            """;

            extensionsList.Add(code);
        }
        //Method
        AddExtensions("FeatureSlice.New.Generation.Method", extensionsList, context);
        AddMethodNames(methods, context);
    }

    // public static IExampleMethod.Response Send<T>(this IDispatcher<T> dispatcher, IExampleMethod.Request request)
    //     where T : IExampleMethod
    // {
    //     return IExampleMethod.Send(dispatcher, request);
    // }

    private static void AddExtensions(string extensionNameSpace, IReadOnlyCollection<string> extensionsList, SourceProductionContext context)
    {
        var extensions = string.Join("\n\n", extensionsList);

        //set namespace to be the same as class implementing it
        var extensionsClass = $$"""
        using {{extensionNameSpace}};

        namespace ServicesExtensions;

        public static class Extensions
        {
        {{extensions}}
        }
        """;

        context.AddSource("extensionNameSpace.extensions.g.cs", extensionsClass);
        
    }

    private static void AddMethodNames(IReadOnlyCollection<string> methods, SourceProductionContext context)
    {

        var methodNames = methods.Select(x => $"\"{x}\"").ToArray();

        var names = string.Join(",\n        ", methodNames);

        var theCode = $$"""
        namespace ClassesList;

        public static class ClassesNamesList
        {
            public static List<string> Names = new ()
            {
                {{names}}
            };
        }
        """;


        context.AddSource("MethodNames.g.cs", theCode);
    }
}


