using UnityEditor;
using UnityEngine;

public class DBTabGenerator : EditorWindow
{


    [MenuItem("DataBase/Database Tab Generator")]
    public static void GetWindow()
    {
        DBTabGenerator wnd = GetWindow<DBTabGenerator>();
        wnd.titleContent = new GUIContent("DB Tab Generator");
    }


    void GenerateTab()
    {

    }
}
