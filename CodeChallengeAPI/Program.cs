using CodeChallengeAPI.Service;
using TwitterService;

namespace CodeChallengeAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add configuration
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environment}.json", optional: true)
                            .AddUserSecrets<Program>(optional: true)
                            .AddEnvironmentVariables();

            //Add services to composition root
            builder.Services.AddControllers();
            builder.Logging.AddDebug();

            //Add swagger support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Add application services
            builder.Services.AddTwitterServices();
            //builder.Services.AddSingleton<ITwitterAuthService, TwitterAuthService>();
            //builder.Services.AddSingleton<ITwitterStreamService, TwitterStreamService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            //Start the twitter tweet streaming process as the application starts up. The streaming service runs as a
            //singleton instance inside the application. This is so the tweet streaming service does not run, for example, per request
            //for now.
            var twitterStreamService = app.Services.GetRequiredService<TwitterService.Service.ITwitterStreamService>();
            await twitterStreamService.StartStream();

            app.Run();
        }
    }
}