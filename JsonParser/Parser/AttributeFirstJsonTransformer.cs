using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace JsonParser.Parser
{
    public static class AttributeFirstJsonTransformer
    {
        public static Stream Transform(Stream source)
        {
            string text = new StreamReader(source).ReadToEnd();
            if (!IsValidJObject(text, out JObject jObject))
                return ConvertFromStringToStream("It is not valid Json!");

            var childeren = GetOrderedJsonChilderen(jObject.Children<JProperty>().ToList());

            jObject = new JObject();
            foreach (var item in childeren)
                jObject.Add(item);

            source = ConvertFromStringToStream(JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented));
            return source;
        }

        private static bool IsValidJObject(string text, out JObject jObject)
        {
            bool isValid;

            try
            {
                jObject = JObject.Parse(text);
                isValid = true;
            }
            catch
            {
                jObject = null;
                isValid = false;
            }

            return isValid;

        }

        private static Stream ConvertFromStringToStream(string str)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream: stream, encoding: Encoding.UTF8, bufferSize: 4096, leaveOpen: true)) // last parameter is important
            {
                writer.Write(str);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }

        private static List<JProperty> GetOrderedJsonChilderen(List<JProperty> properties)
        {
            properties = properties.OrderBy(x => GetOrderedChild(x.Value)).ToList();

            for (var i = 0; i <= properties.Count - 1; i++)
            {
                if (properties[i].Value.Type == JTokenType.Object)
                {
                    var _childeren = GetOrderedJsonChilderen(properties[i].Value.Children<JProperty>().ToList());

                    JObject jsonObject = new JObject(_childeren.ToArray());
                    properties[i] = new JProperty(properties[i].Name, jsonObject);
                }

                if (properties[i].Value.Type == JTokenType.Array)
                {
                    JArray jsonArray = new JArray();
                    foreach (var token in (JToken)properties[i].Value)
                    {
                        var childrenOfArray = token.Children<JProperty>().ToList();
                        if (childrenOfArray.Any())
                        {
                            var orderedChildrenOfArray = GetOrderedJsonChilderen(childrenOfArray);
                            JObject jsonObject = new JObject(orderedChildrenOfArray.ToArray());
                            jsonArray.Add(jsonObject);
                        }
                    }

                    if (jsonArray.Any())
                        properties[i] = new JProperty(properties[i].Name, jsonArray);

                }

            }

            return properties;
        }

        private static int GetOrderedChild(JToken val)
        {
            int toReturn;
            switch (val.Type)
            {
                case JTokenType.String:
                    toReturn = 0;
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                    toReturn = 1;
                    break;

                case JTokenType.Boolean:
                    toReturn = 2;
                    break;

                case JTokenType.Object:
                    toReturn = 3;
                    break;

                case JTokenType.Array:
                    toReturn = 4;
                    break;

                default:
                    toReturn = 100;
                    break;
            }

            return toReturn;
        }

    }
}