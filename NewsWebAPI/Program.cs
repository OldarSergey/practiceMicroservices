using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsMicroservice.Infrastructure.Data;
using NewsMicroservice.Infrastructure.DependencyInjection;
using NewsMicroservice.Infrastructure.Rep.gRPC;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var configuration = builder.Configuration;

        builder.Services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Product API"
            });

            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
            });
        });

        builder.Services.InfrastructureServices(builder.Configuration);

        builder.Services.AddGrpc();

        var app = builder.Build();

        app.UseRouting();

        //app.UseHttpsRedirection();

        app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGrpcService<NewsServiceGrpc>();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<NewsDbContext>();

            if (dbContext.Database.IsRelational())
            {
                dbContext.Database.Migrate();
            }
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Run();
    }
}