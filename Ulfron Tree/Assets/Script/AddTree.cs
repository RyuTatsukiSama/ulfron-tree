using UnityEngine;

public class AddTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] strings = new string[2];
        strings[0] = "Haru";
        strings[1] = "Hiro";
        Character Zekio = new Character("Zekio", "Claire", strings);

        strings[0] = "Adam";
        strings[1] = "Burn";
        Character Haru = new Character("Hiro", "Clara", strings);
    }
    
    public void AddToTree()
    {

    }
}
