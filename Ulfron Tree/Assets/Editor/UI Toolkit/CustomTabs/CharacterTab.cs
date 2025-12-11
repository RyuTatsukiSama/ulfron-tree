using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;

public class CharacterTab : SQLiteTab<CharacterDataNew>
{
    public CharacterTab(string _tableName, SQLiteConnection _connection) : base(_tableName, _connection)
    {
        list = connection.Query<CharacterDataNew>($"SELECT * from {_tableName}");

        VisualElement headerBox = new VisualElement();
        headerBox.style.flexDirection = FlexDirection.Row;
        headerBox.style.display = DisplayStyle.Flex;
        headerBox.style.unityTextAlign = TextAnchor.MiddleCenter;
        headerBox.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerBox.style.borderBottomColor = new StyleColor(new Color(66, 79, 91));
        headerBox.style.borderBottomWidth = 1;
        headerBox.style.fontSize = 16;

        Label nameHeader = new Label("Name");
        nameHeader.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));
        headerBox.Add(nameHeader);

        Label partnerHeader = new Label("Partner");
        partnerHeader.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));
        headerBox.Add(partnerHeader);

        Label childrenHeader = new Label("Children");
        childrenHeader.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));
        headerBox.Add(childrenHeader);

        Add(headerBox);

        foreach (CharacterDataNew data in list)
        {
            // Box of a character
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;
            box.style.marginBottom = 10;
            box.style.fontSize = 15;

            // Name of the character ( editable )
            TextField nameField = new TextField();
            nameField.value = data.CName;
            nameField.style.flexGrow = 1;
            nameField.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));

            box.Add(nameField);

            // Name of the parner character
            CharacterDataNew partner = connection.Query<CharacterDataNew>($"SELECT CName FROM character,engaged WHERE (character.id=engaged.id_spouse1 AND engaged.id_spouse2={data.id}) OR (character.id=engaged.id_spouse2 AND engaged.id_spouse1={data.id});").FirstOrDefault();

            Label partnerLabel = new Label();
            partnerLabel.style.flexGrow = 1;
            partnerLabel.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));

            if (partner != null)
            {
                partnerLabel.text = partner.CName;
            }
            box.Add(partnerLabel);

            // Children between them
            List<CharacterDataNew> children = connection.Query<CharacterDataNew>($"SELECT CName FROM character,kinship WHERE character.id=id_child AND (kinship.id_parent1={data.id} OR kinship.id_parent2={data.id});");

            VisualElement childrenBox = new VisualElement();
            childrenBox.style.flexDirection = FlexDirection.Column;
            childrenBox.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));

            foreach ( CharacterDataNew child in children )
            {
                Label childLabel = new Label();
                childLabel.style.flexGrow = 1;
                childLabel.text = child.CName;
                childrenBox.Add(childLabel);
            }

            box.Add(childrenBox);

            Add(box);
        }
    }
}
