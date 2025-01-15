using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class CreateTree : MonoBehaviour
{
    [SerializeField] GameObject prefabCase;
    [SerializeField] GameObject prefabBarHorizontal;
    [SerializeField] GameObject prefabBarVertical;

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
        Debug.Log(pos);

        if (newChara.children.Length != 0)
        {
            GenerateChildren(newChara.children, pos);
        }

        Debug.Log(pos);

        if (newChara.partner != "")
        {
            GeneratePartner(newChara.partner, pos);
        }
        Debug.Log(pos);
        newCase.transform.position = pos;
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
        Vector2 posVer = pos;
        posVer.y += 200;
        newBar.transform.position = pos;

        pos.y += 400;
        GameObject barHorizontal = Instantiate(prefabBarHorizontal);
        ((RectTransform)barHorizontal.transform).sizeDelta = new Vector2(300 * strings.Length, 10);
        transform.position = pos;

        foreach (string s in strings)
        {
            string path = $"{Application.persistentDataPath}/Saves/{s}.json";
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
}
