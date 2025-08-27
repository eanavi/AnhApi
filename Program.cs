using AnhApi.Esquemas;
using AnhApi.Mapeos; // Ajustado para MappingProfile
using AnhApi.Nucleo;
using AnhApi.Servicios;
using AnhApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog; // Necesario para usar Log.Information()
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite;

DotNetEnv.Env.Load();
var entorno = Environment.GetEnvironmentVariable("ENTORNO_ASPNETCORE");
if (!string.IsNullOrWhiteSpace(entorno))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", entorno);
}

//var httpPort = Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORT");
//var httpsPort = Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT");

var configuracionLogger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Asegura que los mensajes Debug/Information sean capturados
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "AnhApi")
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.WithProperty("ProcessId", Environment.ProcessId)
    .Enrich.WithProperty("TreadId", Environment.CurrentManagedThreadId);

// Opciones del Servidor
var builder = WebApplication.CreateBuilder(args);

/*cors
//Configuracion del puerto http de acuerdo a la variable de .env
if(!string.IsNullOrEmpty(httpPort) && int.TryParse(httpPort, out var httpPortNumber))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(httpPortNumber);
    });
}

//Configuracion del puerto https de acuerdo a la variable de .env
if(!string.IsNullOrEmpty(httpsPort) && int.TryParse(httpsPort, out var httpsPortNumber))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(httpsPortNumber, listenOptions =>
        {
            //Configuracion certificado
            //listenOptions.useHttps();
        });
    });
}
*/

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
        path: Path.Combine(AppContext.BaseDirectory, "logs", "api-log-.txt"),
        rollingInterval: RollingInterval.Day,
        shared:true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
}

Log.Logger = configuracionLogger.CreateLogger(); // Configura el logger estático de Serilog

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
    options.UseNpgsql(bdPostgres.GetConnectionString(), x => x.UseNetTopologySuite());
});


// Configurar la política de Demasiadas Solicitudes (Rate Limiting) abuso sobrecarga
builder.Services.AddRateLimiter(options => {
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fijo", opt =>
    {
        opt.PermitLimit = 20; // Número máximo de solicitudes permitidas
        opt.Window = TimeSpan.FromSeconds(20); // Ventana de tiempo de 5 segundos para prueba
        //opt.Window = TimeSpan.FromMinutes(1); // Ventana de tiempo de 1 minuto
    });
});



// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var env = builder.Environment;

/*

//Configuracion de CORSs
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCors", policy =>
    {
        if (env.IsDevelopment())
        {

            policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
            //Dominios permitidos
        }
        else
        {
            policy.WithOrigins(
                "https://localhost:5001", 
                "http://localhost:5000", 
                "http://127.0.0.1:5000", 
                "http://0.0.0.1:5000", 
                "http://anh.gov.bo"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();


        }

        // Configuración de CORS para producción




    });
});*/


// --- Registro de servicios por convención los servicios a registrar empieza con I[NombreClaseServicio] ---
// Registrar el servicio genérico IGenericoServicio<,> -> GenericoServicio<,>
// Esta línea es crucial y se mantiene porque es un tipo genérico abierto.
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
//app.UseCors("PoliticaCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();