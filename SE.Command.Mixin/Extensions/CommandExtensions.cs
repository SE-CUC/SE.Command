using System;

namespace IngameScript
{
    public static class CommandExtensions
    {
        public static SEApplicationBuilder AddCommands(this SEApplicationBuilder builder)
        {
        
            builder.Services.AddSingleton<ICommandService>(sp => new CommandService(sp.GetService<ILogger>()));
           
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
    }
}