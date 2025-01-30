using SQLite4Unity3d;
using UnityEngine;

public class ConnectionDB : MonoBehaviour
{
    public static SQLiteConnection connection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        string[] ZCChildren = new string[7];
        ZCChildren[0] = "Haru";
        ZCChildren[1] = "Hiro";
        ZCChildren[2] = "Lila";
        ZCChildren[3] = "Lily";
        ZCChildren[4] = "Iris";
        ZCChildren[5] = "Mia";
        ZCChildren[6] = "Loan";
        connection.Query<Results>($"INSERT INTO character (CName, Partner, Children) VALUES ('Zekio', 'Claire', {ZCChildren})");

    }

    [ContextMenu("Create DB")]
    public void Create()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.Create);
        connection.Query<Results>("CREATE TABLE character(CName TEXT, Partner TEXT, Children TEXT);");
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Query<Results>("DROP TABLE IF EXISTS characyer");
        connection.Query<Results>("CREATE TABLE character(CName TEXT, Partner TEXT, Children TEXT);");
    }
}
