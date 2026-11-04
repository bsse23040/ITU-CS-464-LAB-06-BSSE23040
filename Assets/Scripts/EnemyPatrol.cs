using UnityEngine;

public enum PatrolMode { Loop, PingPong, Random }

/// <summary>
/// A patrol is a list of points and a counter. The guard walks to points[index], and when it arrives
/// NextIndex() picks the next one. The waypoints are empty GameObjects placed in the scene.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] float speed = 2.6f;
    [SerializeField] float waitTime = 0f;       // a pause at each point
    [SerializeField] float turnSpeed = 360f;    // degrees per second

    int index;
    int step = 1;
    float waitUntil;

    public Vector3 Target => points[index].position;
    public int Arrivals { get; private set; }   // how many points it has reached so far

    void Update()
    {
        if (Time.time < waitUntil) return;

        Vector3 target = points[index].position;
        Vector3 toTarget = target - transform.position;

        // MoveTowards lands exactly on the target, so == is safe here
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (toTarget != Vector3.zero)           // LookRotation of a zero vector gives a warning
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);

        if (transform.position == target)
        {
            Arrivals++;
            index = NextIndex();
            waitUntil = Time.time + waitTime;
        }
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop)
            return (index + 1) % points.Length;

        if (mode == PatrolMode.PingPong)
        {
            if (index + step < 0 || index + step >= points.Length) step = -step;   // turn round at either end
            return index + step;
        }

        int next = Random.Range(0, points.Length);                                  // never the same point twice
        return next == index ? (next + 1) % points.Length : next;
    }

    // after a chase: walk back to the closest point of the route and carry on from there
    public void ResumeFromNearest()
    {
        float best = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float d = (points[i].position - transform.position).sqrMagnitude;
            if (d < best) { best = d; index = i; }
        }
        waitUntil = 0f;
    }
}
