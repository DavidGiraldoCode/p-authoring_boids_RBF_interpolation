using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Instance to Render -----------")]
    [SerializeField]
    private int m_numberOfBirds = 50;

    [SerializeField]
    private Boid m_birdPrefab = null; //Passing a refernces to the assets Prefab

    [SerializeField] private float m_spawnRadius = 10;
    [SerializeField] private GameObject spawnPointPrefab;

    [SerializeField]
    private BoxCollider m_bounds = null;

    [SerializeField]
    private float m_boundsForceFactor = 5;

    [Header("Cohesion behaviour -----------")]
    [SerializeField]
    private float m_cohesionForceFactor = 1;
    //TODO Check for this data structure, is it C# specific?
    public float CohesionForceFactor
    {
        get { return m_cohesionForceFactor; }
        set { m_cohesionForceFactor = value; }
    }

    [SerializeField]
    private float m_cohesionRadius = 3;
    public float CohesionRadius
    {
        get { return m_cohesionRadius; }
        set { m_cohesionRadius = value; }
    }
    [Header("Separation behaviour -----------")]
    [SerializeField]
    private float m_separationForceFactor = 1;
    public float SeparationForceFactor
    {
        get { return m_separationForceFactor; }
        set { m_separationForceFactor = value; }
    }

    [SerializeField]
    private float m_separationRadius = 2;
    public float SeparationRadius
    {
        get { return m_separationRadius; }
        set { m_separationRadius = value; }
    }
    [Header("Alignment behaviour -----------")]
    [SerializeField]
    private float m_alignmentForceFactor = 1;
    public float AlignmentForceFactor
    {
        get { return m_alignmentForceFactor; }
        set { m_alignmentForceFactor = value; }
    }

    [SerializeField]
    private float m_alignmentRadius = 3;
    public float AlignmentRadius
    {
        get { return m_alignmentRadius; }
        set { m_alignmentRadius = value; }
    }
    [Header("Movement constrains -----------")]
    [SerializeField]
    private float m_maxSpeed = 8;
    public float MaxSpeed
    {
        get { return m_maxSpeed; }
        set { m_maxSpeed = value; }
    }

    [SerializeField]
    private float m_minSpeed;
    public float MinSpeed
    {
        get { return m_minSpeed; }
        set { m_minSpeed = value; }
    }

    [SerializeField]
    private float m_drag = 0.1f;
    // Flow -----------------------------
    [Header("Flow control -----------")]
    [SerializeField] public bool hasFlow = false;
    //TODO ------------------------------------- Vector Field
    [Header("Vector Field Instance -----------")]
    //[SerializeField] private GameObject vectorField;
    [SerializeField] private GridRenderer gridRenderer;
    [SerializeField] public bool hasVectorField = false;
    //TODO -------------------------------------
    public float Drag
    {
        get { return m_drag; }
        set { m_drag = value; }
    }
    //TODO review
    public float NeighborRadius
    {
        get { return Mathf.Max(m_alignmentRadius, Mathf.Max(m_separationRadius, m_cohesionRadius)); }
    }

    //Creat a getter and setter functions in one line.
    public BoidManager BoidManager { get; set; }
    //TODO review
    public IEnumerable<Boid> SpawnBirds()
    {
        for (int i = 0; i < m_numberOfBirds; ++i)
        {
            Vector3 spawnPoint = spawnPointPrefab != null ? spawnPointPrefab.transform.position + m_spawnRadius * Random.insideUnitSphere : transform.position + m_spawnRadius * Random.insideUnitSphere;

            for (int j = 0; j < 3; ++j)
                spawnPoint[j] = Mathf.Clamp(spawnPoint[j], m_bounds.bounds.min[j], m_bounds.bounds.max[j]);

            Boid boid = Instantiate(m_birdPrefab, spawnPoint, m_birdPrefab.transform.rotation) as Boid; //! IMPORTANT as Boid
            boid.Position = spawnPoint;
            boid.Velocity = Random.insideUnitSphere;
            boid.Flock = this; //Add the instance of THIS flock
            boid.transform.parent = this.transform;
            //Returns the IEnumerable objects
            // yield return Provides the next boid in the iteration
            //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/yield#code-try-1
            yield return boid;
        }
    }
    void Awake()
    {

        gridRenderer = GetComponent<GridRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO double check this math
    public Vector3 GetForceFromBounds(Boid boid)
    {
        Vector3 force = new Vector3();
        Vector3 centerToPos = boid.Position - transform.position;
        Vector3 minDiff = centerToPos + m_bounds.size * 0.5f;
        Vector3 maxDiff = centerToPos - m_bounds.size * 0.5f;
        float friction = 0.0f;

        for (int i = 0; i < 3; ++i)
        {
            if (minDiff[i] < 0)
                force[i] = minDiff[i];
            else if (maxDiff[i] > 0)
                force[i] = maxDiff[i];
            else
                force[i] = 0;

            friction += Mathf.Abs(force[i]);
        }

        force += 0.1f * friction * boid.Velocity;
        return -m_boundsForceFactor * force;
    }

    //Flow
    public bool GetFlow()
    {
        return hasFlow;
    }

    //TODO Vector Field
    public bool HasVectorField()
    {
        return hasVectorField;
    }

    public Vector3 GetForceFromVectorField(Boid boid)
    {
        if (gridRenderer)
        {

            Vector3 projectedFlight = new Vector3(boid.Position.x, 0, boid.Position.z);
            //Debug.Log("Interpolating at " + projectedFlight);
            //? Notice, callng GetComponent returns a reference to the component of a given GameObject
            //? BUT, in this case the GridRenderer is empty. Is not retuning the current intance on the game ar Runtime
            //? With the data it holds
            //Vector3 force = vectorField.GetComponent<GridRenderer>().InterpolateVector(projectedFlight);
            //Debug.Log("RESULT " + force);
            Vector3 force = gridRenderer.InterpolateVector(boid.Position) * 20f;
            return force;
        }
        return Vector3.zero;
    }

}
