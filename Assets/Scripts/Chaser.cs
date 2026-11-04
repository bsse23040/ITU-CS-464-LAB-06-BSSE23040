using UnityEngine;

/// <summary>
/// A small enemy from the wave spawner. It walks straight at the player, and it gives up after a while.
/// Its Hazard destroys it when the hit lands.
/// </summary>
public class Chaser : MonoBehaviour
{
    [SerializeField] float speed = 2.6f;
    [SerializeField] float lifetime = 12f;     // seconds, so a wave always ends

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Vector3 target = player.position;
        target.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
