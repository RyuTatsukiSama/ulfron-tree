using SQLite4Unity3d;
using UnityEngine;

namespace ExtensionSQLite
{
    public static class SQLiteExtensions
    {
        

        public static void CreateUlfronTable(this SQLiteConnection connection)
        {
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
        }

        public static void DropUlfrontTable(this SQLiteConnection connection)
        {
            connection.Execute("DROP TABLE IF EXISTS character");
            connection.Execute("DROP TABLE IF EXISTS engaged");
            connection.Execute("DROP TABLE IF EXISTS kinship");
        }

        public static SQLiteConnection OpenUlfronTable()
        {
            SQLiteConnection connection = new SQLiteConnection(Application.streamingAssetsPath + "/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

            return connection;
        }
    }
}
