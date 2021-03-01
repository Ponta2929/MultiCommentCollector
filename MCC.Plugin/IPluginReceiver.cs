using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Plugin
{
    public interface IPluginReceiver : IPluginBase
    {
        void Receive(CommentData comment);
    }
}
