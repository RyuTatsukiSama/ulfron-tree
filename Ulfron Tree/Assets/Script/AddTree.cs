using UnityEngine;

public class AddTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] ZCChildren = new string[7];
        ZCChildren[0] = "Haru";
        ZCChildren[1] = "Hiro";
        ZCChildren[2] = "Lila";
        ZCChildren[3] = "Lila";
        ZCChildren[4] = "Iris";
        ZCChildren[5] = "Mia";
        ZCChildren[6] = "Loan";
        Character Zekio = new Character("Zekio", "Claire", ZCChildren);

        string[] HEhildren = new string[4];
        HEhildren[0] = "Lim";
        HEhildren[1] = "Emma";
        HEhildren[2] = "Yuko";
        HEhildren[3] = "Torch";
        Character Haru = new Character("Haru", "Elia", HEhildren);


        string[] HCChildren = new string[2];
        HCChildren[0] = "Adam";
        HCChildren[1] = "Burn";
        Character Hiro = new Character("Hiro", "Clara", HCChildren);

        Character Iris = new Character("Iris", "Nétior", null);

        Character Mia = new Character("Mia", "Eto", null);
    }
    
    public void AddToTree()
    {

    }
}
