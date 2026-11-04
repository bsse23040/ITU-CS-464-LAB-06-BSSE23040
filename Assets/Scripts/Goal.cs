using UnityEngine;

/// <summary>
/// The exit pad. It stays grey until the GameManager says the goal is open, then it turns green.
/// </summary>
public class Goal : MonoBehaviour
{
    [SerializeField] Renderer pad;
    [SerializeField] Material lockedMaterial, openMaterial;

    bool shownOpen;

    void Start() => pad.sharedMaterial = lockedMaterial;

    void Update()
    {
        bool open = GameManager.Instance.GoalOpen;
        if (open == shownOpen) return;
        shownOpen = open;
        pad.sharedMaterial = open ? openMaterial : lockedMaterial;
    }

    // Stay, not Enter: if the exit opens while the player is already standing on it, it still counts.
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.GoalReached();
    }
}
