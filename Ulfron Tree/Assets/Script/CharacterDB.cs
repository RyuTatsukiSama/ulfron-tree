using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;

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

        connection.Execute($"INSERT OR IGNORE INTO character (cName) VALUES ('{_name}');");
        CharacterDataNew currentChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE cName = '{_name}';").First();

        CharacterDataNew partnerChara = null;
        if (_partner != null && _partner != "") // Partner
        {
            connection.Execute($"INSERT OR IGNORE INTO character (cName) VALUES ('{_partner}');");
            partnerChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE cName = '{_partner}';").First();
            connection.Execute($"INSERT OR IGNORE INTO engaged (id_wife,id_husband) VALUES ('{partnerChara.id}','{currentChara.id}');");
        }

        if (_children != null) // Children
        {
            foreach (string child in _children.Split("_"))
            {
                connection.Execute($"INSERT OR IGNORE INTO character (cName) VALUES ('{child}');");
                CharacterDataNew childChara = connection.Query<CharacterDataNew>($"SELECT * FROM character WHERE cName = '{child}';").First();
                connection.Execute($"INSERT OR IGNORE INTO kinship (id_parent,id_child) VALUES ('{currentChara.id}','{childChara.id}');");
                if (partnerChara != null)
                    connection.Execute($"INSERT OR IGNORE INTO kinship (id_parent,id_child) VALUES ('{partnerChara.id}','{childChara.id}');");
            }
        }

        connection.Close();
    }
}
