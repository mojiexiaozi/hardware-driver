using Newtonsoft.Json;
using System;
using System.IO;

namespace HardwareDriver.Common
{
    public static class JsonManager
    {
        public static void SerializeToFile(object o, string filename)
        {
            var json = JsonConvert.SerializeObject(o, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
        public static object UnSerialzeFromFile(Type type, string filename)
        {
            try
            {
                var json = File.ReadAllText(filename);
                var res = JsonConvert.DeserializeObject(
                    json,
                    type, 
                    new JsonSerializerSettings { Formatting = Formatting.Indented });
                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}
