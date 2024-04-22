using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    // Container of boids
    private List<Boid> m_boids;
    void Start()
    {
        m_boids = new List<Boid>();

        //Get ALL the instances of the Flock class on the scene.
        var flocks = GameObject.FindObjectsOfType<Flock>();

        foreach (var flock in flocks)
        {
            //For each flock instance, add a reference to THIS manager
            flock.BoidManager = this;


            //? void List<Boid>.AddRange(IEnumerable<Boid> collection)
            m_boids.AddRange(flock.SpawnBirds());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime);
        }
    }

    //TODO check for IEnumerable
    //TODO check for yield 
    public IEnumerable<Boid> GetNeighbors(Boid boid, float radius)
    {
        float radiusSq = radius * radius;
        foreach (var other in m_boids)
        {
            if (other != boid && (other.Position - boid.Position).sqrMagnitude < radiusSq)
                yield return other;
        }
    }

    public int GetNeighborsCount()
    {
        return m_boids.Count;
    }
}
