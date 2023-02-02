using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitterService;

namespace TwitterServiceTest
{
    public class ConfigFixture : IDisposable
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; set; }

        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFixture"/> class.
        /// </summary>
        public ConfigFixture()
        {

            Configuration = new ConfigurationBuilder()
                                    .AddUserSecrets<ConfigFixture>(optional: true)                                    
                                    .Build();

            Services = new ServiceCollection()
                                    .AddSingleton(Configuration)  
                                    .AddLogging()
                                    .AddTwitterServices()                                    
                                    .BuildServiceProvider();          
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }
    }
}
