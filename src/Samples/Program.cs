using FeatureSlice;
using FeatureSlice.Samples.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Example.Register(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapFeatureSlices();

await app.RunAsync();