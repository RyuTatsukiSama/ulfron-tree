using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine.InputSystem.HID;
using static ConnectionDB;
using System.Linq;
using UnityEngine.UIElements;

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

        List<CharacterData> results = connection.Query<CharacterData>("SELECT * FROM character WHERE CName='Zekio';");
        CharacterDB zekio = new CharacterDB
        {
            data = results[0]
        };

        GenerateCase(zekio, pos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCase(CharacterDB newChara, Vector2 pos)
    {
        if (newChara.data.Partner != "")
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

        if (newChara.data.Partner != "")
        {
            GeneratePartner(newChara.data.Partner, pos);
            Vector2 foo = pos;
            foo.x -= 200;
            newCase.transform.position = foo;
            if (newChara.data.Children != "")
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
        List<CharacterData> results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{name}';");
        CharacterDB partner = new CharacterDB
        {
            data = results[0]
        };

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = partner.data.CName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = partner.data.CName;
        }

        pos.x += 200;
        newCase.transform.position = pos;
    }

    void GenerateChildren(string[] childrenNames, Vector2 pos)
    {
        // Générer la barre reliant lien parent à liens frère et soeur
        GameObject barParentsToSiblings = Instantiate(prefabBarVertical);
        barParentsToSiblings.transform.SetParent(transform, false);
        Vector2 barPos = pos;
        barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y / 2;
        barParentsToSiblings.transform.position = barPos;

        // Génère les enfants
        if (childrenNames.Length == 1) // Si il n'y a qu'un enfant
        {
            List<CharacterData> results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{childrenNames[0]}';");
            CharacterDB currentChild = new CharacterDB
            {
                data = results[0]
            };
            barPos = pos;
            barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y;
            GenerateCase(currentChild, barPos);
        }
        else // Si y'en a + d'un
        {
            // Générer la barre reliant les frères et soeurs
            GameObject barSiblings = Instantiate(prefabBarHorizontal);
            ((RectTransform)barSiblings.transform).sizeDelta = new Vector2(GetGenerationSize(childrenNames) - 200, 10); // Ici à la place du strings.Length appeler le GetSizeGeneration
            barSiblings.transform.SetParent(transform, false);
            barPos = pos;
            barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y;
            barSiblings.transform.position = barPos;

            Vector2 siblingsPos = barSiblings.transform.position; // Poisition d'un enfant sur la barre frères et soeurs
            siblingsPos.y += ((RectTransform)barSiblings.transform).sizeDelta.y / 2;
            siblingsPos.x -= ((RectTransform)barSiblings.transform).sizeDelta.x / 2;
            for (int i = 0; i < childrenNames.Length; i++)
            {
                List<CharacterData> results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{childrenNames[i]}';");
                CharacterDB currentChild = new CharacterDB
                {
                    data = results[0]
                };

                // Génère sa barre qui le relie à la barre frères et soeurs
                GameObject barSiblingsToCase = Instantiate(prefabBarVertical);
                ((RectTransform)barSiblingsToCase.transform).sizeDelta = new Vector2(10, 200); ;
                barSiblingsToCase.transform.SetParent(transform, false);

                // Décalage du précédent siblings si il n'est pas le premier de la liste
                if (i != 0)
                {
                    if (currentChild.data.Children != "" && currentChild.data.Children.Split("_").Length > 2)
                    {
                        siblingsPos.x += GetGenerationSize(currentChild.data.Children.Split("_")) / 2;
                    }
                    else if (currentChild.data.Partner != "")
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
                GenerateCase(currentChild, tempPos);

                // Décale pour le prochains siblings
                if (currentChild.data.Children != "" && currentChild.data.Children.Split("_").Length > 2)
                {
                    siblingsPos.x += GetGenerationSize(currentChild.data.Children.Split("_")) / 2;
                }
                else if (currentChild.data.Partner != "")
                {
                    siblingsPos.x += 350;
                }
                else
                {
                    siblingsPos.x += 150;
                }
            }
        }
    }

    int GetGenerationSize(string[] children)
    {
        int result = 0;

        for (int i = 0; i < children.Length; i++)
        {
            int tempResult = 300;

            List<CharacterData> results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{children[i]}';");
            CharacterDB child = new CharacterDB
            {
                data = results[0]
            };

            if (child.data.Children != "" && child.data.Children.Split("_").Length > 2)
            {
                tempResult = GetGenerationSize(child.data.Children.Split("_"));
            }
            else if (child.data.Partner != "")
            {
                tempResult = 700;
            }

            if (i == children.Length - 1 || i == 0)
            {
                result += 100;
                result += tempResult / 2;
            }
            else
            {
                result += tempResult;
            }
        }

        return result;
    }
}
