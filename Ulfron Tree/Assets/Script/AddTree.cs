using UnityEngine;

public class AddTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        string ZCChildren = "Haru_Hiro_Lila_Lily_Iris_Mia_Loan";
        Character Zekio = new Character("Zekio", "Claire", ZCChildren);

        string HEhildren = "Lim_Emma_Yuko_Torch";
        Character Haru = new Character("Haru", "Elia", HEhildren);

        string HCChildren = "Adam_Burn";
        Character Hiro = new Character("Hiro", "Clara", HCChildren);

        Character Iris = new Character("Iris", "Nétior", null);

        Character Mia = new Character("Mia", "Eto", null);
    }
    
    public void AddToTree()
    {

    }
}
