using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;

public class KinshipTab : SQLiteTab<KinshipData>
{
    const float headerSize = 100f / 3f;

    public KinshipTab(SQLiteConnection _connection) : base("Kinship", _connection)
    {
        response = connection.Query<KinshipData>($"SELECT * from {tableName}");

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

        Label nameHeader = new Label("Parent 1");
        nameHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(nameHeader);

        Label partnerHeader = new Label("Parent 2");
        partnerHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(partnerHeader);

        Label childHeader = new Label("Child");
        childHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(childHeader);

        Add(headerBox);
    }

    new void Data()
    {
        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        scrollView.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

        foreach (KinshipData data in response)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;
            box.style.marginBottom = 10;
            box.style.fontSize = 15;

            CharacterBox(box, data.id_parent1);

            CharacterBox(box, data.id_parent2);

            CharacterBox(box, data.id_child);

            scrollView.Add(box);
        }

        Add(scrollView);
    }

    void CharacterBox(VisualElement box, int id)
    {
        CharacterData Character = connection.Query<CharacterData>($"SELECT CName FROM character WHERE character.id={id};").First();

        TextField idField = new TextField();
        idField.style.flexGrow = 1;
        idField.style.width = new StyleLength(new Length(headerSize / 2f, LengthUnit.Percent));
        idField.value = id.ToString();

        box.Add(idField);

        Label nameLabel = new Label();
        nameLabel.style.flexGrow = 1;
        nameLabel.style.width = new StyleLength(new Length(headerSize / 2f, LengthUnit.Percent));
        nameLabel.text = Character.CName;

        box.Add(nameLabel);
    }
}
