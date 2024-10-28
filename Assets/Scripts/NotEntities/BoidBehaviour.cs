using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidBehaviour : MonoBehaviour
{
    private const float AlignmentWeight = 0.7f;
    private const float CohesionWeight = 1.5f;
    private const float SeparationWeight = 1.5f;
    private const float ToCenterWeight = 0.12f;
    private const float VisionRadius = 15.0f;
    private const float maxVelocity = 10f;

    public static List<BoidBehaviour> boids = new List<BoidBehaviour>();

    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        boids.Add(this);
    }

    private static Vector3 Separate(Vector3 position, Vector3 otherPosition)
    {
        Vector3 diff = position - otherPosition;
        return diff.normalized * (VisionRadius - diff.magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 alignMove = Vector3.zero;
        Vector3 cohereMove = Vector3.zero;
        Vector3 separateMove = Vector3.zero;

        int counter = 0;

        foreach (BoidBehaviour boid in boids)
        {
            if (boid == this) continue;

            var trans = boid.transform;

            float distance = Vector3.Distance(transform.position, trans.position);
            if (distance < VisionRadius)
            {
                counter++;

                alignMove += trans.forward * AlignmentWeight;

                cohereMove += trans.position;

                separateMove += Separate(transform.position, trans.position) * SeparationWeight;
            }
        }

        //Finish up align, cohere, separate
        if (counter == 0)
        {
            alignMove = transform.forward * AlignmentWeight;
        }
        else
        {
            alignMove /= counter;

            cohereMove /= counter;
            cohereMove -= transform.position;
            cohereMove *= CohesionWeight;
        }

        Vector3 centerMove = -transform.position * ToCenterWeight;

        Vector3 acceleration = alignMove + cohereMove + separateMove + centerMove;

        // Update velocity and position
        velocity += acceleration * Time.deltaTime;

        if (velocity.magnitude > maxVelocity)
        {
            var dir = math.normalize(velocity);
            velocity = dir * maxVelocity;
        }

        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }
}
