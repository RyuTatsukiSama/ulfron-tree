using ExtensionSQLite;
using SQLite4Unity3d;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NewDBView : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [SerializeField]
    private StyleSheet m_StyleSheet = default;

    [SerializeField]
    private int keyValuePair;

    private SQLiteConnection connection;

    [MenuItem("DataBase/NewDBView")]
    public static void GetWindow()
    {
        NewDBView wnd = GetWindow<NewDBView>();
        wnd.titleContent = new GUIContent("NewDBView");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Open the table and create it if necessary
        connection = SQLiteExtensions.OpenUlfronTable();
        connection.CreateUlfronTable();

        // get all the tab
        List<TableData> tables = connection.Query<TableData>("SELECT * FROM sqlite_master WHERE type='table';");

        TabView tabView = new TabView();

        foreach (TableData table in tables)
        {
            tabView.Add(CreateTabTable(table.name));
        }

        tabView.Add(CreateTabTable("AddCharacter"));

        root.Add(tabView);
    }

    public Tab CreateTabTable(string _name)
    {
        Tab tab = null;

        switch (_name)
        {
            case "character":
                tab = new CharacterTab(connection);
                break;
            case "engaged":
                tab = new EngangedTab(connection);
                break;
            case "kinship":
                tab = new KinshipTab(connection);
                break;
            default:
                tab = new Tab(_name);
                break;
        }

        return tab;
    }
}