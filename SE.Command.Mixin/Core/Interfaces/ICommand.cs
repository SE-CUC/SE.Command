using System;

namespace IngameScript
{
    public interface ICommand
    {
        string Name { get; }
        string HelpText { get; }
        Action<string[]> Action { get; }
    }
}
