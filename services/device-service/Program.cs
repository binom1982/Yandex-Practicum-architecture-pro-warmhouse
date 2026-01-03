using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);

});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
         c.SwaggerEndpoint("/openapi/v1.json", "API Sensor Service v1")
    );
   
//}

Sensor[] sensors = [
    new (Guid.NewGuid(), "Датчик температуры","931013150305", "on"),
    new (Guid.NewGuid(), "Датчик отопления", "C8QH6T96DPNG", "on"),
    new (Guid.NewGuid(), "Лампочка", "1002353BVK1990046841" ,"off")
];

var sensorsApi = app.MapGroup("/sensors");
sensorsApi.MapGet("/", () => sensors)
    .WithName("GetAllSensors");

sensorsApi.MapGet("/{id}", Results<Ok<Sensor>, NotFound> (Guid id) =>
    sensors.FirstOrDefault(sensor => sensor.Id == id) is { } sensor
        ? TypedResults.Ok(sensor)
        : TypedResults.NotFound())
    .WithName("GetSensorById");

app.Run();


public record Sensor(Guid Id, string Name, string SerialNumber, string Status);

[JsonSerializable(typeof(Sensor[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}