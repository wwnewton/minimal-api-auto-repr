global using MinimalApiAutoRepr.Generated;
using MinimalApiAutoRepr.TestApi.Common.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddValidation();


builder.Services.AddOpenApi(opt => opt.AddSchemaTransformer(new FullNameSchemaTransformer()));

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAutoReprEndpoints();

app.Run();

