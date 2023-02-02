using CodeChallengeAPI.Service;

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

            //Add swagger support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ITwitterStreamService, TwitterStreamService>();

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
            //singleton instance inside the application.
            var twitterStreamService = app.Services.GetRequiredService<ITwitterStreamService>();
            await twitterStreamService.StartStream();

            app.Run();
        }
    }
}