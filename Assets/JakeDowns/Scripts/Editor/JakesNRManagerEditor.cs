using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JakesNRManager))]
public class JakesNRManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        JakesNRManager myScript = (JakesNRManager)target;
        if (GUILayout.Button("Initialize NR"))
        {
            myScript.InitNR();
        }
    }
}