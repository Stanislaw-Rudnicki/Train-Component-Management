using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using TrainComponent.Infrastructure.ErrorHandling;
using TrainComponent.Infrastructure.Persistence;

namespace TrainComponent;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .CreateLogger();

        try
        {
            Log.Information("Starting up");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            ConfigureServices(builder);

            var app = builder.Build();

            ApplyMigrations(app);

            ConfigurePipeline(app);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Train Component Management API",
                    Version = "v1",
                    Description =
                        "API for managing train components, including quantity assignment and search functionality."
                }
            );

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.OperationFilter<AddResponseHeadersFilter>();
        });

        services.AddDbContext<TrainDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Train Component API v1");
                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "Train Component Management API Docs";
            });
        }

        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<TrainDbContext>();

        context.Database.Migrate();

        var configuration = services.GetRequiredService<IConfiguration>();
        bool enableFts = configuration.GetValue<bool>("EnableFullTextSearch");

        if (!enableFts)
        {
            Log.Information("Full-Text Search is disabled by configuration.");
            return;
        }

        try
        {
            Log.Information("Attempting to create Full-Text Search structures...");

            var connection = context.Database.GetDbConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                @"
                    IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'ftCatalog')
                    BEGIN
                        CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;
                    END;

                    IF NOT EXISTS (
                        SELECT * FROM sys.fulltext_indexes i
                        JOIN sys.objects o ON i.object_id = o.object_id
                        WHERE o.name = 'Components'
                    )
                    BEGIN
                        CREATE FULLTEXT INDEX ON Components(Name) 
                        KEY INDEX PK_Components;
                    END;
                ";

            command.ExecuteNonQuery();

            Log.Information("Full-Text Search structures created.");
        }
        catch (SqlException ex) when (ex.Message.Contains("Full-Text Search"))
        {
            Log.Warning("Full-Text Search is not installed or could not be initialized.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while setting up Full-Text Search.");
        }
    }
}
