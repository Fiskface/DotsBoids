using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoidDataAuthoring : MonoBehaviour
{
    public float3 velocity;

    private class Baker : Baker<BoidDataAuthoring>
    {
        public override void Bake(BoidDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BoidData
            {
                velocity = authoring.velocity,
            });
        }
    }
}


public struct BoidData : IComponentData
{
    public float3 velocity;
}