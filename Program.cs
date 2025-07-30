using AnhApi.Esquemas;
using AnhApi.Mapeos; // Ajustado para MappingProfile
using AnhApi.Modelos;
using AnhApi.Nucleo;
using AnhApi.Servicios;
using AnhApi.Interfaces;
using AutoMapper; // Necesario para AutoMapper
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting; // <-- Añadido para Assembly busqueda de servicios en los programas disponibles 

DotNetEnv.Env.Load();
var entorno = Environment.GetEnvironmentVariable("ENTORNO_ASPNETCORE");
if (!string.IsNullOrWhiteSpace(entorno))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", entorno);
}

var configuracionLogger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "AnhApi")
    .Enrich.WithProperty("MchineName", Environment.MachineName)
    .Enrich.WithProperty("ProcessId", Environment.ProcessId)
    .Enrich.WithProperty("TreadId", Environment.CurrentManagedThreadId);

// Opciones del Servidor
var builder = WebApplication.CreateBuilder(args);

//Incluir la configuracion de los archivos de parametros de la aplicacion
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{entorno}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configuracion de salida de los logs, podria ser diferente en desarrollo y produccion
if (builder.Environment.IsDevelopment())
{
    configuracionLogger.WriteTo.Console();
}
else
{
    //Configuracion de salida de los logs podria implementarse en un servidor de correo para alertas del servicio
    configuracionLogger.WriteTo.File(
        path: Path.Combine("logs", "Api-log-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
}

Log.Logger = configuracionLogger.CreateLogger();

// Configuración de la base de datos
builder.Services.Configure<BdPostgres>(builder.Configuration.GetSection("BDPosgres"));


// Configuracion de las opciones LDAP 
builder.Services.Configure<LdapOptions>(builder.Configuration.GetSection("LdapOptions"));

// Configuración una unica instancia de BdPostgres para ser usada en toda la aplicación
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<BdPostgres>>();
    return options.Value;
});

// Configuración de las opciones de esquema de autenticación
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(op =>
    {
        op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        op.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.NoResult();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\":\"Token de autenticación inválido o expirado.\"}");
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\":\"Necesitas autenticarte para acceder a este recurso.\"}");
            }
        };
    });

// Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<AnhApi.Datos.ContextoAppBD>((serviceProvider, options) =>
{
    var bdPostgres = serviceProvider.GetRequiredService<BdPostgres>();
    options.UseNpgsql(bdPostgres.GetConnectionString());
});


// Configurar la política de Demasiadas Solicitudes (Rate Limiting) abuso sobrecarga
builder.Services.AddRateLimiter(options => {
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fijo", opt =>
    {
        opt.PermitLimit = 50; // Número máximo de solicitudes permitidas
        //opt.Window = TimeSpan.FromSeconds(20); // Ventana de tiempo de 5 segundos para prueba
        opt.Window = TimeSpan.FromMinutes(1); // Ventana de tiempo de 1 minuto
    });
});



// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// --- Registro de servicios por convención los servicios a registrar empieza con I[NombreClaseServicio] ---
// Registrar el servicio genérico IGenericoServicio<,> -> GenericoServicio<,>
// Esta línea es crucial y se mantiene porque es un tipo genérico abierto.
builder.Services.AddScoped(typeof(IAuditoriaServicio<,>), typeof(AuditoriaServicio<,>));

// Obtener todas las clases de servicio del ensamblado actual que no son abstractas ni genéricas abiertas.
// Esto incluirá PersonaServicio, ParametroServicio, AuthServicio.
var servicios = typeof(Program).Assembly.GetTypes()
    .Where(t => t.Name.EndsWith("Servicio") && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition);

foreach (var servicio in servicios)
{
    // Buscar una interfaz que siga la convención I[NombreClaseServicio] (ej. IAuthServicio para AuthServicio)
    var interfaz = servicio.GetInterfaces().FirstOrDefault(i =>
        i.Name == $"I{servicio.Name}");

    if (interfaz != null)
    {
        // Si la interfaz se encuentra (ej. IAuthServicio), se registra la implementación de la interfaz
        builder.Services.AddScoped(interfaz, servicio);
    }
    else
    {
        // Si no se encuentra una interfaz I[NombreClaseServicio] (ej. PersonaServicio no tiene IPersonaServicio,
        // pero hereda de IGenericoServicio<,> o ParametroServicio que no tiene interfaz personalizada),
        // se registra la clase de servicio directamente.
        // Esto permite la inyección directa por el tipo concreto si no hay una interfaz específica o si se hereda.
        builder.Services.AddScoped(servicio);
    }
}
// --- FIN Registro de convención ---

builder.Services.AddAuthorization(opciones =>
{
    opciones.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Adicionar servicios al contenedor
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
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

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa el token JWT en este formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
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
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();