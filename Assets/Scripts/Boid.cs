using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Boid : MonoBehaviour
{
    //Create a reference to an intance type Flock
    public Flock Flock { get; set; }

    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;
    void Start()
    {
        //TODO review
        Velocity = Random.insideUnitSphere * 2;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Custom Methods
    public void UpdateSimulation(float deltaTime)
    {
        //Clear acceleration from last frame
        Acceleration = Vector3.zero;

        //Apply forces
        Acceleration += Flock.GetForceFromBounds(this);
        Acceleration += GetConstraintSpeedForce();
        Acceleration += GetSteeringForce();
        if (Flock.GetFlow())
            Acceleration += GetStationaryFlowForce();

        //TODO -------- Interpolation with the Vector Field
        if (Flock.HasVectorField())
            Acceleration += Flock.GetForceFromVectorField(this);

        //Step simulation
        Velocity += deltaTime * Acceleration;
        Position += 0.5f * deltaTime * deltaTime * Acceleration + deltaTime * Velocity;

        //? Visualizing projected flight
        ProjectFlightOntoVectorField();
    }

    //Internal computation of the forces:

    Vector3 GetSteeringForce()
    {
        Vector3 cohesionForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 separationForce = Vector3.zero;

        //Average velocity
        Vector3 velocityAccumulador = Vector3.zero;
        Vector3 averageVelocity = Vector3.zero;
        //Average position
        Vector3 positionAccumulador = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;
        //Boid forces
        //The iteration happens on a collection IEnumerable<Boid>
        foreach (Boid neighbor in Flock.BoidManager.GetNeighbors(this, Flock.NeighborRadius))
        {
            float distance = (neighbor.Position - Position).magnitude;

            //Separation force
            if (distance < Flock.SeparationRadius)
            {
                separationForce += Flock.SeparationForceFactor * ((Flock.SeparationRadius - distance) / distance) * (Position - neighbor.Position);
            }

            //TODO Calculate average position/velocity here
            //Aerage velocity
            if (distance < Flock.AlignmentRadius)
            {
                velocityAccumulador += neighbor.Velocity;
            }

            //Aerage velocity
            if (distance < Flock.CohesionRadius)
            {
                positionAccumulador += neighbor.Position;
            }

        }

        //Set cohesion/alignment forces here
        averageVelocity = velocityAccumulador / Flock.BoidManager.GetNeighborsCount();
        alignmentForce = Flock.AlignmentForceFactor * (averageVelocity - Velocity);

        averagePosition = positionAccumulador / Flock.BoidManager.GetNeighborsCount();
        cohesionForce = Flock.CohesionForceFactor * (averagePosition - Position);

        return alignmentForce + cohesionForce + separationForce;
    }

    Vector3 GetConstraintSpeedForce()
    {
        Vector3 force = Vector3.zero;

        //Apply drag
        force -= Flock.Drag * Velocity;

        float vel = Velocity.magnitude;
        if (vel > Flock.MaxSpeed)
        {
            //If speed is above the maximum allowed speed, apply extra friction force
            force -= (20.0f * (vel - Flock.MaxSpeed) / vel) * Velocity;
        }
        else if (vel < Flock.MinSpeed)
        {
            //Increase the speed slightly in the same direction if it is below the minimum
            force += (5.0f * (Flock.MinSpeed - vel) / vel) * Velocity;
        }

        return force;
    }

    //Flow force
    Vector3 GetStationaryFlowForce()
    {
        Vector3 flow = Vector3.zero;
        Vector3 flowForceLocationOne = new Vector3(0, 4, 0);
        Vector3 flowForceLocationTwo = new Vector3(0, -4, 0);
        float flowRadius = 3f;

        //Flow One
        if ((flowForceLocationOne - Position).magnitude < flowRadius)
        {
            //Debuging
            Debug.DrawLine(flowForceLocationOne, Position, Color.grey);
            flow = Vector3.up * 40f;
            return flow;
        }

        //Flow Two
        if ((flowForceLocationTwo - Position).magnitude < flowRadius)
        {
            //Debuging
            Debug.DrawLine(flowForceLocationTwo, Position, Color.grey);
            flow = Vector3.down * 40f;
            return flow;
        }

        return flow;
    }

    //? HELPER METHODS to allow visualizing the projected position of the boid

    private void ProjectFlightOntoVectorField()
    {
        Vector3 projectedPosition = new Vector3(Position.x, 0, Position.z);
        Vector3 boidVFSample = Flock.GetForceFromVectorField(this);
        //Debug.Log("boidVFSample: " + boidVFSample);
        Vector3 directionOnVF = projectedPosition + (boidVFSample * 0.01f);
        //Vector3 projectedXZVelocity = new Vector3(Velocity.x, 0, Velocity.z);
        Debug.DrawLine(Position, projectedPosition, Color.black);
        Debug.DrawLine(projectedPosition, directionOnVF, Color.green);
    }
}
