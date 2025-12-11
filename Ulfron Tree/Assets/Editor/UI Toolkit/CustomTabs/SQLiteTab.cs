using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;

public class SQLiteTab<T> : Tab
{
    protected string tableName;

    protected ListView listView;

    protected List<T> list;

    protected SQLiteConnection connection;

    public SQLiteTab(string _tableName, SQLiteConnection _connection) : base(_tableName)
    {
        tableName = _tableName;
        connection = _connection;
    }
}
