using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;

public class EngangedTab : SQLiteTab<EngagedData>
{
    public EngangedTab(string _tableName, SQLiteConnection _connection) : base(_tableName, _connection)
    {
        list = connection.Query<EngagedData>($"SELECT * from {_tableName}");

        Headers();
    }
}
