using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using System.Reflection;

[CustomEditor(typeof(DeathMetric)), CanEditMultipleObjects]
public class DeathMetricEditor : Editor
{
    int index = 0;

    public SerializedProperty
        Curresesionbool_PROP,
        SessionName_PROP;

    private void OnEnable()
    {
        Curresesionbool_PROP = serializedObject.FindProperty("useCurrentSession");
        SessionName_PROP = serializedObject.FindProperty("sesstionName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //bool for to see if currrent one or saved one is used
        EditorGUILayout.PropertyField(Curresesionbool_PROP, new GUIContent("use the current session"));

        bool useCurrentSession = Curresesionbool_PROP.boolValue;

        if (useCurrentSession == false)
        {
            //load name
            DirectoryInfo info = new DirectoryInfo("SavedSessions");

            if (Directory.Exists("SavedSessions"))
            {
                List<string> filenames = new List<string>();
                for (int i = 0; i < info.GetFiles().Length; i++)
                {
                    filenames.Add(info.GetFiles()[i].Name);
                }
                index = EditorGUILayout.Popup(new GUIContent("select file"), index, filenames.ToArray());
            }

            //load
            if (GUILayout.Button("Load Session"))
            {
                //get from json
                List<DeathSaveData> deathSaveData = new List<DeathSaveData>();
                foreach (string line in File.ReadLines(info.GetFiles()[index].FullName))
                {
                    deathSaveData.Add(JsonUtility.FromJson<DeathSaveData>(line));
                }

                //spawn markers
                DeathMetric baseScript = (DeathMetric)target;
                List<Transform> childs = new List<Transform>();
                foreach (Transform child in baseScript.transform)
                {
                    childs.Add(child);
                }
                baseScript.ClearDeathMarker(childs.ToArray());
                for (int i = 0; i < deathSaveData.Count; i++)
                {
                    Debug.LogFormat("saved scene: {0} current scene: {1}", deathSaveData[i].scene, SceneManager.GetActiveScene().name);
                    if (deathSaveData[i].scene == SceneManager.GetActiveScene().name)
                    {
                        Debug.Log("Spawn");
                        baseScript.SpawnDeathMarker(deathSaveData[i].position, "Time of death since startup: " + deathSaveData[i].timeOfDeath.ToString("f2"));
                    }
                }
            }
        }
        else
        {
            //load
            if (GUILayout.Button("Load Current Session"))
            {
                //get from json
                List<DeathSaveData> deathSaveData = new List<DeathSaveData>();
                foreach (string line in File.ReadLines("CurrentSession/Death.death"))
                {
                    deathSaveData.Add(JsonUtility.FromJson<DeathSaveData>(line));
                }

                //spawn markers
                DeathMetric baseScript = (DeathMetric)target;
                List<Transform> childs = new List<Transform>();
                foreach (Transform child in baseScript.transform)
                {
                    childs.Add(child);
                }
                baseScript.ClearDeathMarker(childs.ToArray());
                for (int i = 0; i < deathSaveData.Count; i++)
                {
                    Debug.LogFormat("saved scene: {0} current scene: {1}", deathSaveData[i].scene, SceneManager.GetActiveScene().name);
                    if (deathSaveData[i].scene == SceneManager.GetActiveScene().name)
                    {
                        Debug.Log("Spawn");
                        baseScript.SpawnDeathMarker(deathSaveData[i].position, "Time of death since startup: " + deathSaveData[i].timeOfDeath.ToString("f2"));
                    }
                }
            }
        }

        //name
        EditorGUILayout.PropertyField(SessionName_PROP, new GUIContent("session save name"));

        //save
        if (GUILayout.Button("Save Current Session"))
        {
            if (!Directory.Exists("SavedSessions"))
            {
                Directory.CreateDirectory("SavedSessions");
            }

            if (!File.Exists("SavedSessions/" + SessionName_PROP.stringValue + ".death"))
            {
                File.Copy("CurrentSession/Death.death", "SavedSessions/" + SessionName_PROP.stringValue + ".death");
            }
            else
            {
                Debug.LogError("file name already in use please choose a different one");
            }
        }

        //clear
        if (GUILayout.Button("Clear Current Death Markers"))
        {
            DeathMetric baseScript = (DeathMetric)target;
            List<Transform> childs = new List<Transform>();
            foreach (Transform child in baseScript.transform)
            {
                childs.Add(child);
            }
            baseScript.ClearDeathMarker(childs.ToArray());
        }

        serializedObject.ApplyModifiedProperties();
    }
}
