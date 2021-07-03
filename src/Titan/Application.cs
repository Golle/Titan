namespace Titan
{
    public abstract class Application
    {
        public virtual void OnStart() { }
        public virtual void OnTerminate() { }
        public virtual void ConfigureSystems(SystemsCollection collection) { }

        public GameWindow Window { get; internal set; }
    }
}
