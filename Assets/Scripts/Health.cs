using UnityEngine;

/// <summary>
/// The hearts. Health owns the hearts and the rule about how often they can be lost;
/// the GameManager owns the lives.
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;      // seconds of safety after a hit
    public bool shielded;                     // public on purpose: the shield power-up sets it

    int current;
    float invulnerableUntil;
    Renderer body;

    public int Current => current;
    public int Max => max;

    void Awake()
    {
        current = max;
        body = GetComponentInChildren<Renderer>();
    }

    // the ball blinks while it cannot be hurt
    void Update()
    {
        bool hidden = Time.time < invulnerableUntil && Mathf.FloorToInt(Time.time * 10f) % 2 == 0;
        body.enabled = !hidden;
    }

    // Returns true if the hit landed. The hazard uses that to decide on knockback.
    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        if (GameManager.Instance.IsRespawning) return false;

        invulnerableUntil = Time.time + iFrames;
        if (shielded)
        {
            shielded = false;                    // the shield takes this hit
            GameManager.Instance.UpdateUI();
            return false;
        }

        current -= amount;
        GameManager.Instance.UpdateUI();
        if (current <= 0) GameManager.Instance.PlayerDied();   // <= 0, not == 0: the crusher hits for 2
        return true;
    }

    // called by the GameManager when the player respawns
    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        GameManager.Instance.UpdateUI();
    }
}
