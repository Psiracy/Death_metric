﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    DeathMetric deathMetric;

    private void Start()
    {
        deathMetric = DeathMetric.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            deathMetric.SaveOnDeath(transform.position);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
