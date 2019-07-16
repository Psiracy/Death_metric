using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField]
    DeathMetric deathMetric;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            deathMetric.SaveOnDeath(transform.position);
        }
    }
}
