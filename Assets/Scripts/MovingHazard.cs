using UnityEngine;

/// <summary>
/// A hazard that slides back and forth on a steady rhythm, so the player can learn the timing.
/// It sits on a kinematic Rigidbody and is moved with MovePosition in FixedUpdate.
/// </summary>
public class MovingHazard : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.right;
    [SerializeField] float distance = 10f;    // how far it travels in total
    [SerializeField] float speed = 3f;
    [SerializeField] float offset = 0f;       // starts part-way through the cycle, so two sweepers can take turns

    Rigidbody rb;
    Vector3 centre;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        centre = rb.position;
    }

    void FixedUpdate()
    {
        float along = Mathf.PingPong(Time.time * speed + offset, distance) - distance / 2f;
        rb.MovePosition(centre + direction.normalized * along);
    }
}
