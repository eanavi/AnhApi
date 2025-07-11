using EsqPersona = AnhApi.Esquemas.Persona;
using AnhApi.Mapeos; // Ajustado para MappingProfile
using AnhApi.Modelos;
using AnhApi.Nucleo;
using AnhApi.Servicios;
using AutoMapper; // Necesario para AutoMapper
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

DotNetEnv.Env.Load();
var entorno = Environment.GetEnvironmentVariable("ENTORNO_ASPNETCORE");
if (!string.IsNullOrWhiteSpace(entorno))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", entorno);
}

// Opciones del Servidor
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{entorno}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configuración de la base de datos
builder.Services.Configure<BdPostgres>(builder.Configuration.GetSection("BDPosgres"));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<BdPostgres>>();
    return options.Value;
});

builder.Services.AddDbContext<AnhApi.Datos.AppDbContext>((serviceProvider, options) =>
{
    var bdPostgres = serviceProvider.GetRequiredService<BdPostgres>();
    options.UseNpgsql(bdPostgres.GetConnectionString());
});

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Registrar el servicio genérico con el DTO EsqPersona
builder.Services.AddScoped<IGenericoServicio<Persona, EsqPersona, Guid>, PersonaServicio>();

// Adicionar servicios al contenedor
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

//Incluir logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservicio de Login ANH",
        Version = "v1",
        Description = "API RESTful para manejo de usuarios, personas, sistemas y módulos.",
        Contact = new OpenApiContact
        {
            Name = "Equipo ANH",
            Email = "soporte@anh.gob.bo"
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de Uso en la ANH",
            Url = new Uri("https://example.com/license")
        }
    });
});

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();