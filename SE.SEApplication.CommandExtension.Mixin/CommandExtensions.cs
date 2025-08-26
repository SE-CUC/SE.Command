using System;
using System.Collections.Generic;

namespace IngameScript
{
    public static class CommandExtensions
    {
        public static SEApplicationBuilder AddCommands(
            this SEApplicationBuilder builder,
            IEnumerable<ICommandModule> modules = null,
            IEnumerable<ICommand> commands = null)
        {
            builder.Services.AddSingleton<ICommandService>(sp => 
            {
                var cs = new CommandService(sp.GetService<ILogger>());

                if (modules != null) 
                {
                    foreach (var module in modules)
                    {
                        cs.RegisterModule(module);
                    }
                }
                if(commands != null)
                {
                    cs.RegisterModule(new FakeModule(commands));
                }

                return cs;
            });

            builder.OnTerminalAction((arg, update, sp) =>
            {
                if (!string.IsNullOrWhiteSpace(arg))
                {
                    var commandService = sp.GetService<ICommandService>();
                    commandService.Execute(arg);
                }
            });

            return builder;
        }  
        
        public static SEApplicationBuilder AddCommand(this SEApplicationBuilder builder, Func<SEApplicationBuilder, SEApplicationBuilder> func) => func(builder);
    }
}