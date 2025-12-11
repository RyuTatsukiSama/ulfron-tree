using ExtensionSQLite;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DBView : EditorWindow
{
    // use this to make the DB view modulable
    // List<TableData> res = connection.Query<TableData>("SELECT name,sql FROM sqlite_master WHERE name NOT LIKE 'sqlite_%';");

    #region --- Shared Members ---

    public static SQLiteConnection connection;

    int currentTabId = 0;
    string[] tabName = { "Character", "Engaged", "Kinship" };

    #endregion

    #region --- Character Table Members ---

    static List<CharacterDataNew> resultsCharacter;

    Vector2 scrollPosCharacter = new Vector2(600, 0);

    #endregion

    #region --- Engaged Table Members ---

    static List<EngagedData> resultsPartner;

    Vector2 scrollPosEngaged = new Vector2(600, 0);

    #endregion

    #region --- Kinship Table Members ---

    static List<KinshipData> resultsKinship;

    Vector2 scrollPosKinship = new Vector2(600, 0);

    #endregion

    [MenuItem("DataBase/DB View")]
    public static void ShowView()
    {
        GetWindow<DBView>("DB View");
        // connection = SQLiteExtensions.OpenUlfronTable(); // TODO : Fix this conflict
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Execute("CREATE TABLE IF NOT EXISTS engaged(id_wife INTEGER NOT NULL, id_husband INTEGER NOT NULL, PRIMARY KEY (id_wife,id_husband), CONSTRAINT fk_idwife FOREIGN KEY (id_wife) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idhusband FOREIGN KEY (id_husband) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");
        connection.Execute("CREATE TABLE IF NOT EXISTS kinship(id_parent INTEGER NOT NULL, id_child INTEGER NOT NULL, PRIMARY KEY (id_parent,id_child), CONSTRAINT fk_idparent FOREIGN KEY (id_parent) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idchild FOREIGN KEY (id_child) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");

        resultsCharacter = connection.Query<CharacterDataNew>("SELECT * FROM character");
        resultsPartner = connection.Query<EngagedData>("SELECT * FROM engaged");
        resultsKinship = connection.Query<KinshipData>("SELECT * FROM kinship");
    }

    private void OnGUI()
    {
        currentTabId = GUILayout.Toolbar(currentTabId, tabName);

        switch (currentTabId)
        {
            case 0:
                DrawCharacterTable();
                break;
            case 1:
                DrawEngagedTable();
                break;
            case 2:
                DrawKinshipTable();
                break;
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Save DB", GUILayout.Height(25)))
        {
            UpdateDB();
        }

        if (GUILayout.Button("Create Backup", GUILayout.Height(25)))
        {
            CreateBackup();
        }

        if (GUILayout.Button("Restore Backup", GUILayout.Height(25)))
        {
            RestoreBackup();
        }

        GUILayout.EndHorizontal();

    }

    #region --- Utility Methods ---

    void UpdateDB()
    {


        connection.Close();
        connection = SQLiteExtensions.OpenUlfronTable();
    }

    void CreateBackup()
    {
        if (File.Exists(Application.streamingAssetsPath + "/ulfronBackup.db"))
        {
            File.Delete(Application.streamingAssetsPath + "/ulfronBackup.db");
        }

        File.Copy(Application.streamingAssetsPath + "/ulfron.db", Application.streamingAssetsPath + "/ulfronBackup.db");
    }

    void RestoreBackup()
    {
        // Destroy the data base
        connection.DropUlfrontTable();

        // Recreate the data base
        // connection.CreateUlfronTable(); // TODO : Fix this conflict
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


        SQLiteConnection backupConnection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfronBackup.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        #region --- Character Table Restore ---
        List<CharacterDataNew> backupDataCharacter = backupConnection.Query<CharacterDataNew>("SELECT * FROM character");

        foreach (CharacterDataNew character in backupDataCharacter)
        {
            connection.Execute($"INSERT INTO character (id, cName) VALUES ({character.id}, '{character.CName}');");
        }

        resultsCharacter = connection.Query<CharacterDataNew>("SELECT * FROM character");

        #endregion

        #region --- Engaged Table Restore ---

        List<EngagedData> backupPartner = backupConnection.Query<EngagedData>("SELECT * FROM engaged");

        foreach (EngagedData character in backupPartner)
        {
            connection.Execute($"INSERT INTO engaged (id_spouse1, id_spouse2) VALUES ({character.id_spouse2}, '{character.id_spouse1}');");
        }

        resultsPartner = connection.Query<EngagedData>("SELECT * FROM engaged");

        #endregion

        #region --- Kinship Table Restore ---

        List<KinshipData> backupKinship = backupConnection.Query<KinshipData>("SELECT * FROM kinship");

        foreach (KinshipData character in backupKinship)
        {
            connection.Execute($"INSERT INTO kinship (id_parent1, id_parent2, id_child) VALUES ({character.id_parent1}, '{character.id_parent2}', '{character.id_child}');");
        }

        resultsKinship = connection.Query<KinshipData>("SELECT * FROM kinship");

        #endregion
    }

    #endregion

    #region --- Character Table Methods ---
    void DrawCharacterTable()
    {
        GUILayout.Label("Character Table", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUILayout.Label("cName", GUILayout.Width(200));

        GUILayout.EndHorizontal();

        scrollPosCharacter = GUILayout.BeginScrollView(scrollPosCharacter, false, true, GUILayout.Width(660));

        foreach (CharacterDataNew r in resultsCharacter)
        {
            GUILayout.BeginHorizontal();

            r.CName = GUILayout.TextField(r.CName, GUILayout.Width(200));

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeleteCharacter(r);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    void DeleteCharacter(CharacterDataNew character)
    {
        connection.Execute($"DELETE FROM Character WHERE id={character.id}");

        UpdateDB();
    }

    #endregion

    #region --- Engaged Table Methods ---

    void DrawEngagedTable()
    {
        GUILayout.Label("Engaged Table", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Spouse1", GUILayout.Width(200));
        GUILayout.Label("Spouse2", GUILayout.Width(200));

        GUILayout.EndHorizontal();

        scrollPosEngaged = GUILayout.BeginScrollView(scrollPosEngaged, false, true, GUILayout.Width(660));

        foreach (EngagedData r in resultsPartner)
        {
            GUILayout.BeginHorizontal();

            CharacterDataNew spouse1 = resultsCharacter.FirstOrDefault(c => c.id == r.id_spouse1);
            CharacterDataNew spouse2 = resultsCharacter.FirstOrDefault(c => c.id == r.id_spouse2);

            GUILayout.Label(spouse1.CName, GUILayout.Width(200));
            GUILayout.Label(spouse2.CName, GUILayout.Width(200));

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeleteEngaged(r);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    void DeleteEngaged(EngagedData engaged)
    {
        connection.Execute($"DELETE FROM engaged WHERE id_spouse2={engaged.id_spouse2} AND id_spouse1={engaged.id_spouse1}");

        UpdateDB();
    }

    #endregion

    #region --- Kinship Table Methods ---

    void DrawKinshipTable()
    {
        GUILayout.Label("Kinship Table", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Parent1", GUILayout.Width(200));
        GUILayout.Label("Parent2", GUILayout.Width(200));
        GUILayout.Label("Child", GUILayout.Width(200));

        GUILayout.EndHorizontal();

        scrollPosKinship = GUILayout.BeginScrollView(scrollPosKinship, false, true, GUILayout.Width(665));

        foreach (KinshipData r in resultsKinship)
        {
            GUILayout.BeginHorizontal();
            CharacterDataNew parent1 = resultsCharacter.FirstOrDefault(c => c.id == r.id_parent1);
            CharacterDataNew parent2 = resultsCharacter.FirstOrDefault(c => c.id == r.id_parent2);
            CharacterDataNew child = resultsCharacter.FirstOrDefault(c => c.id == r.id_child);
            GUILayout.Label(parent1.CName, GUILayout.Width(200));
            GUILayout.Label(parent2.CName, GUILayout.Width(200));
            GUILayout.Label(child.CName, GUILayout.Width(200));
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeleteKinship(r);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    void DeleteKinship(KinshipData kinship)
    {
        connection.Execute($"DELETE FROM kinship WHERE id_parent1={kinship.id_parent1} AND id_parent2={kinship.id_parent2} AND id_child={kinship.id_child}");
        UpdateDB();
    }

    #endregion
}
