using Codice.CM.Common;
using SQLite4Unity3d;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class DBView : EditorWindow
{
    public static SQLiteConnection connection;

    static List<CharacterData> results;

    Vector2 scrollPos = new Vector2(600, 0);

    [MenuItem("DataBase/View")]
    public static void ShowView()
    {
        GetWindow<DBView>("Ulfron Database");
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(CName TEXT UNIQUE, Partner TEXT, Children TEXT);");

        results = connection.Query<CharacterData>("SELECT * FROM character");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Name", GUILayout.Width(200));
        GUILayout.Label("Partner", GUILayout.Width(200));
        GUILayout.Label("Children", GUILayout.Width(200));

        GUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(700));

        foreach (CharacterData r in results)
        {
            GUILayout.BeginHorizontal();

            r.CName = GUILayout.TextField(r.CName, GUILayout.Width(200));
            r.Partner = GUILayout.TextField(r.Partner, GUILayout.Width(200));
            r.Children = GUILayout.TextArea(r.Children, GUILayout.Width(200));

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add", GUILayout.Width(300)))
        {
            connection.Query<CharacterData>("INSERT INTO character(CName,Partner,Children) VALUES ('','','')");
            results = connection.Query<CharacterData>("SELECT * FROM character");
        }

        if (GUILayout.Button("Update DB", GUILayout.Width(300)))
        {
            List<CharacterData> losresults = connection.Query<CharacterData>($"SELECT * FROM character");

            for (int i = 0; i < results.Count; i++)
            {
                if (!results[i].Equals(losresults[i]))
                {
                    if (results[i].Children != losresults[i].Children)
                    {
                        List<CharacterData> partner = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{results[i].Partner}'");
                        connection.Query<CharacterData>($"UPDATE character SET Children = '{results[i].Children}' WHERE id = {partner[0].id}");

                        foreach (string child in results[i].Children.Split("_"))
                        {
                            if (connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{child}'").Count == 0)
                                connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children) VALUES ('{child}','','')");
                        }
                    }

                    connection.Query<CharacterData>($"INSERT OR REPLACE INTO character (id,CName,Partner,Children) VALUES ({results[i].id},'{results[i].CName}','{results[i].Partner}','{results[i].Children}')");
                }
            }

            results = connection.Query<CharacterData>("SELECT * FROM character");
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh Database", GUILayout.Width(600)))
        {
            RefreshDataBase();
        }
    }

    public void RefreshDataBase()
    {
        connection.Query<CharacterData>("DROP TABLE IF EXISTS character");
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT UNIQUE, Partner TEXT, Children TEXT);");

        string ZCChildren = "Haru_Hiro_Lila_Lily_Iris_Mia_Loan_Izuki_Milie_Kari_Isy_Esther_Sacha_Ugo_Liam_Amane_Kira_Élimina";
        CharacterDB Zekio = new CharacterDB("Zekio", "Claire", ZCChildren);

        string HEhildren = "Lim_Emma_Yuko_Torch";
        CharacterDB Haru = new CharacterDB("Haru", "Élia", HEhildren);

        string HCChildren = "Adam_Burn";
        CharacterDB Hiro = new CharacterDB("Hiro", "Clara", HCChildren);

        string INChildren = "Hélio";
        CharacterDB Iris = new CharacterDB("Iris", "Nétior", INChildren);

        CharacterDB Mia = new CharacterDB("Mia", "Eto", null);

        results = connection.Query<CharacterData>("SELECT * FROM character");
    }
}
