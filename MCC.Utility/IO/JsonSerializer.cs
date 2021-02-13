using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MCC.Utility.IO
{
    /// <summary>
    /// クラスのJsonシリアライズ・デシリアライズを行います。
    /// </summary>
    public static class JsonSerializer
    {
        public static DataContractJsonSerializerSettings Settings = new()
        {
            DateTimeFormat = new("yyyy-MM-dd'T'HH:mm:ss.fffffffK")
        };

        /// <summary>
        /// Xmlデータをファイルに書き込みます。
        /// </summary>
        /// <param name="fileName">対象のファイル名。</param>
        public static void FileSerialize<T>(string fileName, object @object)
        {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                (new DataContractJsonSerializer(typeof(T), Settings)).WriteObject(stream, @object);
        }

        /// <summary>
        /// Xmlデータをファイルから読み込みます。
        /// </summary>
        /// <param name="fileName">対象のファイル名。</param>
        public static T FileDeserialize<T>(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                return (T)(new DataContractJsonSerializer(typeof(T), Settings)).ReadObject(stream);
        }

        /// <summary>
        /// Jsonデータを文字列として取得します。
        /// </summary>
        /// <returns>Jsonデータ</returns>
        public static string Serialize<T>(object @object)
        {
            using (var stream = new MemoryStream())
            {
                (new DataContractJsonSerializer(typeof(T), Settings)).WriteObject(stream, @object);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Jsonデータを文字列から読み込みます。
        /// </summary>
        public static T Deserialize<T>(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return (T)(new DataContractJsonSerializer(typeof(T), Settings)).ReadObject(stream);
        }
    }

    /// <summary>
    /// クラスのシリアライズ・デシリアライズを行います。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonSerializer<T>
    {
        /// <summary>
        /// Jsonデータをファイルに書き込みます。
        /// </summary>
        /// <param name="fileName">対象のファイル名。</param>
        public void FileSerialize(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                (new DataContractJsonSerializer(typeof(T), JsonSerializer.Settings)).WriteObject(stream, this);
        }

        /// <summary>
        /// Jsonデータをファイルから読み込みます。
        /// </summary>
        /// <param name="fileName">対象のファイル名。</param>
        public void FileDeserialize(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var t = (T)(new DataContractJsonSerializer(typeof(T), JsonSerializer.Settings)).ReadObject(stream);

                foreach (var info in t.GetType().GetFields())
                    info.SetValue(this, info.GetValue(t));

                foreach (var info in t.GetType().GetProperties())
                {
                    if (info.CanWrite)
                    {
                        info.SetValue(this, info.GetValue(t));
                    }
                }
            }
        }

        /// <summary>
        /// Xmlデータを文字列として取得します。
        /// </summary>
        /// <returns>Xmlデータ</returns>
        public string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                (new DataContractJsonSerializer(typeof(T), JsonSerializer.Settings)).WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Xmlデータを文字列から読み込みます。
        /// </summary>
        public void Deserialize(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var t = (T)(new DataContractJsonSerializer(typeof(T), JsonSerializer.Settings)).ReadObject(stream);

                foreach (var info in t.GetType().GetFields())
                    info.SetValue(this, info.GetValue(t));

                foreach (var info in t.GetType().GetProperties())
                {
                    if (info.CanWrite)
                    {
                        info.SetValue(this, info.GetValue(t));
                    }
                }
            }
        }
    }
}