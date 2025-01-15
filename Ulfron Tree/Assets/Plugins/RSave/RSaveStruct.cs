using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine;
using RLog;

// Save manager for struct and class who don't heritate MonoBehaviour
public class RSaveStruct<T> where T : struct
{
    /// <summary>
    /// Saves an object as JSON to a file.
    /// </summary>
    /// <typeparam name="T">The type of the object to save.</typeparam>
    /// <param name="data">The object to serialize and save.</param>
    /// <param name="fileName">The file name to save to (default: the name of the struct).</param>
    public static void Save(T data, string fileName = "")
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        WriteSaveFile(json, fileName);
    }

    static void WriteSaveFile(string json, string fileName)
    {
        string name = fileName != "" ? fileName : typeof(T).Name;

        if (!Directory.Exists($"{Application.persistentDataPath}/Saves"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/Saves");
        }

        try
        {
#if UNITY_EDITOR
            using (StreamWriter writer = new StreamWriter($"{Application.dataPath}/Resources/Json/{name}.json", false))
            {
                writer.Write(json);
            }
#else
            using (StreamWriter writer = new StreamWriter($"{Application.persistentDataPath}/Saves/{name}.json", false))
            {
                writer.Write(json);
            }
#endif
        }
        catch (Exception ex)
        {
            RLogger.Log(LogLevel.ERROR, ex.Message);
        }
    }

    /// <summary>
    /// Loads JSON data from a file into the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object to load.</typeparam>
    /// <param name="data">A reference to the object to populate.</param>
    /// <param name="fileName">The file name to load from (default: the name of the struct).</param>
    public static void Load(ref T data, string fileName = "")
    {
        string json = ReadSaveFile(fileName);
        if (json == string.Empty)
        {
            return;
        }
        data = JsonConvert.DeserializeObject<T>(json);
    }

    static string ReadSaveFile(string fileName)
    {
        string name = fileName != "" ? fileName : typeof(T).Name;

        try
        {
            using (StreamReader reader = new StreamReader($"{Application.persistentDataPath}/Saves/{name}.json"))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        catch (Exception ex)
        {
            RLogger.Log(LogLevel.ERROR, ex.Message);
            return string.Empty;
        }
    }
}
