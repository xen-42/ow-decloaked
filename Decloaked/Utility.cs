using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decloaked
{
    public static class Utility
    {
        public static void Log(string message, MessageType messageType = MessageType.Info)
        {
            Decloaked.Instance.ModHelper.Console.WriteLine(message, messageType);
        }

        public static int AddUI(string text)
        {
            var uiTable = TextTranslation.Get().m_table.theUITable;

            var key = uiTable.Keys.Max() + 1;
            try
            {
                // Ensure it doesn't already contain our UI entry
                KeyValuePair<int, string> pair = uiTable.First(x => x.Value.Equals(text));
                if (pair.Equals(default(KeyValuePair<int, string>))) key = pair.Key;
            }
            catch (Exception) { }

            TextTranslation.Get().m_table.Insert_UI(key, text);

            return key;
        }
    }
}
