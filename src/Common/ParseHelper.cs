using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace CommonDotNetHelpers.Common
{
    public static class ParseHelper
    {

        //TODO: Not tested nor refered in solution either. Commenting below code
        //public static T ParseXmlFromPath<T>(string xmlFilePath) where T : class
        //{
        //    string xml = File.ReadAllText(xmlFilePath);
        //    var reader = XmlReader.Create(xmlFilePath, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });

        //    return new XmlSerializer(typeof(T)).Deserialize(reader) as T;

        //}

        
        /// <summary>
        ///  Extention method for Stream
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Stream ToStream(this string xmlString)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xmlString);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        /// <summary>
        /// http://stackoverflow.com/questions/894263/how-do-i-identify-if-a-string-is-a-number/894325#894325
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(this object expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
#if NET461
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T ParseXml<T>(this string xmlString) where T : class
        {
            var reader = XmlReader.Create(xmlString.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }
#endif // NET461
#if INCLUDE_NOT_COVERED_BY_TESTS
        //TODO: Not tested nor refered in solution either. Commenting below code
        public static T ParseJSON<T>(this string @this) where T : class
        {
            return JsonParser.Deserialize<T>(@this.Trim());
        }
#endif
    }
}
