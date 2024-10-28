using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public partial struct BoidSystem : ISystem
{
    private const float AlignmentWeight = 1.2f;
    private const float CohesionWeight = 1.5f;
    private const float SeparationWeight = 1.0f;
    private const float ToCenterWeight = 0.04f;
    private const float VisionRadius = 15.0f;
    private const float maxVelocity = 10f;

    private const int amountSpawned = 11500;

    [NativeDisableParallelForRestriction]
    private NativeArray<LocalTransform> transforms;
    [NativeDisableParallelForRestriction]
    private NativeArray<BoidData> boids;

    public void OnCreate(ref SystemState state)
    {
        
        state.RequireForUpdate<BoidData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        transforms = new NativeArray<LocalTransform>(amountSpawned, Allocator.TempJob);
        boids = new NativeArray<BoidData>(amountSpawned, Allocator.TempJob);

        var counter = 0;
        foreach ((RefRW<LocalTransform> LocalTransform, RefRW<BoidData> boidData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<BoidData>>())
        {
            transforms[counter] = LocalTransform.ValueRW;
            boids[counter] = boidData.ValueRW;
            counter++;

        }


        BoidJob boidJob = new BoidJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            transforms = transforms,
            boids = boids,
        };
        //state.Dependency = boidJob.Schedule(state.Dependency);
        state.Dependency = boidJob.ScheduleParallel(state.Dependency);

        
        transforms.Dispose(state.Dependency);
        boids.Dispose(state.Dependency);
        
    }

    private static float3 Separate(float3 position, float3 otherPosition)
    {
        float3 diff = position - otherPosition;
        return math.normalize(diff) * (VisionRadius - math.length(diff));
    }

    [BurstCompile]
    public partial struct BoidJob : IJobEntity
    {
        public float deltaTime;
        [ReadOnly] [NativeDisableParallelForRestriction] public NativeArray<LocalTransform> transforms;
        [ReadOnly] [NativeDisableParallelForRestriction] public NativeArray<BoidData> boids;

        public void Execute(ref LocalTransform localTransform, ref BoidData boid)
        {

            float3 position = localTransform.Position;

            float3 alignMove = float3.zero;
            float3 cohereMove = float3.zero;
            float3 separateMove = float3.zero;

            int counter = 0;

            for (int i = 0; i < transforms.Length; i++)
            {

                
                var trans = transforms[i];
                if (localTransform.Equals(trans)) continue;

                float distance = math.distance(position, trans.Position);
                if (distance < VisionRadius)
                {
                    counter++;


                    alignMove += math.mul(trans.Rotation, new float3(0,0,1)) * AlignmentWeight;

                    cohereMove += trans.Position;

                    separateMove += Separate(position, trans.Position) * SeparationWeight;
                    

                    
                }

            }

            //Finish up align, cohere, separate
            if (counter == 0)
            {
                alignMove = math.normalize(math.mul(localTransform.Rotation, new float3(0, 0, 1))) * AlignmentWeight;
            }
            else
            {
                alignMove /= counter;

                cohereMove /= counter;
                cohereMove -= localTransform.Position;
                cohereMove *= CohesionWeight;
            }

            float3 centerMove = -localTransform.Position * ToCenterWeight;

            float3 acceleration = alignMove + cohereMove + separateMove + centerMove;

            // Update velocity and position
            boid.velocity += acceleration * deltaTime;

            if (math.length(boid.velocity) > maxVelocity) 
            {
                var dir = math.normalize(boid.velocity);
                boid.velocity = dir * maxVelocity;
            }

            localTransform.Position += boid.velocity * deltaTime;
            localTransform.Rotation = Quaternion.LookRotation(boid.velocity, Vector3.up);
        }
        
    }
}
