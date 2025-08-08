using AnhApi.Esquemas;
using AnhApi.Mapeos; // Ajustado para MappingProfile
using AnhApi.Modelos;
using AnhApi.Nucleo;
using AnhApi.Servicios;
using AnhApi.Interfaces;
using AnhApi.Controladores;
using AutoMapper; // Necesario para AutoMapper
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog; // Necesario para usar Log.Information()
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;
using System.Diagnostics; // <-- A�adido para Assembly busqueda de servicios en los programas disponibles

DotNetEnv.Env.Load();
var entorno = Environment.GetEnvironmentVariable("ENTORNO_ASPNETCORE");
if (!string.IsNullOrWhiteSpace(entorno))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", entorno);
}

var configuracionLogger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Asegura que los mensajes Debug/Information sean capturados
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
    configuracionLogger.WriteTo.Console(); // Escribe a la consola en desarrollo
}
else
{
    //Configuracion de salida de los logs podria implementarse en un servidor de correo para alertas del servicio
    configuracionLogger.WriteTo.File(
        path: Path.Combine("logs", "Api-log-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
}

Log.Logger = configuracionLogger.CreateLogger(); // Configura el logger est�tico de Serilog

// Configuraci�n de la base de datos
builder.Services.Configure<BdPostgres>(builder.Configuration.GetSection("BDPosgres"));


// Configuracion de las opciones LDAP
builder.Services.Configure<LdapOptions>(builder.Configuration.GetSection("LdapOptions"));

// Configuraci�n una unica instancia de BdPostgres para ser usada en toda la aplicaci�n
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<BdPostgres>>();
    return options.Value;
});

// Configuraci�n de las opciones de esquema de autenticaci�n
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
                return context.Response.WriteAsync("{\"message\":\"Token de autenticaci�n inv�lido o expirado.\"}");
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


// Configurar la pol�tica de Demasiadas Solicitudes (Rate Limiting) abuso sobrecarga
builder.Services.AddRateLimiter(options => {
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fijo", opt =>
    {
        opt.PermitLimit = 50; // N�mero m�ximo de solicitudes permitidas
        //opt.Window = TimeSpan.FromSeconds(20); // Ventana de tiempo de 5 segundos para prueba
        opt.Window = TimeSpan.FromMinutes(1); // Ventana de tiempo de 1 minuto
    });
});



// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// --- Registro de servicios por convenci�n los servicios a registrar empieza con I[NombreClaseServicio] ---
// Registrar el servicio gen�rico IGenericoServicio<,> -> GenericoServicio<,>
// Esta l�nea es crucial y se mantiene porque es un tipo gen�rico abierto.
builder.Services.AddScoped(typeof(IServicioAuditoria<,>), typeof(ServicioAuditoria<,>));

//Inyeccion de dependencias por convencion  --------  Registra todos los servicios que se inicien con "Servicio"
var servicios = typeof(Program).Assembly.GetTypes()
    .Where(t => t.Name.StartsWith("Servicio") && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition);

//Inyeccion de dependencias por convencion --------  Registra todos los servicios que se inicien con "Iservicio"
//esto se puede cambiar para un entorno de pruebas o desarrollo
foreach (var servicio in servicios)
{
    var interfaz = servicio.GetInterfaces().FirstOrDefault(i =>
        i.Name == $"I{servicio.Name}"); 

    if (interfaz != null)
    {
        builder.Services.AddScoped(interfaz, servicio);
    }
    else
    {
        builder.Services.AddScoped(servicio);
    }
}
// --- FIN Registro de convenci�n ---

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

//Incluir logging (esto configura el logging para ILogger<T> inyectado en otras clases)
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
        Description = "API RESTful para manejo de usuarios, personas, sistemas y m�dulos.",
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

    // Configuraci�n para JWT en Swagger
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