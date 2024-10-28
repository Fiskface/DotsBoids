using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawBehaviour : MonoBehaviour
{
    public int amountToSpawn = 100;
    public GameObject boid;

    void Awake() 
    {
        BoidBehaviour.boids.Clear();
        for (int i = 0; i < amountToSpawn; i++) Spawn();
    }

    private void Spawn()
    {
        Instantiate(boid, new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f)), Quaternion.Euler(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90)));
    }
}
