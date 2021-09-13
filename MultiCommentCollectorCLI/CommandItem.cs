using System.Collections.Generic;

namespace MultiCommentCollectorCLI
{
    public class CommandItem
    {
        public string Command { get; set; }

        public List<string> Argv { get; set; }
    }
}
