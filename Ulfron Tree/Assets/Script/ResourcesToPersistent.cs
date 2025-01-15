using System.IO;
using System.Linq;
using UnityEngine;

public class ResourcesToPersistent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        TextAsset[] textAssets = Resources.LoadAll("Json").OfType<TextAsset>().ToArray();

        if (!Directory.Exists($"{Application.persistentDataPath}/Saves"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/Saves");

            foreach (TextAsset asset in textAssets)
            {
                using (StreamWriter writer = new StreamWriter($"{Application.persistentDataPath}/Saves/{asset.name}.json", false))
                {
                    writer.Write(asset.text);
                }
            }
        }
    }
}
