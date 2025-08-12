using System.Collections.Generic;

namespace IngameScript
{
    public interface ICommandModule
    {
        IEnumerable<ICommand> GetCommands();
    }
}
