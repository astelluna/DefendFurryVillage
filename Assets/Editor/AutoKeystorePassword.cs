using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AutoKeystorePassword : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android) return;
        if (!PlayerSettings.Android.useCustomKeystore) return;

        PlayerSettings.Android.keystorePass =
            System.Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS")
            ?? EditorPrefs.GetString("AndroidKeystorePass", "");

        PlayerSettings.Android.keyaliasPass =
            System.Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS")
            ?? EditorPrefs.GetString("AndroidKeyaliasPass", "");
    }
}