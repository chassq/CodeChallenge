namespace TwitterServiceTest
{
    /// <summary>
    /// Base test class all test classes inherit from. Runs config setup.
    /// </summary>
    public class BaseTest
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public ConfigFixture ConfigFixture { get; set; } = new ConfigFixture();
    }
}
