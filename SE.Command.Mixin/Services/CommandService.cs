using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class CommandService : ICommandService
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public CommandService(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }

        public void RegisterModule(ICommandModule module)
        {
            if (module == null)
            {
                _logger.Error("Attempted to register a null module.");
                return;
            }

            var moduleCommands = module.GetCommands();
            if (moduleCommands == null) return;

            foreach (var command in moduleCommands)
            {
                if (command == null || string.IsNullOrEmpty(command.Name))
                {
                    _logger.Error($"Module '{module.GetType().Name}' provided a null or nameless command.");
                    continue;
                }

                if (_commands.ContainsKey(command.Name))
                {
                    _logger.Error($"Command '{command.Name}' from module '{module.GetType().Name}' is already registered.");
                }
                else
                {
                    _commands[command.Name] = command;
                    _logger.Debug($"Registered command: '{command.Name}'");
                }
            }
        }

        public void Execute(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument)) return;

            var parts = argument.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var commandName = parts[0].ToLower();
            var parameters = parts.Skip(1).ToArray();

            ICommand command;
            if (!_commands.TryGetValue(commandName, out command))
            {
                _logger.Error($"Unknown command: '{commandName}'");
                return;
            }

            try
            {
                _logger.Info($"Executing: '{command.Name}'...");
                command.Action(parameters);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception in command '{command.Name}': {ex.Message}");
            }
        }
    }
}
