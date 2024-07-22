using FeatureSlice;
using FeatureSlice.Samples.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
});

Example.Register(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

await app.MapFeatureSlices();

await app.RunAsync();