using UnityEngine;

/// <summary>
/// A guard with a brain: Patrol, Chase and Return, a small state machine from Lecture 07.
/// It patrols with the EnemyPatrol next to it, and switches that script off while it chases.
/// </summary>
[RequireComponent(typeof(EnemyPatrol))]
public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    [SerializeField] float chaseSpeed = 3.4f;     // slower than the ball, so a guard can be outrun
    [SerializeField] float sightRange = 7f;
    [SerializeField] float loseRange = 10f;       // it keeps chasing a little further than it first saw you
    [SerializeField] float sightAngle = 70f;      // half the field of view, in degrees
    [SerializeField] float turnSpeed = 360f;

    State state = State.Patrol;
    EnemyPatrol patrol;
    Transform player;
    int arrivalsAtReturn;

    void Awake()
    {
        patrol = GetComponent<EnemyPatrol>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                if (CanSee(sightRange, sightAngle)) StartChase();
                break;

            case State.Chase:
                Chase();
                if (!CanSee(loseRange, 180f)) StartReturn();
                break;

            case State.Return:
                if (CanSee(sightRange, sightAngle)) StartChase();
                else if (patrol.Arrivals > arrivalsAtReturn) state = State.Patrol;   // back on the route
                break;
        }
    }

    void StartChase()
    {
        state = State.Chase;
        patrol.enabled = false;
    }

    void StartReturn()
    {
        state = State.Return;
        patrol.enabled = true;
        patrol.ResumeFromNearest();
        arrivalsAtReturn = patrol.Arrivals;
    }

    void Chase()
    {
        Vector3 target = player.position;
        target.y = transform.position.y;                       // stay on the floor
        Vector3 toTarget = target - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
    }

    // in range, inside the field of view, and nothing solid in between (Lecture 05's sight test)
    bool CanSee(float range, float angle)
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > range) return false;

        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (Vector3.Angle(transform.forward, flat) > angle) return false;

        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, range,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");
        return false;
    }
}
