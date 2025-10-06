using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DBView : EditorWindow
{
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

    #endregion

    #region --- Kinship Table Members ---

    static List<KinshipData> resultsKinship;

    #endregion

    [MenuItem("DataBase/DB View")]
    public static void ShowView()
    {
        GetWindow<DBView>("DB View");
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
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    void CreateBackup()
    {
        if (File.Exists(Application.dataPath + "/StreamingAssets/ulfronBackup.db"))
        {
            File.Delete(Application.dataPath + "/StreamingAssets/ulfronBackup.db");
        }

        File.Copy(Application.dataPath + "/StreamingAssets/ulfron.db", Application.dataPath + "/StreamingAssets/ulfronBackup.db");
    }

    void RestoreBackup()
    {
        // Destroy the data base
        connection.Execute("DROP TABLE IF EXISTS character");
        connection.Execute("DROP TABLE IF EXISTS engaged");
        connection.Execute("DROP TABLE IF EXISTS kinship");

        // Recreate the data base
        connection.Execute("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT UNIQUE);");
        connection.Execute("CREATE TABLE IF NOT EXISTS engaged(id_wife INTEGER NOT NULL, id_husband INTEGER NOT NULL, PRIMARY KEY (id_wife,id_husband), CONSTRAINT fk_idwife FOREIGN KEY (id_wife) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idhusband FOREIGN KEY (id_husband) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");
        connection.Execute("CREATE TABLE IF NOT EXISTS kinship(id_parent INTEGER NOT NULL, id_child INTEGER NOT NULL, PRIMARY KEY (id_parent,id_child), CONSTRAINT fk_idparent FOREIGN KEY (id_parent) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE, CONSTRAINT fk_idchild FOREIGN KEY (id_child) REFERENCES character(id) ON UPDATE CASCADE ON DELETE CASCADE);");

        SQLiteConnection backupConnection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfronBackup.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        #region --- Character Table Restore ---
        List<CharacterDataNew> backupDataCharacter = backupConnection.Query<CharacterDataNew>("SELECT * FROM character");

        foreach (CharacterDataNew character in backupDataCharacter)
        {
            connection.Execute($"INSERT INTO character (id, cName) VALUES ({character.id}, '{character.CName}');");
        }

        resultsCharacter = connection.Query<CharacterDataNew>("SELECT * FROM character");

        #endregion

        #region --- Kinship Table Restore ---

        List<EngagedData> backupPartner = connection.Query<EngagedData>("SELECT * FROM partner");

        foreach (EngagedData character in backupPartner)
        {
            connection.Execute($"INSERT INTO engaged (id_wife, id_husband) VALUES ({character.id_wife}, '{character.id_husband}');");
        }

        resultsPartner = connection.Query<EngagedData>("SELECT * FROM partner");

        #endregion

        #region --- Engaged Table Restore ---

        List<KinshipData> backupKinship = connection.Query<KinshipData>("SELECT * FROM kinship");

        foreach (KinshipData character in backupKinship)
        {
            connection.Execute($"INSERT INTO kinship (id_parent, id_child) VALUES ({character.id_parent}, '{character.id_child}');");
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
        GUILayout.Label("Partner", GUILayout.Width(200));
        GUILayout.Label("Children", GUILayout.Width(200));

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
    }

    #endregion

    #region --- Kinship Table Methods ---

    void DrawKinshipTable()
    {
        GUILayout.Label("Kinship Table", EditorStyles.boldLabel);
    }

    #endregion
}
