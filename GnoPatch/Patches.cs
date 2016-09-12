using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GnoPatch
{
    public class Patches
    {

        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
            }
        };

        public static PatchGroup Load(string filename)
        {
            if (!File.Exists(filename)) throw new Exception("Can't find file " + filename);
            var text = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<PatchGroup>(text, _settings);
        }

        internal static void Save(PatchGroup tests)
        {
            var filename = "patches.json";
            if (File.Exists(filename)) throw new Exception("Won't overwrite existing file.");
            var text = JsonConvert.SerializeObject(tests, _settings);
            File.WriteAllText(filename, text);
        }
    }
}