using UnityEngine;

[RequireComponent(typeof(Boid))] //! IMPORTANT
//This allows the Flock class to instance the Prefab as type Boid 
public class BirdRenderer : MonoBehaviour
{
    private Boid m_boid = null; //Private referenc to the boid
    void Start()
    {
        m_boid = GetComponent<Boid>();
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

    }
}
