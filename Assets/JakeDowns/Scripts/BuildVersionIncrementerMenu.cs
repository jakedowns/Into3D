using UnityEngine;
using UnityEditor;

public class BuildVersionIncrementerMenu : MonoBehaviour
{

#if UNITY_EDITOR
    [MenuItem("Build/Increment Patch Version")]
    static void IncrementPatchVersion()
    {
        IncrementVersionNumber(2);
    }

    [MenuItem("Build/Increment Minor Version")]
    static void IncrementMinorVersion()
    {
        IncrementVersionNumber(1);
    }

    [MenuItem("Build/Increment Major Version")]
    static void IncrementMajorVersion()
    {
        IncrementVersionNumber(0);
    }


    static void IncrementVersionNumber(int versionIndex)
    {
        Debug.LogWarning("Incrementing Version Number " + versionIndex + " " + PlayerSettings.bundleVersion);
        // Get the current version numbers
        string[] versionNumbers = PlayerSettings.bundleVersion.Split('.');

        // Increment the specified version number
        int versionNumber = int.Parse(versionNumbers[versionIndex]);
        versionNumber++;
        versionNumbers[versionIndex] = versionNumber.ToString();

        // Reset the lower version numbers to zero
        for (int i = versionIndex + 1; i < versionNumbers.Length; i++)
        {
            versionNumbers[i] = "0";
        }

        // Update the version numbers in PlayerSettings
        PlayerSettings.bundleVersion = string.Join(".", versionNumbers);

        // Increment the version code
        PlayerSettings.Android.bundleVersionCode++;

        // Log the new version number
        Debug.Log("Build version incremented to: " + PlayerSettings.bundleVersion);
        Debug.Log("Version code incremented to: " + PlayerSettings.Android.bundleVersionCode);
    }
#endif
}
