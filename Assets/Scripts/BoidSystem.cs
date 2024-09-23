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

    private NativeArray<LocalTransform> positions;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoidData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        foreach((RefRW<LocalTransform> LocalTransform, RefRW<BoidData> boidData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<BoidData>>())
        {
            
        }
        

        
        BoidJob boidJob = new BoidJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        boidJob.ScheduleParallel();
    }

    private float3 Align(float3 velocity, float3 otherVelocity)
    {
        return math.normalize(otherVelocity - velocity);
    }

    private float3 Cohere(float3 position, float3 otherPosition)
    {
        return otherPosition - position;
    }

    private float3 Separate(float3 position, float3 otherPosition)
    {
        float3 diff = position - otherPosition;
        return math.normalize(diff) / math.length(diff);
    }

    [BurstCompile]
    public partial struct BoidJob : IJobEntity
    {
        public float deltaTime;


        /*This does the function on every thing that has LocalTransform and BoidData?
        public void Execute(ref LocalTransform localTransform, ref BoidData boidData)
        {
            float3 position = localTransform.Position;
            float3 acceleration = float3.zero;

            var a = new OverlapSphereCommand() { };

            //This is apparently not allowed, need to go through each one or do a better way of looking through
            Entities.ForEach((ref LocalTransform otherTransform, ref BoidData otherBoid) =>
            {
                if (localTransform.Equals(otherTransform)) return; // Skip self

                float distance = math.distance(position, otherTranslation.Value);
                if (distance < VisionRadius)
                {
                    // Alignment
                    acceleration += Align(boid.Velocity, otherBoid.Velocity) * AlignmentWeight;
                    // Cohesion
                    acceleration += Cohere(position, otherTranslation.Value) * CohesionWeight;
                    // Separation
                    acceleration += Separate(position, otherTranslation.Value) * SeparationWeight;
                }
            }).Run();

            // Update velocity and position
            boidData.velocity += acceleration * deltaTime;
            localTransform.Position += boidData.velocity * deltaTime;
        }
        */
    }
}
