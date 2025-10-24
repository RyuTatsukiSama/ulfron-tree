using SQLite4Unity3d;
using UnityEngine;
using ExtensionSQLite;

public class ConnectionDB : MonoBehaviour
{
    public static SQLiteConnection connection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        connection.OpenUlfronTable();
    }

    [ContextMenu("Create DB")]
    public void Create()
    {

        connection = new SQLiteConnection(Application.streamingAssetsPath + "/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.CreateUlfronTable();
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = new SQLiteConnection(Application.streamingAssetsPath + "/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.DropUlfrontTable();
    }
}
