using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MCC.Utility
{
    [JsonConverter(typeof(ColorDataJsonConverter))]
    [Serializable]
    public struct ColorData
    {
        /// <summary>
        /// 透明度
        /// </summary>
        public int A { get; set; }
        /// <summary>
        /// 赤度
        /// </summary>
        public int R { get; set; }
        /// <summary>
        /// 緑度
        /// </summary>
        public int G { get; set; }
        /// <summary>
        /// 青度
        /// </summary>
        public int B { get; set; }

        public static ColorData FromArgb(int a, int r, int g, int b)
            => new() { A = a, R = r, G = g, B = b };

        public ColorData ToRgb()
            => new() { A = 255, R = R, G = G, B = B };

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => A ^ R ^ G ^ B;

        public override string ToString()
            => string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
    }

    public class ColorDataJsonConverter : JsonConverter<ColorData>
    {
        public override ColorData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ColorData.FromArgb(0, 0, 0, 0);

        public override void Write(Utf8JsonWriter writer, ColorData value, JsonSerializerOptions options)
        {
            if (value.A == 0)
                writer.WriteStringValue("#none");
            else
                writer.WriteStringValue(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", value.R, value.G, value.B, value.A));
        }
    }
}
