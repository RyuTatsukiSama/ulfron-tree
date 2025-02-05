using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;

public class CharacterDB
{
    public CharacterData data;

    public static SQLiteConnection connection;

    public CharacterDB()
    {
        data = null;
    }

    public CharacterDB(string _name, string _partner, string _children)
    {
        connection = new SQLiteConnection(Application.dataPath + "/StreamingAssets/ulfron.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        data = new CharacterData();
        data.CName = _name;
        data.Partner = _partner;
        data.Children = _children;

        List<CharacterData> results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{data.CName}'");
        if (results.Count > 0) // Itself
        {
            connection.Query<CharacterData>($"INSERT OR REPLACE INTO character (id,CName,Partner,Children,Parent) VALUES ({results[0].id},'{data.CName}','{data.Partner}','{data.Children}','{results[0].Parent}')");
        }
        else
        {
            connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children,Parent) VALUES ('{data.CName}','{data.Partner}','{data.Children}','')");
        }

        results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{data.Partner}'");
        if (results.Count == 0) // Partner
        {
            connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children,Parent) VALUES ('{data.Partner}','{data.CName}','{data.Children}','')");
        }

        if (data.Children != null) // Children
        {
            foreach (string child in data.Children.Split("_"))
            {
                results = connection.Query<CharacterData>($"SELECT * FROM character WHERE CName='{child}'");
                if (results.Count == 0)
                {
                    connection.Query<CharacterData>($"INSERT INTO character (CName,Partner,Children,Parent) VALUES ('{child}','','','{data.CName}_{data.Partner}')");
                }
            }
        }
    }
}
