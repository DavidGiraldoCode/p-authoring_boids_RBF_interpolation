using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    // Container of boids
    //References: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic?view=net-8.0
    private List<Boid> m_boids;
    void Start()
    {
        //TODO separation of concerns, create a new method to create boids
        m_boids = new List<Boid>();

        //Get ALL the instances of the Flock class on the scene.
        var flocks = GameObject.FindObjectsOfType<Flock>();

        foreach (var flock in flocks)
        {
            //For each flock instance, add a reference to THIS manager
            flock.BoidManager = this;


            //? void List<Boid>.AddRange(IEnumerable<Boid> collection)
            //Add a collection of Objects(Boid) to the List<T>, it uses the 
            //IEnumerable<T> interface.
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

    //* IEnumerable<T>
    /*
    IEnmerable is an interface that makes enables iteration over a collection,
    by exposing the enumerator. Here, GetNeighbors is retunrning a collection
    of all the boids that fit the condition of being a neighbor of the current boid
    */
    //TODO check for yield 
    //TODO check if boids from flock A and change to flock B
    public IEnumerable<Boid> GetNeighbors(Boid boid, float radius)
    {
        float radiusSq = radius * radius;
        foreach (var other in m_boids)
        {
            if (other != boid && (other.Position - boid.Position).sqrMagnitude < radiusSq)
                yield return other; //The yield return is providing the collection with the next boid in the interation
        }
    }

    public int GetNeighborsCount()
    {
        return m_boids.Count;
    }
}
