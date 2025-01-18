using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

public class IgnoreBaseClassPropertiesResolver<TBase> : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        // Exclude properties declared in the base class and problematic Unity properties
        return base.CreateProperties(type, memberSerialization)
                   .Where(p => p.DeclaringType != typeof(TBase) && !IsProblematicUnityProperty(p))
                   .ToList();
    }

    private bool IsProblematicUnityProperty(JsonProperty property)
    {
        // List of Unity-specific or deprecated properties to exclude
        var problematicProperties = new HashSet<string>
        {
            "gameObject", "transform", "enabled", "isActiveAndEnabled", "tag", "name", "hideFlags",
            "rigidbody", "rigidbody2D", "renderer", "camera", "light", "animation", "constantForce", "audio", "guiText", "guiElement", "networkView", "collider", "collider2D", "hingeJoint", "particleSystem"
        };
        return problematicProperties.Contains(property.PropertyName);
    }
}

// Save Manager Class for class
public static class RSaveClass<T> where T : class
{
    /// <summary>
    /// Saves an object as JSON to a file, optionally excluding parent class fields.
    /// </summary>
    /// <typeparam name="T">The type of the object to save.</typeparam>
    /// <param name="data">The object to serialize and save.</param>
    /// <param name="ignoreParent">Whether to exclude fields from parent classes (default: true).</param>
    /// <param name="fileName">The file name to save to (default: the name of the class).</param>
    public static void Save(T data, bool ignoreParent = true, string fileName = "")
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        if (ignoreParent) // If you want to ignore the field from the mother class
            settings.ContractResolver = new IgnoreBaseClassPropertiesResolver<MonoBehaviour>();

        string json = JsonConvert.SerializeObject(data, settings);
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
    /// Loads JSON data from a file and updates the fields of the target object.
    /// Optionally includes fields from parent classes.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="data">The object to update with loaded data.</param>
    /// <param name="ignoreParent">Whether to ignore fields from parent classes (default: true).</param>
    /// <param name="fileName">The file name to load from (default: the name of the class).</param>
    public static void Load(T data, bool ignoreParent = true, string fileName = "")
    {
        string json = ReadSaveFile(fileName);
        if (json == string.Empty)
        {
            return;
        }
        T saveData = JsonConvert.DeserializeObject<T>(json);

        List<System.Reflection.FieldInfo> targetFields = new();
        List<System.Reflection.FieldInfo> sourceFields = new();

        if (ignoreParent) // If you want to ignore the field from the mother class
        {
            targetFields = data.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic).ToList();
            sourceFields = saveData.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic).ToList();
        }
        else
        {
            Type currentDataType = data.GetType();
            Type currentSaveDataType = data.GetType();
            while (currentSaveDataType != null && currentDataType != null)
            {
                targetFields.AddRange(currentDataType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic));
                sourceFields.AddRange(currentSaveDataType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic));
                currentDataType = currentDataType.BaseType;
                currentSaveDataType = currentSaveDataType.BaseType;
            }
        }


        foreach (var targetProperty in targetFields)
        {
            // Find the matching property in the source object
            var sourceProperty = Array.Find(sourceFields.ToArray(), p => p.Name == targetProperty.Name);
            if (sourceProperty != null)
            {
                // Copy the value from the source property to the target property
                var value = sourceProperty.GetValue(saveData);
                targetProperty.SetValue(data, value);
            }
        }
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
