using UnityEngine;
using System.IO;

public class Character
{
    public string name;
    public string partner;
    public string[] children;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Character(string _name, string _partner, string[] _children)
    {
        name = _name;
        partner = _partner;
        if (!File.Exists($"{Application.dataPath}/Json/{partner}.json"))
        {
            Character newChara = new Character(partner, name, _children, true);
        }
        children = _children;
        foreach (string child in children)
        {
            if (!File.Exists($"{Application.dataPath}/Json/{child}.json"))
            {
                Character newChara = new Character(child, true);
            }
        }
        string path = $"{Application.dataPath}/Json/{name}.json";
        File.WriteAllText(path, JsonUtility.ToJson(this,true));
    }

    Character(string _name, string _partner, string[] _children, bool isCreatedByPartner)
    {
        name = _name;
        partner = _partner;
        children = _children;
        string path = $"{Application.dataPath}/Json/{name}.json";
        File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }

    Character(string _name, bool isCreatedByParent)
    {
        name = _name;
        partner = null;
        children = null;
        string path = $"{Application.dataPath}/Json/{name}.json";
        File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }
}
