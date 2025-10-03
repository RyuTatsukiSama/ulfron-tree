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

        List<CharacterDataNew> results = connection.Query<CharacterDataNew>("SELECT * FROM character WHERE CName='Zekio';");
        CharacterDB zekio = new CharacterDB
        {
            data = results[0]
        };

        GenerateCase(zekio, pos);
    }

    void GenerateCase(CharacterDB currentChara, Vector2 pos)
    {
        CharacterDataNew partnerChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_husband FROM engaged WHERE id_wife={currentChara.data.id} UNION SELECT id_wife FROM engaged WHERE id_husband={currentChara.data.id});").FirstOrDefault();
        if (partnerChara != null)
        {
            GameObject newBar = Instantiate(prefabBarHorizontal);
            newBar.transform.SetParent(transform, false);
            newBar.transform.position = pos;
        }

        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = currentChara.data.CName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = currentChara.data.CName;
        }

        if (partnerChara != null)
        {
            GeneratePartner(partnerChara, pos);
            Vector2 foo = pos;
            foo.x -= 200;
            newCase.transform.position = foo;
            List<CharacterDataNew> children = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_child FROM kinship WHERE id_parent='{currentChara.data.id}');");
            if (children.Count > 0)
            {
                GenerateChildren(children, pos);
            }
        }
        else
        {
            newCase.transform.position = pos;
        }
    }

    void GeneratePartner(CharacterDataNew partner, Vector2 pos)
    {
        GameObject newCase = Instantiate(prefabCase);
        newCase.transform.SetParent(transform);
        newCase.transform.name = partner.CName;

        foreach (TextMeshProUGUI text in newCase.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = partner.CName;
        }

        pos.x += 200;
        newCase.transform.position = pos;
    }

    void GenerateChildren(List<CharacterDataNew> children, Vector2 pos)
    {
        // Générer la barre reliant lien parent à liens frère et soeur
        GameObject barParentsToSiblings = Instantiate(prefabBarVertical);
        barParentsToSiblings.transform.SetParent(transform, false);
        Vector2 barPos = pos;
        barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y / 2;
        barParentsToSiblings.transform.position = barPos;

        // Génère les enfants
        if (children.Count == 1) // Si il n'y a qu'un enfant
        {
            CharacterDB currentChild = new CharacterDB
            {
                data = children[0]
            };
            barPos = pos;
            barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y;
            GenerateCase(currentChild, barPos);
        }
        else // Si y'en a + d'un
        {
            // Générer la barre reliant les frères et soeurs
            GameObject barSiblings = Instantiate(prefabBarHorizontal);
            ((RectTransform)barSiblings.transform).sizeDelta = new Vector2(GetGenerationSize(children) - 200, 10); // Ici à la place du strings.Length appeler le GetSizeGeneration
            barSiblings.transform.SetParent(transform, false);
            barPos = pos;
            barPos.y -= ((RectTransform)barParentsToSiblings.transform).sizeDelta.y;
            barSiblings.transform.position = barPos;

            Vector2 siblingsPos = barSiblings.transform.position; // Position d'un enfant sur la barre frères et soeurs
            siblingsPos.y += ((RectTransform)barSiblings.transform).sizeDelta.y / 2;
            siblingsPos.x -= ((RectTransform)barSiblings.transform).sizeDelta.x / 2;
            for (int i = 0; i < children.Count; i++)
            {
                CharacterDB currentChild = new CharacterDB
                {
                    data = children[i]
                };

                // Génère sa barre qui le relie à la barre frères et soeurs
                GameObject barSiblingsToCase = Instantiate(prefabBarVertical);
                ((RectTransform)barSiblingsToCase.transform).sizeDelta = new Vector2(10, 200); ;
                barSiblingsToCase.transform.SetParent(transform, false);

                List<CharacterDataNew> granchildren = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_child FROM kinship WHERE id_parent='{children[i].id}');");
                CharacterDataNew partnerChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_husband FROM engaged WHERE id_wife={children[i].id} UNION SELECT id_wife FROM engaged WHERE id_husband={children[i].id});").FirstOrDefault();


                // Décalage du précédent siblings si il n'est pas le premier de la liste
                if (i != 0)
                {
                    if (granchildren.Count > 2)
                    {
                        siblingsPos.x += GetGenerationSize(granchildren) / 2;
                    }
                    else if (partnerChara != null)
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
                if (granchildren.Count > 2)
                {
                    siblingsPos.x += GetGenerationSize(granchildren) / 2;
                }
                else if (partnerChara != null)
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

    int GetGenerationSize(List<CharacterDataNew> children)
    {
        int result = 0;

        for (int i = 0; i < children.Count; i++)
        {
            int tempResult = 300;

            List<CharacterDataNew> granchildren = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_child FROM kinship WHERE id_parent='{children[i].id}');");
            CharacterDataNew partnerChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE id IN (SELECT id_husband FROM engaged WHERE id_wife={children[i].id} UNION SELECT id_wife FROM engaged WHERE id_husband={children[i].id});").FirstOrDefault();

            if (granchildren.Count > 2)
            {
                tempResult = GetGenerationSize(granchildren);
            }
            else if (partnerChara != null)
            {
                tempResult = 700;
            }

            if (i == children.Count - 1 || i == 0)
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
