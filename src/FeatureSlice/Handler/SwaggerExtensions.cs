using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FeatureSlice;

public static class SwaggerExtensions
{
    public static SwaggerGenOptions SetCustomSchemaId(this SwaggerGenOptions options)
    {
        var schemaHelper = new SwaggerSchemaIdHelper();
        options.CustomSchemaIds(schemaHelper.GetSchemaId);

        return options;
    }
}

public sealed class SwaggerSchemaIdHelper
{
    private readonly Dictionary<string, HashSet<Type>> _schemaNameRepetition = new();

    public string GetSchemaId(Type modelType)
    {
        var id = DefaultSchemaIdSelector(modelType);

        if (_schemaNameRepetition.TryGetValue(id, out var value) == false)
        {
            _schemaNameRepetition[id] = [modelType];

            return id;
        }

        if(value.Contains(modelType) && value.Count == 1)
        {
            return id;
        }

        value.Add(modelType);

        return modelType.FullName!.Replace("+", ".");
    }

    // borrowed from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/95cb4d370e08e54eb04cf14e7e6388ca974a686e/src/Swashbuckle.AspNetCore.SwaggerGen/SchemaGenerator/SchemaGeneratorOptions.cs#L44
    private string DefaultSchemaIdSelector(Type modelType)
    {
        if (modelType.IsConstructedGenericType == false)
        {
            return modelType.Name.Replace("[]", "Array");
        }

        var prefix = modelType.GetGenericArguments()
            .Select(GetSchemaId)
            .Aggregate((previous, current) => previous + current);

        return $"{prefix}{modelType.Name.Split('`').First()}";
    }
}