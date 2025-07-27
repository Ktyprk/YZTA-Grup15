using UnityEditor;
using UnityEngine;

public class EditorInputDialog : EditorWindow
{
    private static string message;
    private static string inputText;
    private static bool isConfirmed = false;

    public static string Show(string title, string dialogMessage, string defaultValue)
    {
        message = dialogMessage;
        inputText = defaultValue;
        isConfirmed = false;

        var window = CreateInstance<EditorInputDialog>();
        window.titleContent = new GUIContent(title);
        window.minSize = new Vector2(300, 100);
        window.maxSize = new Vector2(300, 100);
        window.ShowModalUtility();

        return isConfirmed ? inputText : null;
    }

    private void OnGUI()
    {
        GUILayout.Label(message, EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        GUI.SetNextControlName("InputField");
        inputText = EditorGUILayout.TextField(inputText);
        GUI.FocusControl("InputField");

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("OK", GUILayout.Width(80)))
        {
            isConfirmed = true;
            Close();
        }

        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
            isConfirmed = false;
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnLostFocus()
    {
        Focus();
    }
}