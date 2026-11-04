using UnityEngine;

public enum PowerUpType { Speed, Shield }

/// <summary>
/// A pickup that changes the rules for a while. It only asks the player to do the work:
/// the boost's timer has to live on the player, because the pickup is destroyed right away.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType type = PowerUpType.Speed;
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;
    [SerializeField] float spinSpeed = 60f;

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (type == PowerUpType.Speed)
            other.GetComponent<PlayerController>().BoostSpeed(multiplier, duration);
        else
            other.GetComponent<Health>().shielded = true;

        GameManager.Instance.UpdateUI();
        Destroy(gameObject);
    }
}
