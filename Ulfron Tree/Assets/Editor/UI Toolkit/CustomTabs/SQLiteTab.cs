using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using System.Reflection;

public class SQLiteTab<T> : Tab where T : new()
{
    protected string tableName;

    protected ListView listView;

    protected List<T> response;

    protected SQLiteConnection connection;

    protected ScrollView scrollView;

    List<FieldInfo> templateFields;

    public SQLiteTab(string _tableName, SQLiteConnection _connection) : base(_tableName)
    {
        tableName = _tableName;
        connection = _connection;

        T empty = new T();
        templateFields = empty.GetType().GetFields(System.Reflection.BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic)
            .ToList();
    }

    /// <summary>
    /// Just here for preview and if you want to just see the table has it is
    /// Feel free to create a new void Headers() method in the child class
    /// </summary>
    public void Headers()
    {
        VisualElement headerBox = new VisualElement();
        headerBox.style.flexDirection = FlexDirection.Row;
        headerBox.style.display = DisplayStyle.Flex;
        headerBox.style.justifyContent = Justify.SpaceAround;
        headerBox.style.unityTextAlign = TextAnchor.MiddleCenter;
        headerBox.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerBox.style.fontSize = 16;
        headerBox.style.borderBottomColor = new StyleColor(new Color(66, 79, 91));
        headerBox.style.borderBottomWidth = 1;

        foreach (FieldInfo field in templateFields)
        {
            Label nameHeader = new Label(field.Name.Split('<', '>')[1]);
            nameHeader.style.width = new StyleLength(new Length(100f / templateFields.Count, LengthUnit.Percent));
            headerBox.Add(nameHeader);
        }

        Add(headerBox);
    }

    /// <summary>
    /// Just here for preview and if you want to just see the table has it is
    /// Feel free to create a new void Data() method in the child class
    /// </summary>
    public void Data()
    {
        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        scrollView.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

        foreach (T data in response)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;
            box.style.marginBottom = 10;
            box.style.fontSize = 15;

            foreach (FieldInfo field in templateFields)
            {
                Label dataLabel = new Label();
                dataLabel.style.flexGrow = 1;
                dataLabel.style.width = new StyleLength(new Length(100f / templateFields.Count, LengthUnit.Percent));
                dataLabel.text = field.GetValue(data).ToString();
                
                box.Add(dataLabel);
            }

            scrollView.Add(box);
        }

        Add(scrollView);
    }
}
