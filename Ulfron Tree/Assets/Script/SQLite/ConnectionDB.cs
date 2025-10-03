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
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Execute("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT UNIQUE);");
        connection.Execute("CREATE TABLE IF NOT EXISTS engaged(id_wife INTEGER NOT NULL, id_husband INTEGER NOT NULL, PRIMARY KEY (id_wife,id_husband), CONSTRAINT fk_idwife FOREIGN KEY (id_wife) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idhusband FOREIGN KEY (id_husband) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");
        connection.Execute("CREATE TABLE IF NOT EXISTS kinship(id_parent INTEGER NOT NULL, id_child INTEGER NOT NULL, PRIMARY KEY (id_parent,id_child), CONSTRAINT fk_idparent FOREIGN KEY (id_parent) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idchild FOREIGN KEY (id_child) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");
        
        connection.Close();
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Execute("DROP TABLE IF EXISTS character");
        connection.Execute("DROP TABLE IF EXISTS engaged");
        connection.Execute("DROP TABLE IF EXISTS kinship");

        connection.Close();
    }
}
