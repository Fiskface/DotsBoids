using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnBoidsConfigAuthoring : MonoBehaviour
{
    public GameObject boidPrefab;
    public int amountToSpawn;

    public class Baker : Baker<SpawnBoidsConfigAuthoring>
    {
        public override void Bake(SpawnBoidsConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnBoidsConfig{
                boidPrefabEntity = GetEntity(authoring.boidPrefab, TransformUsageFlags.Dynamic),
                amountToSpawn = authoring.amountToSpawn,
            });
        }
    }
}

public struct SpawnBoidsConfig : IComponentData
{
    public Entity boidPrefabEntity;
    public int amountToSpawn;
}
