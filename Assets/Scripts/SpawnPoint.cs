using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint: MonoBehaviour
{
    public GameObject spawnPoint;
    public bool Occupied = false;

    void Start() {
        spawnPoint = this.gameObject;
    }
}
