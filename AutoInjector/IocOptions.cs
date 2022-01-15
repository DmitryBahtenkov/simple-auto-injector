namespace AutoInjector
{
    /// <summary>
    /// options for DI-Container
    /// </summary>
    public record IocOptions
    {
        public string ProjectKey { get; set; }
        
        public static IocOptions Empty => new();
    }
}