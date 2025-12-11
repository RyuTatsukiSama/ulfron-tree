using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class SQLiteTab<T> : Tab where T : new()
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

    public void Headers()
    {
        // Get the list fo the field
        T empty = new T();
        List<System.Reflection.FieldInfo> targetFields = empty.GetType().GetFields(System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly |
            System.Reflection.BindingFlags.NonPublic)
            .ToList();

        VisualElement headerBox = new VisualElement();
        headerBox.style.flexDirection = FlexDirection.Row;
        headerBox.style.display = DisplayStyle.Flex;
        headerBox.style.justifyContent = Justify.SpaceAround;
        headerBox.style.unityTextAlign = TextAnchor.MiddleCenter;
        headerBox.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerBox.style.fontSize = 16;
        headerBox.style.borderBottomColor = new StyleColor(new Color(66, 79, 91));
        headerBox.style.borderBottomWidth = 1;

        foreach (var field in targetFields)
        {
            Label nameHeader = new Label(field.Name.Split('<', '>')[1]);
            nameHeader.style.width = new StyleLength(new Length(100f / targetFields.Count, LengthUnit.Percent));
            headerBox.Add(nameHeader);
        }

        Add(headerBox);
    }
}
