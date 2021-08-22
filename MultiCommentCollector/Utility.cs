using MCC.Utility.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector
{
    public static class Utility
    {
        public static string ApplicationPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public static void SaveToXml<T>(string fileName, object @object)
        {
            try
            {
                XmlSerializer.FileSerialize<T>($"{ApplicationPath}\\{fileName}", @object);
            }
            catch
            {

            }
        }

        public static void SaveToJson<T>(string fileName, object @object)
        {
            try
            {
                JsonSerializer.FileSerialize<T>($"{ApplicationPath}\\{fileName}", @object);
            }
            catch
            {

            }
        }

        public static T LoadToXml<T>(string fileName) where T : new()
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

        public static T LoadToJson<T>(string fileName) where T : new()
        {
            try
            {
                return JsonSerializer.FileDeserialize<T>($"{ApplicationPath}\\{fileName}");
            }
            catch
            {
                return new();
            }
        }
    }
}
