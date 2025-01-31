using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine.InputSystem.HID;

public class CreateTree : MonoBehaviour
{
    [SerializeField] GameObject prefabCase;
    [SerializeField] GameObject prefabBarHorizontal;
    [SerializeField] GameObject prefabBarVertical;

    Dictionary<int, int> sizeGeneration = new Dictionary<int, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 pos = new Vector2(960, 540);
        CharacterDB zekio = new CharacterDB();
        RSaveClass<CharacterDB>.Load(zekio, false, "Zekio");
        GenerateCase(zekio, pos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCase(CharacterDB newChara, Vector2 pos)
    {
        if (newChara.data.Partner != null)
        {
            GameObject newBar = Instantiate(prefabBarHorizontal);
            newBar.transform.SetParent(transform, false);
            newBar.transform.position = pos;
        }

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = newChara.data.CName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.data.CName;
        }

        if (newChara.data.Partner != null)
        {
            GeneratePartner(newChara.data.Partner, pos);
            Vector2 foo = pos;
            foo.x -= 200;
            newCase.transform.position = foo;
            if (newChara.data.Children != null)
            {
                GenerateChildren(newChara.data.Children.Split("_"), pos);
            }
        }
        else
        {
            newCase.transform.position = pos;
        }
    }

    void GeneratePartner(string name, Vector2 pos)
    {
        CharacterDB newChara = new CharacterDB();
        RSaveClass<CharacterDB>.Load(newChara, false, name);

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = newChara.data.CName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = newChara.data.CName;
        }

        pos.x += 200;
        newCase.transform.position = pos;
    }

    void GenerateChildren(string[] strings, Vector2 pos)
    {
        // Générer la barre reliant lien parent à liens frère et soeur
        GameObject barParentsToSiblings = Instantiate(prefabBarVertical);
        barParentsToSiblings.transform.SetParent(transform, false);
        Vector2 barPos = pos;
        barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y / 2;
        barParentsToSiblings.transform.position = barPos;

        // Générer la barre reliant les frères et soeurs
        GameObject barSiblings = Instantiate(prefabBarHorizontal);
        ((RectTransform)barSiblings.transform).sizeDelta = new Vector2(GetGenerationSize(strings)-200, 10); // Ici à la place du strings.Length appeler le GetSizeGeneration
        barSiblings.transform.SetParent(transform, false);
        barPos = pos;
        barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y;
        barSiblings.transform.position = barPos;

        // Génère les enfants
        Vector2 siblingsPos = barSiblings.transform.position; // Poisition d'un enfant sur la barre frères et soeurs
        siblingsPos.y += ((RectTransform)barSiblings.transform).sizeDelta.y / 2;
        siblingsPos.x -= ((RectTransform)barSiblings.transform).sizeDelta.x / 2;
        for (int i = 0; i < strings.Length;i++)
        {
            CharacterDB newChara = new CharacterDB();
            RSaveClass<CharacterDB>.Load(newChara, false, strings[i]);

            // Génère sa barre qui le relie à la barre frères et soeurs
            GameObject barSiblingsToCase = Instantiate(prefabBarVertical);
            ((RectTransform)barSiblingsToCase.transform).sizeDelta = new Vector2(10, 200); ;
            barSiblingsToCase.transform.SetParent(transform, false);
            // Décalage du précédent siblings si il n'est pas le premier de la liste
            if (i != 0)
            {
                if (newChara.data.Children != null && newChara.data.Children.Length > 2)
                {
                    siblingsPos.x += GetGenerationSize(newChara.data.Children.Split("_")) / 2;
                }
                else if (newChara.data.Partner != null)
                {
                    siblingsPos.x += 350;
                }
                else
                {
                    siblingsPos.x += 150;
                }
            }

            Vector2 tempPos = siblingsPos;
            // Put the position
            tempPos.y -= ((RectTransform)barSiblingsToCase.transform).sizeDelta.y / 2;
            barSiblingsToCase.transform.position = tempPos;

            // Génère sa case
            tempPos.y -= ((RectTransform)barSiblingsToCase.transform).sizeDelta.y / 2;
            GenerateCase(newChara, tempPos);

            // Décale pour le prochains siblings
            if (newChara.data.Children != null && newChara.data.Children.Length > 2)
            {
                siblingsPos.x += GetGenerationSize(newChara.data.Children.Split("_"))/2;
            }
            else if (newChara.data.Partner != null)
            {
                siblingsPos.x += 350;
            }
            else
            {
                siblingsPos.x += 150;
            }
        }
    }

    int GetGenerationSize(string[] children)
    {
        int result = 0;

        for (int i = 0; i < children.Length;i++)
        {
            int tempResult = 300;
            CharacterDB child = new CharacterDB();
            RSaveClass<CharacterDB>.Load(child, false, children[i]);

            if (child.data.Children != null && child.data.Children.Length > 2)
            {
                tempResult = GetGenerationSize(child.data.Children.Split("_"));
            }
            else if (child.data.Partner != null)
            {
                tempResult = 700;
            }

            if (i == children.Length - 1 || i == 0)
            {
                result += 100;
                result += tempResult/2;
            }
            else
            {
                result += tempResult;
            }
        }

        return result;
    }
}
