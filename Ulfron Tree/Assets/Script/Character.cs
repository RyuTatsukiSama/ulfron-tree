using UnityEngine;
using System.IO;

public class Character
{
    public string cName;
    public string partner;
    public string[] children;

    public Character()
    {
        cName = null;
        partner = null;
        children = null;
    }

    public Character(string _name, string _partner, string[] _children)
    {
        cName = _name;
        partner = _partner;
        bool exist;
#if UNITY_EDITOR
        exist = File.Exists($"{Application.dataPath}/Resources/Json/{partner}.json");
#else
            exist = File.Exists($"{Application.persistentDataPath}/Saves/{partner}.json");
#endif
        if (!exist)
        {
            Character newChara = new Character(partner, cName, _children, true);
        }
        children = _children;
        foreach (string child in children)
        {
#if UNITY_EDITOR
            exist = File.Exists($"{Application.dataPath}/Resources/Json/{child}.json");
#else
            exist = File.Exists($"{Application.persistentDataPath}/Saves/{child}.json");
#endif
            if (!exist)
            {
                Character newChara = new Character(child, true);
            }
        }
        RSaveClass<Character>.Save(this, false, cName);
    }

    Character(string _name, string _partner, string[] _children, bool isCreatedByPartner)
    {
        cName = _name;
        partner = _partner;
        children = _children;
        RSaveClass<Character>.Save(this, false, cName);
    }

    Character(string _name, bool isCreatedByParent)
    {
        cName = _name;
        partner = null;
        children = null;
        RSaveClass<Character>.Save(this, false, cName);
    }
}
