using MCC.Utility.IO;
using System;
using System.IO;

namespace MultiCommentCollector.Helper
{
    public static class SerializerHelper
    {
        public static string ApplicationPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public static void XmlSerialize(string fileName, object @object)
        {
            try
            {
                XmlSerializer.FileSerialize($"{ApplicationPath}\\{fileName}", @object);
            }
            catch
            {

            }
        }

        public static T XmlDeserialize<T>(string fileName) where T : new()
        {
            try
            {
                return XmlSerializer.FileDeserialize<T>($"{ApplicationPath}\\{fileName}");
            }
            catch
            {
                return new();
            }
        }
    }
}
