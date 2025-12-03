using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Services;
using ASMS.Services.Interfaces;
using ASMS.Services.Mappings;
using ASMS.Services.Model.Authentication;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ASMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.ConfigureServicesLayers(builder.Configuration);

            builder.Services.ConfigureRepositoryServices();

            //Sending Mail
            builder.Services.Configure<ProjectMailConfig>(
            builder.Configuration.GetSection("ProjectMail"));

            //Auto Mapper
            var mapperKey = builder.Configuration["KeyAutoMapper:Key"];
            builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = mapperKey, typeof(Program));

            // JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });

            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ASMS API", Version = "v1" });

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập JWT token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtScheme, Array.Empty<string>() }
                });
            });


            builder.Services.AddDbContext<VstorageContext>(options =>
            {
                //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseSqlServer(builder.Configuration.GetConnectionString("DeployConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            // Hangfire Configuration
            var connectionString = builder.Configuration.GetConnectionString("DeployConnection");
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    SchemaName = "Hangfire"
                }));

            builder.Services.AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            builder.Services.ConfigureServicesLayers(builder.Configuration).ConfigureRepositoryServices();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "VStorage Background Jobs",
                Authorization = new[] { new AllowAllAuthorizationFilter() }
            });

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                TimeZoneInfo vietnamTimeZone;
                try
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                }

                recurringJobManager.AddOrUpdate<IOrderMaintenanceService>(
                    "check-overdue-orders",
                    service => service.CheckAndProcessOverdueOrdersAsync(),
                    "0 0 * * *",
                    new RecurringJobOptions { TimeZone = vietnamTimeZone });

                recurringJobManager.AddOrUpdate<IOrderMaintenanceService>(
                    "move-expired-orders",
                    service => service.MoveOldOverdueOrdersToExpiredStorageAsync(),
                    "0 1 * * *",
                    new RecurringJobOptions { TimeZone = vietnamTimeZone });
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VStorage API");
                });
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAllOrigins");
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; 
    }
}
