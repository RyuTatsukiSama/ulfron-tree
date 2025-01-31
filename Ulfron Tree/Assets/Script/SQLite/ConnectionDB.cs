using SQLite4Unity3d;
using UnityEngine;

public class ConnectionDB : MonoBehaviour
{
    public static SQLiteConnection connection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    [ContextMenu("Create DB")]
    public void Create()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.Create);
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT, Partner TEXT, Children TEXT);");
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Query<CharacterData>("DROP TABLE IF EXISTS character");
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT, Partner TEXT, Children TEXT);");
    }
}
