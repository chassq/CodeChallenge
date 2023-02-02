using Microsoft.Extensions.DependencyInjection;
using TwitterService.Service;

namespace TwitterService
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Adds Twitter services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddTwitterServices(this IServiceCollection services)
        {
            //Add application services
            services.AddSingleton<ITwitterAuthService, TwitterAuthService>();
            services.AddSingleton<ITwitterStreamService, TwitterStreamService>();

            return services;
        }
    }
}
