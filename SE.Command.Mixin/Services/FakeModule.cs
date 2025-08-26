using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public class FakeModule : ICommandModule
    {
        private readonly IEnumerable<ICommand> _commands;

        public FakeModule(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public IEnumerable<ICommand> GetCommands() => _commands;
    }
}
