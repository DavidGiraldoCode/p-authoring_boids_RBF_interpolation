using UnityEngine;

[RequireComponent(typeof(Boid))] //! IMPORTANT
//This allows the Flock class to instance the Prefab as type Boid 
public class BirdRenderer : MonoBehaviour
{
    private Boid m_boid = null; //Private referenc to the boid
    private Animator m_animator;
    private float m_wingsFlappingSpeed;

    void Start()
    {
        m_boid = GetComponent<Boid>();
        m_animator = GetComponentInChildren<Animator>();
        Debug.Log(m_animator);
        m_wingsFlappingSpeed = Random.Range(1.5f, 2.5f);
    }
    public float AnimationSpeed
    {
        get => m_animator.speed;
        set => m_animator.speed = value;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO Double check this algorithm's math
        Vector3 velocity = m_boid.Velocity;
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 6);
        }

        transform.position = m_boid.Position;
        transform.position += new Vector3(0, 0, transform.parent.position.z); //inherit z-position

        if (m_animator != null) m_animator.speed = m_wingsFlappingSpeed - (velocity.magnitude * 0.06f);
    }
}
