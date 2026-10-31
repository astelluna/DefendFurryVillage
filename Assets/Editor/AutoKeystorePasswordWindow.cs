using UnityEditor;
using UnityEngine;

public class AutoKeystorePasswordWindow : EditorWindow
{
    string _keystorePass = "";
    string _keyaliasPass = "";

    [MenuItem("Tools/Keystore Password Setting")]
    public static void ShowWindow()
    {
        GetWindow<AutoKeystorePasswordWindow>("Keystore Password");
    }

    void OnEnable()
    {
        _keystorePass = EditorPrefs.GetString("AndroidKeystorePass", "");
        _keyaliasPass = EditorPrefs.GetString("AndroidKeyaliasPass", "");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Keystore Password");
        _keystorePass = EditorGUILayout.PasswordField(_keystorePass);

        EditorGUILayout.LabelField("Key Alias Password");
        _keyaliasPass = EditorGUILayout.PasswordField(_keyaliasPass);

        if (GUILayout.Button("Save"))
        {
            EditorPrefs.SetString("AndroidKeystorePass", _keystorePass);
            EditorPrefs.SetString("AndroidKeyaliasPass", _keyaliasPass);
            EditorUtility.DisplayDialog("저장 완료", "비밀번호가 이 컴퓨터에 저장되었습니다.", "OK");
        }
    }
}