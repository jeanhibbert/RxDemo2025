using Pricing.Services;
using RxAspireApp.ApiService.Pricing.Extensions;
using RxAspireApp.ApiService.Strategy.Extensions;
using RxAspireApp.Domain.Strategy.Services;
using RxDemo.Common.Strategy.DTO;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Pricing
builder.Services.AddHub();
builder.Services.AddPricingServices();

// Backend strategy
builder.Services.AddStrategyServices();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Swagger services
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(opts =>
{
    opts.InferSecuritySchemes = true;
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(setup =>
{
    setup.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET Podcasts API");
});

app.UseCors();
app.MapHub<PricingHub>("/pricinghub");

var _strategyManagementService = app.Services.GetRequiredService<IHostedServiceAccessor<IStrategyManagementService>>();

app.MapGet("/api/strategies", () =>
{
    var strategies = _strategyManagementService.Service.GetAllStrategies();
    return Results.Ok(strategies);
});

app.MapPost("/api/registerstrategy", (StrategyDetailsDto strategyDetails) =>
{
    var strategyIds = _strategyManagementService.Service.RegisterStrategy(strategyDetails);
    return Results.Ok(strategyIds);
});

app.MapDelete("/api/unregisterstrategy/{id}", (string id) =>
{
    var success = _strategyManagementService.Service.UnregisterStrategy(id);
    if (success)
        return Results.Ok();
    else
        return Results.NotFound();
});

var priceFeed = app.Services.GetRequiredService<IPriceFeed>();
priceFeed.Start();

app.MapDefaultEndpoints();

app.Run();
