using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public static class ItemLoader
{
    public class JsonEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, float> Parameters { get; set; }

        public override string ToString()
        {
            return $"[{ID}] name = {Name}, type = {Type} ({Parameters.Count} attributes)";
        }
    }
    
    private static List<JsonEntry> _jsonEntries;
    
    public static List<JsonEntry> Entries { get => _jsonEntries; }
    
    public static void Load(string path)
    {
        string json = File.ReadAllText(path);
        _jsonEntries = JsonConvert.DeserializeObject<List<JsonEntry>>(json);
    }
}
