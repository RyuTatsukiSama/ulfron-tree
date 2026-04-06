using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class EngangedTab : SQLiteTab<EngagedData>
{
    const float headerSize = 100f / 2f;

    public EngangedTab(SQLiteConnection _connection) : base("Engaged", _connection)
    {
        response = connection.Query<EngagedData>($"SELECT * from {tableName}");

        Headers();
        Data();
    }

    new void Headers()
    {
        VisualElement headerBox = new VisualElement();
        headerBox.style.flexDirection = FlexDirection.Row;
        headerBox.style.display = DisplayStyle.Flex;
        headerBox.style.unityTextAlign = TextAnchor.MiddleCenter;
        headerBox.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerBox.style.borderBottomColor = new StyleColor(new Color(66, 79, 91));
        headerBox.style.borderBottomWidth = 1;
        headerBox.style.fontSize = 16;

        Label nameHeader = new Label("Spouse 1");
        nameHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(nameHeader);

        Label partnerHeader = new Label("Spouse 2");
        partnerHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(partnerHeader);

        Add(headerBox);
    }

    new void Data()
    {
        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        scrollView.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

        foreach (EngagedData data in response)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;
            box.style.marginBottom = 10;
            box.style.fontSize = 15;

            LoadSpouseBox(box, data.id_spouse1);

            LoadSpouseBox(box, data.id_spouse2);

            scrollView.Add(box);
        }

        Add(scrollView);
    }

    void LoadSpouseBox(VisualElement box, int id)
    {
        CharacterData Spouse = connection.Query<CharacterData>($"SELECT CName FROM character WHERE character.id={id};").First();

        TextField idField = new TextField();
        idField.style.flexGrow = 1;
        idField.style.width = new StyleLength(new Length(headerSize / 2f, LengthUnit.Percent));
        idField.value = id.ToString();

        box.Add(idField);

        Label nameLabel = new Label();
        nameLabel.style.flexGrow = 1;
        nameLabel.style.width = new StyleLength(new Length(headerSize / 2f, LengthUnit.Percent));
        nameLabel.text = Spouse.CName;

        box.Add(nameLabel);
    }
}
