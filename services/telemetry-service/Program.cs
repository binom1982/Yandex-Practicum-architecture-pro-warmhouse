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
         c.SwaggerEndpoint("/openapi/v1.json", "API Telemetry Service v1")
    );
//}

TelemetryRecord[] telemetryRecords = [
    new (
        Id: 1,
        CreatedAt: DateTime.Parse("2026-01-01T05:04:56"),
        SensorId: 1,
        Metric: "Датчик температуры",
        Value: 25.36,
        Uint: "°C",
        Status: "on"
    ),
    new (
        Id: 2,
        CreatedAt: DateTime.Parse("2026-01-01T05:15:56"),
        SensorId: 1,
        Metric: "Датчик температуры",
        Value: 26.36,
        Uint: "°C",
        Status: "on"
    ),
    new (
        Id: 3,
        CreatedAt: DateTime.Parse("2026-01-01T06:04:56"),
        SensorId: 2,
        Metric: "Датчик отопления",
        Value: 17.36,
        Uint: "°C",
        Status: "on"
    ),
    new (
        Id: 4,
        CreatedAt: DateTime.Parse("2026-01-01T06:15:56"),
        SensorId: 2,
        Metric: "Датчик отопления",
        Value: 20.36,
        Uint: "°C",
        Status: "on"
    ),
    new (
        Id: 5,
        CreatedAt: DateTime.Parse("2026-01-01T07:04:56"),
        SensorId: 3,
        Metric: "Лампочка",
        Value: 0,
        Uint: "lx",
        Status: "off"
    ),
    new (
        Id: 6,
        CreatedAt: DateTime.Parse("2026-01-01T08:15:56"),
        SensorId: 3,
        Metric: "Лампочка",
        Value: 150,
        Uint: "lx",
        Status: "on"
    )
];

var sensorsApi = app.MapGroup("/telemetry");

sensorsApi.MapGet("/", () => telemetryRecords)
    .WithName("GetAllTelemetryRecords");

sensorsApi.MapGet("/{id}", Results<Ok<TelemetryRecord>, NotFound> (int id) =>
    telemetryRecords.FirstOrDefault(sensor => sensor.Id == id) is { } theTelemetryRecord
        ? TypedResults.Ok(theTelemetryRecord)
        : TypedResults.NotFound())
    .WithName("GetTelemetryRecordById");

app.Run();

public record TelemetryRecord(
    int Id,
    DateTime CreatedAt,
    int SensorId,
    string Metric,
    double Value,
    string Uint,
    string Status
);

[JsonSerializable(typeof(TelemetryRecord[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}