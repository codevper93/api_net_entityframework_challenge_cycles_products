using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Entity Framework con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controladores
builder.Services.AddControllers();

// Configuración de CORS (IMPORTANTE: debe ir antes de app.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Productos",
        Version = "v1",
        Description = "Una API para gestionar productos en el sistema.",
        Contact = new OpenApiContact
        {
            Name = "Gastón",
            Email = "bailadorgaston93@gmail.com",
            Url = new Uri("https://codevper93.vercel.app")
        }
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "ProductApi.xml");
    c.IncludeXmlComments(xmlFile);
});

// Autorización (por ahora sin autenticación, usando una MasterKey personalizada)
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware para Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Productos v1");
    });
}

app.UseHttpsRedirection();

// CORS (aplicar la política aquí)
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
