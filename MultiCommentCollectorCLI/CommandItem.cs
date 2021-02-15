using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollectorCLI
{
    public class CommandItem
    {
        public string Command { get; set; }

        public List<string> Argv { get; set; }
    }
}
