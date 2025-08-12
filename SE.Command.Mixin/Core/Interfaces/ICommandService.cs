namespace IngameScript
{
    public interface ICommandService
    {
        void RegisterModule(ICommandModule module);
        void Execute(string argument);
    }
}
