using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;
using UnityEngine.Events;

public class DeathMetric : MonoBehaviour
{
    //for editor
    public bool useCurrentSession;
    public string sesstionName;

    //norm
    DeathSaveData deathSaveData;
    float timer;
    bool dead;
    bool newSave = true;

    public static DeathMetric Instance { get; private set; }

    void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //save
            if (!Directory.Exists("CurrentSession"))
            {
                Directory.CreateDirectory("CurrentSession");
            }

            if (newSave == true)
            {
                if (File.Exists("CurrentSession/Death.death"))
                {
                    File.Delete("CurrentSession/Death.death");
                }
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnLevelWasLoaded(int level)
    {
        dead = false;
        newSave = false;
    }

    public void SaveOnDeath(Vector3 deathPosition)
    {
        Save(File.Exists("CurrentSession/Death.death"), Time.realtimeSinceStartup, deathPosition);
    }

    void Save(bool doesFileExist, float timeSinceSessionStarted, Vector3 deathPosition)
    {
        deathSaveData = new DeathSaveData
        {
            position = deathPosition,
            timeOfDeath = timeSinceSessionStarted,
            scene = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(deathSaveData);

        if (doesFileExist == true)
        {
            string currentdata = File.ReadAllText("CurrentSession/Death.death");
            json = currentdata + System.Environment.NewLine + json;
        }

        File.WriteAllText("CurrentSession/Death.death", json);
    }

    public void SpawnDeathMarker(Vector3 spawnPos, string name)
    {
        GameObject deathMarker = new GameObject();
        deathMarker.transform.parent = transform;
        deathMarker.transform.position = spawnPos;
        deathMarker.name = name;
    }

    public void ClearDeathMarker(int index)
    {
        DestroyImmediate(transform.GetChild(index).gameObject);
    }

    public void ClearDeathMarker(Transform[] childs)
    {
        for (int i = 0; i < childs.Length; i++)
        {
            DestroyImmediate(childs[i].gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        foreach (Transform child in transform)
        {
            GUIStyle style = new GUIStyle();
            Handles.Label(child.position, new GUIContent(child.name), style);
        }
    }
}

public struct DeathSaveData
{
    public Vector3 position;
    public float timeOfDeath;
    public string scene;
}
