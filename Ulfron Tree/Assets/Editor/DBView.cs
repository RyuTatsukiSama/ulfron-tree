using Codice.CM.Common;
using SQLite4Unity3d;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class DBView : EditorWindow
{
    public static SQLiteConnection connection;

    static List<CharacterDB> results;

    [MenuItem("DataBase/View")]
    public static void ShowView()
    {
        GetWindow<DBView>("Ulfron Database");
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Query<CharacterDB>("CREATE TABLE IF NOT EXISTS character(CName TEXT UNIQUE, Partner TEXT, Children TEXT);");

        results = connection.Query<CharacterDB>("SELECT * FROM character");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Name");
        GUILayout.Label("Partner");
        GUILayout.Label("Children");

        GUILayout.EndHorizontal();

        foreach (CharacterDB r in results)
        {
            GUILayout.BeginHorizontal();

            r.CName = GUILayout.TextField(r.CName);
            r.Partner = GUILayout.TextField(r.Partner);
            r.Children = GUILayout.TextField(r.Children);

            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
            connection.Query<CharacterDB>("INSERT INTO character(CName,Partner,Children) VALUES ('','','')");
            results = connection.Query<CharacterDB>("SELECT * FROM character");
        }

        if (GUILayout.Button("Update DB"))
        {
            foreach (CharacterDB r in results)
                connection.Query<CharacterDB>($"INSERT OR REPLACE INTO character (id,CName,Partner,Children) VALUES ({r.id},'{r.CName}','{r.Partner}','{r.Children}')");
        }

        GUILayout.EndHorizontal();
    }

    public void Create()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.Create);
        connection.Query<CharacterDB>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT, Partner TEXT, Children TEXT);");
    }

    public void DropTable()
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Query<CharacterDB>("DROP TABLE IF EXISTS character");
        connection.Query<CharacterDB>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT, Partner TEXT, Children TEXT);");
    }
}
