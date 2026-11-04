using UnityEngine;

/// <summary>
/// A collectible. It spins, and when the player touches it, it reports to the GameManager and removes itself.
/// The collider on the prefab has Is Trigger ticked, so the ball passes through it.
/// </summary>
public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 90f;   // degrees per second

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.AddScore(value);
        Destroy(gameObject);                  // gameObject, not this: Destroy(this) would remove only the script
    }
}
