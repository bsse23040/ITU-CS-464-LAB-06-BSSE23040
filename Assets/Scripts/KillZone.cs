using UnityEngine;

/// <summary>
/// Lava. It skips the hearts: touching it costs a life at once.
/// </summary>
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.PlayerDied();
    }
}
