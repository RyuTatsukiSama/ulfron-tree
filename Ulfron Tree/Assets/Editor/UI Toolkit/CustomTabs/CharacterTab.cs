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

        Label nameHeader = new Label("Name");
        Label partnerHeader = new Label("Partner");
        Label childrenHeader = new Label("Children");

        foreach (CharacterDataNew data in list)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;
            box.style.display = DisplayStyle.Flex;
            box.style.justifyContent = Justify.SpaceAround;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;

            TextField txtField = new TextField();
            txtField.value = data.CName;
            txtField.style.flexGrow = 1;

            box.Add(txtField);

            CharacterDataNew partner = connection.Query<CharacterDataNew>($"SELECT CName FROM character,engaged WHERE (character.id=engaged.id_husband AND engaged.id_wife={data.id}) OR (character.id=engaged.id_wife AND engaged.id_husband={data.id});").FirstOrDefault();

            Label partnerLabel = new Label();
            partnerLabel.style.flexGrow = 1;

            if (partner != null)
            {
                partnerLabel.text = partner.CName;
            }
            box.Add(partnerLabel);



            Add(box);
        }
    }
}
