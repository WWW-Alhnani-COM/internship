using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using InternshipManagement.API.Data;
using InternshipManagement.API.Services;
using InternshipManagement.API.Middleware;


var builder = WebApplication.CreateBuilder(args);

// =============== تسجيل الخدمات (Services) ===============
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Internship Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: \"Bearer eyJhbGciOi...\""
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

// Database (MySQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// CORS - for React/Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly", p => p.RequireRole("Student"));
    options.AddPolicy("SiteSupervisorOnly", p => p.RequireRole("SiteSupervisor"));
    options.AddPolicy("AcademicSupervisorOnly", p => p.RequireRole("AcademicSupervisor"));
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

// Scoped Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<SiteSupervisorService>();
builder.Services.AddScoped<AcademicSupervisorService>();
builder.Services.AddScoped<WeeklyReportService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddControllers();
builder.Services.AddScoped<ActivityLogService>();


// =============== بناء التطبيق ===============
var app = builder.Build();

// =============== Middleware Pipeline ===============
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Internship Management API v1");
        c.RoutePrefix = string.Empty; // Swagger at root: http://localhost:5000
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // لتمكين الوصول إلى الملفات من wwwroot
app.UseCors("AllowReactApp"); 
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ActivityLoggingMiddleware>();
app.MapControllers();

// =============== تطبيق الـ Migrations عند التشغيل ===============
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();