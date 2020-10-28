using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventors.ECP
{
    public static class XmlExtensions
    {
        public static T ToObject<T>(this string self)
            where T : class
        {
            using (var text = new StringReader(self))
            {
                using (var reader = XmlReader.Create(text))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(reader) as T;
                }
            }
        }

        public static string ToXml<T>(this T x)
            where T : class
        {
            using (TextWriter writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, x);
                return writer.ToString();
            }
        }
    }
}
