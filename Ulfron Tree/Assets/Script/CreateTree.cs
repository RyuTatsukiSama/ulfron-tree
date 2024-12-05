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
        string path = $"{Application.streamingAssetsPath}/Json/Zekio.json";
        Vector2 pos = new Vector2(960, 540);
        GenerateCase(path, pos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCase(string path, Vector2 pos)
    {
        Character newChara = JsonUtility.FromJson<Character>(File.ReadAllText(path));

        if (newChara.partner != "")
        {
            GameObject newBar = Instantiate(prefabBarHorizontal);
            newBar.transform.SetParent(transform, false);
            newBar.transform.position = pos;
        }

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.name;
        }

        if (newChara.children.Length != 0)
        {
            //GenerateChildren(newChara.children, pos);
        }

        if (newChara.partner != "")
        {
            GeneratePartner(newChara.partner, pos);
            pos.x -= 200;
        }
        newCase.transform.position = pos;
    }

    void GeneratePartner(string name, Vector2 pos)
    {
        string path = $"{Application.streamingAssetsPath}/Json/{name}.json";
        Character newChara = JsonUtility.FromJson<Character>(File.ReadAllText(path));

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.name;
        }

        pos.x += 200;
        newCase.transform.position = pos;
    }

    void GenerateChildren(string[] strings, Vector2 pos)
    {


        foreach (string s in strings)
        {
            string path = $"{Application.streamingAssetsPath}/Json/{s}.json";
            Character newChara = JsonUtility.FromJson<Character>(File.ReadAllText(path));

            GameObject newCase = Instantiate(prefabCase);
            newCase.transform.SetParent(transform);

            foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.text = newChara.name;
            }
        }
    }
}
