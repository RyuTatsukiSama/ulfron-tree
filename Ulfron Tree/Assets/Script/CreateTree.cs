using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class CreateTree : MonoBehaviour
{
    [SerializeField] GameObject prefabCase;
    [SerializeField] GameObject prefabBarHorizontal;
    [SerializeField] GameObject prefabBarVertical;

    Dictionary<int,int> sizeGeneration = new Dictionary<int, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 pos = new Vector2(960, 540);
        GenerateCase("Zekio", pos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCase(string name, Vector2 pos)
    {
        Character newChara = new Character();
        RSaveClass<Character>.Load(newChara, false, name);
        GetGenerationSize(newChara, 0);

        if (newChara.partner != "")
        {
            GameObject newBar = Instantiate(prefabBarHorizontal);
            newBar.transform.SetParent(transform, false);
            newBar.transform.position = pos;
        }

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = newChara.cName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.cName;
        }

        if (newChara.partner != "")
        {
            GeneratePartner(newChara.partner, pos);
            Vector2 foo = pos;
            foo.x -= 200;
            newCase.transform.position = foo;
            if (newChara.children.Length != 0)
            {
                GenerateChildren(newChara.children, pos);
            }
        }
        else
        {
            newCase.transform.position = pos;
        }
    }

    void GeneratePartner(string name, Vector2 pos)
    {
        Character newChara = new Character();
        RSaveClass<Character>.Load(newChara, false, name);

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = newChara.cName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.cName;
        }

        pos.x += 200;
        newCase.transform.position = pos;
    }

    void GenerateChildren(string[] strings, Vector2 pos)
    {
        GameObject newBar = Instantiate(prefabBarVertical);
        newBar.transform.SetParent(transform, false);
        Vector2 newPos = pos;
        newPos.y -= 200;
        newBar.transform.position = newPos;

        GameObject barHorizontal = Instantiate(prefabBarHorizontal);
        ((RectTransform)barHorizontal.transform).sizeDelta = new Vector2(300 * strings.Length, 10);
        barHorizontal.transform.SetParent(transform, false);
        newPos = pos;
        newPos.y -= 400;
        barHorizontal.transform.position = newPos;

        foreach (string s in strings)
        {
            Character newChara = new Character();
            RSaveClass<Character>.Load(newChara, false, s);

            GameObject newCase = Instantiate(prefabCase);
            newCase.transform.SetParent(transform);
            newCase.transform.name = newChara.cName;

            foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.text = newChara.cName;
            }
        }
    }

    int GetGenerationSize(Character parent, int generation)
    {
        int size = 300;

        if (parent.partner != null)
        {
            size += 300;
        }

        if (parent.children != null && parent.children.Length > 2)
        {
            foreach (string child in parent.children)
            {
                Character character = new Character();
                RSaveClass<Character>.Load(character, false, child);
                size += GetGenerationSize(character, generation + 1);
            }
        }
        int foo = 0;
        if (!sizeGeneration.TryGetValue(generation, out foo))
        {
            sizeGeneration[generation] = 0;
        }

        sizeGeneration[generation] += size;
        return size;
    }
}
