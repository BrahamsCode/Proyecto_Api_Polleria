using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.PolleriaMappers;
using Proyecto_Api_Polleria.Repositorio;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
                opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

// 🔑 JWT Configuration
var key = builder.Configuration.GetValue<string>("APISettings:Secreta");
if (string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("JWT Secret key is not configured");
}
var keyBytes = Encoding.ASCII.GetBytes(key);

// Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<IPedidoRepositorio, PedidoRepositorio>();
builder.Services.AddScoped<IDetallePedidoRepositorio, DetallePedidoRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// 💾 CACHE CONFIGURATION
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Límite de 1024 entradas
});

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1MB máximo
    options.UseCaseSensitivePaths = false;
});

// 🌐 CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5030") // tu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Permitir cookies/credenciales
    });
});

// Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(PolleriaMapper));

// Controladores con filtro de caché global (opcional)
builder.Services.AddControllers(options =>
{
    // 🗄️ Aplicar caché global solo a métodos GET de lectura
    // options.Filters.Add(new GlobalCacheFilter(300)); // Descomenta si quieres caché global
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 📖 Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Polleria API", 
        Version = "v1",
        Description = "API para gestión de pollería con autenticación JWT y caché"
    });
    
    // 🔐 JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \n\n " +
                      "Enter 'Bearer' [space] and then your token in the text input below.\n\n" +
                      "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// 🔒 JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, // Validar expiración del token
        ClockSkew = TimeSpan.Zero // No tolerancia de tiempo
    };
    
    // 🔍 Eventos para debugging (opcional - remover en producción)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            }
            return Task.CompletedTask;
        }
    };
});

// 🛡️ Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("User", "Admin"));
    
    // Política personalizada para recursos propios
    options.AddPolicy("SameUser", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polleria API V1");
        c.DocumentTitle = "Polleria API - Swagger";
    });
}

// 🌐 CORS debe ir antes de Authentication
app.UseCors("AllowFrontend");

// 💾 Response Caching debe ir antes de Authentication
app.UseResponseCaching();

// 🔒 Authentication & Authorization Pipeline
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.UseHttpsRedirection();
app.MapControllers();

Console.WriteLine("🚀 Polleria API iniciada con JWT y Caché configurados");
app.Run();