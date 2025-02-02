using UnityEngine;

public class AddTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string ZCChildren = "Haru_Hiro_Lila_Lily_Iris_Mia_Loan";
        CharacterDB Zekio = new CharacterDB("Zekio", "Claire", ZCChildren);

        string HEhildren = "Lim_Emma_Yuko_Torch";
        CharacterDB Haru = new CharacterDB("Haru", "Elia", HEhildren);

        string HCChildren = "Adam_Burn";
        CharacterDB Hiro = new CharacterDB("Hiro", "Clara", HCChildren);

        string INChildren = "Hélio";
        CharacterDB Iris = new CharacterDB("Iris", "Nétior", INChildren);

        CharacterDB Mia = new CharacterDB("Mia", "Eto", null);
    }
    
    public void AddToTree()
    {

    }
}
