using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SQLite4Unity3d;
using System.Linq;

public class CharacterTab : SQLiteTab<CharacterDataNew>
{
    Dictionary<string, TextField> pairs = new();

    const float headerSize = 96.8f / 3f;

    public CharacterTab(string _tableName, SQLiteConnection _connection) : base(_tableName, _connection)
    {
        LoadTab();
    }

    void LoadTab()
    {
        response = connection.Query<CharacterDataNew>($"SELECT * from {tableName}");

        VisualElement headerBox = new VisualElement();
        headerBox.style.flexDirection = FlexDirection.Row;
        headerBox.style.display = DisplayStyle.Flex;
        headerBox.style.unityTextAlign = TextAnchor.MiddleCenter;
        headerBox.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerBox.style.borderBottomColor = new StyleColor(new Color(66, 79, 91));
        headerBox.style.borderBottomWidth = 1;
        headerBox.style.fontSize = 16;

        Label nameHeader = new Label("Name");
        nameHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(nameHeader);

        Label partnerHeader = new Label("Partner");
        partnerHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(partnerHeader);

        Label childrenHeader = new Label("Children");
        childrenHeader.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
        headerBox.Add(childrenHeader);

        Add(headerBox);

        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        scrollView.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

        foreach (CharacterDataNew data in response)
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
            nameField.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));
            pairs[data.CName] = nameField;

            box.Add(nameField);

            // Name of the parner character
            CharacterDataNew partner = connection.Query<CharacterDataNew>($"SELECT CName FROM character,engaged WHERE (character.id=engaged.id_spouse1 AND engaged.id_spouse2={data.id}) OR (character.id=engaged.id_spouse2 AND engaged.id_spouse1={data.id});").FirstOrDefault();

            Label partnerLabel = new Label();
            partnerLabel.style.flexGrow = 1;
            partnerLabel.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));

            if (partner != null)
            {
                partnerLabel.text = partner.CName;
            }
            box.Add(partnerLabel);

            // Children between them
            List<CharacterDataNew> children = connection.Query<CharacterDataNew>($"SELECT CName FROM character,kinship WHERE character.id=id_child AND (kinship.id_parent1={data.id} OR kinship.id_parent2={data.id});");

            VisualElement childrenBox = new VisualElement();
            childrenBox.style.flexDirection = FlexDirection.Column;
            childrenBox.style.width = new StyleLength(new Length(headerSize, LengthUnit.Percent));

            foreach (CharacterDataNew child in children)
            {
                Label childLabel = new Label();
                childLabel.style.flexGrow = 1;
                childLabel.text = child.CName;
                childrenBox.Add(childLabel);
            }

            box.Add(childrenBox);

            Button removeButton = new Button(RemoveFromTable);
            removeButton.style.width = 10;
            removeButton.text = "-";

            box.Add(removeButton);

            scrollView.Add(box);
        }

        Button addCharacter = new Button(AddToTable);
        addCharacter.text = "Add Character";

        scrollView.Add(addCharacter);

        Add(scrollView);

        VisualElement buttonBox = new VisualElement();
        buttonBox.style.flexDirection = FlexDirection.Row;
        buttonBox.style.unityTextAlign = TextAnchor.MiddleCenter;

        buttonBox.Add(AddOptionButton(SaveTable, "Save Table"));
        buttonBox.Add(AddOptionButton(CreateBackUp, "Create Backup"));
        buttonBox.Add(AddOptionButton(SaveTable, "Restore Backup"));

        Add(buttonBox);
    }

    void AddToTable()
    {

    }

    void RemoveFromTable()
    {

    }

    Button AddOptionButton(System.Action clickEvent, string text)
    {
        Button button = new Button(clickEvent);
        button.style.width = new StyleLength(new Length(100f / 3f, LengthUnit.Percent));
        button.style.height = 30;
        button.text = text;

        return button;
    }

    void SaveTable()
    {
        foreach (CharacterDataNew character in response)
        {
            if (pairs[character.CName].value != character.CName)
            {
                connection.Execute($"UPDATE character SET CName='{pairs[character.CName].value}' WHERE id={character.id}");
            }
        }

        Clear();
        LoadTab();
    }

    void CreateBackUp()
    {

    }

    void RestoreBackup()
    {

    }
}
