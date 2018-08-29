namespace NPNDAutoVNC
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    internal class XmlSerializationHelper
    {
        public static T Deserialize<T>(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(filename))
            {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static void Serialize<T>(string filename, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize((TextWriter) writer, obj);
            }
        }
    }
}

