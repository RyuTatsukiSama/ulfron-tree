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
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        connection.Execute("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT UNIQUE);");

        connection.Execute("CREATE TABLE IF NOT EXISTS engaged" +
            "(id_spouse1 INTEGER NOT NULL, id_spouse2 INTEGER NOT NULL, " +
            "PRIMARY KEY (id_spouse1,id_spouse2), " +
            "CONSTRAINT fk_idspouse1 FOREIGN KEY (id_spouse1) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, " +
            "CONSTRAINT fk_idspouse2 FOREIGN KEY (id_spouse2) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");

        connection.Execute("CREATE TABLE IF NOT EXISTS kinship" +
            "(id_parent1 INTEGER NOT NULL,id_parent2 INTEGER NOT NULL, id_child INTEGER NOT NULL," +
            " PRIMARY KEY (id_parent1, id_parent2,id_child), " +
            "CONSTRAINT fk_idparent1 FOREIGN KEY (id_parent1) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, " +
            "CONSTRAINT fk_idparent2 FOREIGN KEY (id_parent2) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, " +
            "CONSTRAINT fk_idchild FOREIGN KEY (id_child) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");
        // connection = SQLiteExtensions.OpenUlfronTable();
        // connection.CreateUlfronTable(); // TODO : Fix this conflict
        connection.Close();
    }

    [ContextMenu("Drop")]
    public void DropTable()
    {
        connection = SQLiteExtensions.OpenUlfronTable();
        connection.DropUlfrontTable();
    }
}
