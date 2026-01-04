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
    new (
        Id: 1,
        Name: "Датчик температуры",
        SerialNumber: "931013150305",
        TypeId: 1,
        Location: "Гостиная",
        Status: "on" ,
        Value: 25.36,
        Uint: "°C",
        LastUpdated: DateTime.Now,
        CreatedAt: DateTime.Parse("2026-01-01")
    ),
    new (
        Id: 2,
        Name: "Датчик отопления",
        SerialNumber: "C8QH6T96DPNG",
        TypeId: 3,
        Location: "Спальня",
        Status: "on" ,
        Value: 25.36,
        Uint: "°C",
        LastUpdated: DateTime.Now,
        CreatedAt: DateTime.Parse("2026-01-02")
    ),
    new (
        Id: 3,
        Name: "Лампочка",
        SerialNumber: "1002353BVK1990046841",
        TypeId: 2,
        Location: "Кухня",
        Status: "off" ,
        Value: 150,
        Uint: "lx",
        LastUpdated: DateTime.Now,
        CreatedAt: DateTime.Parse("2026-01-03")
    ),
];

var sensorsApi = app.MapGroup("/sensors");

sensorsApi.MapGet("/", () => sensors)
    .WithName("GetAllSensors");

sensorsApi.MapGet("/{id}", Results<Ok<Sensor>, NotFound> (int id) =>
    sensors.FirstOrDefault(sensor => sensor.Id == id) is { } sensor
        ? TypedResults.Ok(sensor)
        : TypedResults.NotFound())
    .WithName("GetSensorById");

app.Run();

public record Sensor(
    int Id, 
    string Name,
    string SerialNumber,
    int TypeId,
    string Location,
    string Status,
    double Value,
    string Uint,
    DateTime LastUpdated,
    DateTime CreatedAt
);

[JsonSerializable(typeof(Sensor[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}