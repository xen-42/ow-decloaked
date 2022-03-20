using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Decloaked
{
    public static class Utility
    {
        public static void Log(string message, MessageType messageType = MessageType.Info)
        {
            Decloaked.Instance.ModHelper.Console.WriteLine(message, messageType);
        }
    }
}
