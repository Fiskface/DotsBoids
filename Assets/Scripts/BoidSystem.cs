using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct BoidSystem : ISystem
{
    private const float AlignmentWeight = 1.0f;
    private const float CohesionWeight = 1.0f;
    private const float SeparationWeight = 1.0f;
    private const float VisionRadius = 5.0f;

    private List<Data> boids;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoidData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        boids = new List<Data>();

        foreach ((RefRW<LocalTransform> LocalTransform, RefRW<BoidData> boidData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<BoidData>>())
        {
            boids.Add(new Data { boid = boidData.ValueRW, localTransform = LocalTransform.ValueRW });
        }


        BoidJob boidJob = new BoidJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            boids = boids.ToArray(),
        };
        boidJob.ScheduleParallel();
    }

    public static float3 Align(float3 velocity, float3 otherVelocity)
    {
        return math.normalize(otherVelocity - velocity);
    }

    private static float3 Cohere(float3 position, float3 otherPosition)
    {
        return otherPosition - position;
    }

    private static float3 Separate(float3 position, float3 otherPosition)
    {
        float3 diff = position - otherPosition;
        return math.normalize(diff) / math.length(diff);
    }

    [BurstCompile]
    public partial struct BoidJob : IJobEntity
    {
        public float deltaTime;
        public Data[] boids;

        public void Execute(ref LocalTransform localTransform, ref BoidData boid)
        {
            float3 position = localTransform.Position;
            float3 acceleration = float3.zero;

            foreach (var otherBoid in boids)
            {
                if (boid.Equals(otherBoid.boid)) return;

                float distance = math.distance(position, otherBoid.localTransform.Position);
                if (distance < VisionRadius)
                {
                    // Alignment
                    acceleration += Align(boid.velocity, otherBoid.boid.velocity) * AlignmentWeight;
                    // Cohesion
                    acceleration += Cohere(position, otherBoid.localTransform.Position) * CohesionWeight;
                    // Separation
                    acceleration += Separate(position, otherBoid.localTransform.Position) * SeparationWeight;
                }
            }

            // Update velocity and position
            boid.velocity += acceleration * deltaTime;
            localTransform.Position += boid.velocity * deltaTime;
        }
        
    }

    public struct Data
    {
        public LocalTransform localTransform;
        public BoidData boid;
    }
}
