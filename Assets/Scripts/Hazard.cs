using System.Collections;
using UnityEngine;

/// <summary>
/// A trigger that hurts. One script is every hazard in the game: spikes, guards, the crusher and the sweepers.
/// With a restTime above 0 it becomes a timed hazard: rest, warn, strike, and round again.
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] bool destroyOnHit;       // for the wave chasers: they spend themselves on the hit

    [Header("Timed hazards (restTime 0 = always on)")]
    [SerializeField] float restTime = 0f;
    [SerializeField] float warnTime = 0.7f;   // reaction time is about 0.25 s, so 0.7 s is fair
    [SerializeField] float strikeTime = 0.4f;
    [SerializeField] float startDelay = 0f;   // so two crushers do not strike together
    [SerializeField] GameObject warning;      // shown during the warning
    [SerializeField] GameObject slab;         // shown during the strike

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;
        Collider zone = GetComponent<Collider>();
        zone.enabled = false;
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            zone.enabled = false;
            Show(slab, false);
            yield return new WaitForSeconds(restTime);
            Show(warning, true);
            yield return new WaitForSeconds(warnTime);
            Show(warning, false);
            zone.enabled = true;
            Show(slab, true);
            yield return new WaitForSeconds(strikeTime);
        }
    }

    static void Show(GameObject thing, bool on)
    {
        if (thing != null) thing.SetActive(on);
    }

    // OnTriggerStay, not Enter: standing on spikes keeps hurting. Health decides how often (its i-frames).
    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health)) return;
        if (!health.TakeDamage(damage)) return;           // only a hit that lands knocks back

        Rigidbody body = other.attachedRigidbody;
        if (body != null)
        {
            Vector3 away = body.position - transform.position;
            away.y = 0f;                                  // along the floor, never into the air
            body.AddForce(away.normalized * knockback, ForceMode.Impulse);
        }
        if (destroyOnHit) Destroy(gameObject);
    }
}
