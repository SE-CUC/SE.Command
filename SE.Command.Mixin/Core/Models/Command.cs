using System;

namespace IngameScript
{
    public class Command : ICommand
    {
        public string Name { get; }
        public string HelpText { get; }
        public Action<string[]> Action { get; }

        public Command(string name, string helpText, Action<string[]> action)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Name = name.ToLower();
            HelpText = helpText;
            Action = action;
        }
    }
}
