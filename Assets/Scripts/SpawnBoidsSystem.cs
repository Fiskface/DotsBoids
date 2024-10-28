using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnBoidsSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnBoidsConfig>();
    }

    protected override void OnUpdate()
    {
        this.Enabled = false;

        SpawnBoidsConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnBoidsConfig>();

        for(int i = 0; i<spawnCubesConfig.amountToSpawn; i++)
        {
            Entity spawnedEntity = EntityManager.Instantiate(spawnCubesConfig.boidPrefabEntity);
            EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = new Unity.Mathematics.float3(Random.Range(-30f, 30f), Random.Range(-30, 30), Random.Range(-30, 30)),
                Rotation = Quaternion.Euler(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90)),
                Scale = 1f
            });
        }
    }
}
