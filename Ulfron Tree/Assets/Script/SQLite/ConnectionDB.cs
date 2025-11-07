using SQLite4Unity3d;
using UnityEngine;
using ExtensionSQLite;

public class ConnectionDB : MonoBehaviour
{
    public static SQLiteConnection connection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        connection = SQLiteExtensions.OpenUlfronTable();
    }

    [ContextMenu("Create DB")]
    public void Create()
    {

        connection = SQLiteExtensions.OpenUlfronTable();
        connection.CreateUlfronTable();
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = SQLiteExtensions.OpenUlfronTable();
        connection.DropUlfrontTable();
    }
}
