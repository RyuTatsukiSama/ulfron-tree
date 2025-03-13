using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ---- Warning ----
// In this code you have 2 #region Dead Code
// This code is currently dead because it make
// the work more complicated and those function are not that useful
// But i keep it in case i would want to finish them

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
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(CName TEXT UNIQUE, Partner TEXT, Children TEXT, Parent TEXT);");

        results = connection.Query<CharacterData>("SELECT * FROM character");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("CName", GUILayout.Width(200));
        GUILayout.Label("Partner", GUILayout.Width(200));
        GUILayout.Label("Children", GUILayout.Width(200));

        GUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(660));

        foreach (CharacterData r in results)
        {
            GUILayout.BeginHorizontal();

            r.CName = GUILayout.TextField(r.CName, GUILayout.Width(200));
            r.Partner = GUILayout.TextField(r.Partner, GUILayout.Width(200));
            r.Children = GUILayout.TextArea(r.Children, GUILayout.Width(200));

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeleteFromDB(r);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update DB", GUILayout.Width(600)))
        {
            UpdateDB();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Backup", GUILayout.Width(300)))
        {
            CreateBackup();
        }

        if (GUILayout.Button("Refresh Database", GUILayout.Width(300)))
        {
            RefreshDataBase();
        }

        GUILayout.EndHorizontal();
    }

    void UpdateDB()
    {
        List<CharacterData> losresults = connection.Query<CharacterData>($"SELECT * FROM character");

        for (int i = 0; i < results.Count; i++)
        {

            if (!results[i].Equals(losresults[i]))
            {
                #region DeadCode 
                //if (results[i].CName != losresults[i].CName)
                //{
                //    UpdateDataByCNameField(ref losresults, i);
                //}

                //if (results[i].Partner != losresults[i].Partner)
                //{
                //    UpdateDataByPartnerField(losresults, i);
                //}

                //if (results[i].Children != losresults[i].Children)
                //{
                //    UpdateDataByChildrenField(losresults, i);
                //}
                #endregion
                connection.Query<CharacterData>($"INSERT OR REPLACE INTO character (id,CName,Partner,Children,Parent) VALUES ({results[i].id},'{results[i].CName}','{results[i].Partner}','{results[i].Children}','{results[i].Parent}')");
            }
        }

        results = connection.Query<CharacterData>("SELECT * FROM character");
    }

    #region Dead Code
    void UpdateDataByCNameField(ref List<CharacterData> losResults, int index)
    {
        if (results[index].Partner != "")
        {
            List<CharacterData> partnerData = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{losResults[index].Partner}' OR CName = '{results[index].Partner}'");
            connection.Query<CharacterData>($"UPDATE character SET Partner = '{results[index].CName}' WHERE id = {partnerData[0].id}");
            results[partnerData[0].id - 1].Partner = results[index].CName;
            losResults[partnerData[0].id - 1].Partner = results[index].CName;
        }

        if (results[index].Parent != "")
        {
            string[] parents = results[index].Parent.Split("_");
            string[] losParents = losResults[index].Parent.Split("_");


            List<CharacterData> parent1Data = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{parents[0]}' OR CName = '{losParents[0]}'");
            List<CharacterData> parent2Data = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{parents[1]}' OR CName = '{losParents[1]}'");
            
            string newChildren = string.Empty;
            foreach (string child in parent1Data[0].Children.Split("_"))
            {
                if (child != results[index].CName )
                {

                }
            }

        }
    }

    void UpdateDataByPartnerField(List<CharacterData> losResults, int index)
    {
        if (losResults[index].Partner == "")
        {
            connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children,Parent) VALUES ('{results[index].Partner}','{results[index].CName}','{results[index].Children}','')");
        }
        else
        {
            List<CharacterData> partnerData = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{losResults[index].Partner}'");
            connection.Query<CharacterData>($"UPDATE character SET CName = '{results[index].Partner}' WHERE id = {partnerData[0].id}");
        }
    }

    void UpdateDataByChildrenField(List<CharacterData> losResults, int index)
    {
        List<CharacterData> partner = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{results[index].Partner}'");
        connection.Query<CharacterData>($"UPDATE character SET Children = '{results[index].Children}' WHERE id = {partner[0].id}");

        string[] childrenResults = results[index].Children.Split("_");
        string[] childrenLosResults = losResults[index].Children.Split("_");

        for (int j = 0; j < childrenResults.Length; j++)
        {
            if (childrenResults.Length == childrenLosResults.Length && childrenResults[j] != childrenLosResults[j]) // Update a child who already exist
            {
                List<CharacterData> childrenData = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{childrenLosResults[j]}'");
                connection.Query<CharacterData>($"UPDATE character SET CName = '{childrenResults[j]}' WHERE id = {childrenData[0].id}");
            }
            else if (connection.Query<CharacterData>($"SELECT * FROM character WHERE CName = '{childrenResults[j]}'").Count == 0) // Create a new child
            {
                connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children,Parent) VALUES ('{childrenResults[j]}','','','')");
            }
        }
    }
    #endregion

    void DeleteFromDB(CharacterData character)
    {
        results = connection.Query<CharacterData>($"DELETE FROM Character WHERE id={character.id}");
    }

    void CreateBackup()
    {
        if (File.Exists(Application.dataPath + "/StreamingAssets/ulfronBackup.db"))
        {
            File.Delete(Application.dataPath + "/StreamingAssets/ulfronBackup.db");
        }

        File.Copy(Application.dataPath + "/StreamingAssets/ulfron.db", Application.dataPath + "/StreamingAssets/ulfronBackup.db");
    }

    void RefreshDataBase()
    {
        connection.Query<CharacterData>("DROP TABLE IF EXISTS character");
        connection.Query<CharacterData>("CREATE TABLE IF NOT EXISTS character(id INTEGER PRIMARY KEY, CName TEXT UNIQUE, Partner TEXT, Children TEXT, Parent TEXT);");

        SQLiteConnection backupConnection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfronBackup.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        List<CharacterData> backupData = backupConnection.Query<CharacterData>("SELECT * FROM character");

        foreach (CharacterData backupDataItem in backupData)
        {
            connection.Query<CharacterData>($"INSERT INTO character (id, CName,Partner,Children, Parent) VALUES ({backupDataItem.id}, '{backupDataItem.CName}','{backupDataItem.Partner}','{backupDataItem.Children}','{backupDataItem.Parent}')");
        }

        results = connection.Query<CharacterData>("SELECT * FROM character");

    }

}