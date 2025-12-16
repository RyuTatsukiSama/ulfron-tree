using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;

public class KinshipTab : SQLiteTab<KinshipData>
{
    public KinshipTab(string _tableName, SQLiteConnection _connection) : base(_tableName, _connection)
    {
        response = connection.Query<KinshipData>($"SELECT * from {_tableName}");

        Headers();
        Data();
    }
}
