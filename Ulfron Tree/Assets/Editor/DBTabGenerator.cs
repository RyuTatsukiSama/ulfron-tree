using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DBTabGenerator : EditorWindow
{


    [MenuItem("DataBase/Database Tab Generator")]
    public static void GetWindow()
    {
        DBTabGenerator wnd = GetWindow<DBTabGenerator>();
        wnd.titleContent = new GUIContent("DB Tab Generator");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        UnityEngine.UIElements.PopupField<MonoBehaviour> testFiled = new();

        root.Add(testFiled);
    }

    void GenerateTab()
    {

    }
}
