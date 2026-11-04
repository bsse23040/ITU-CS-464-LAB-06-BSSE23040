using System.Collections;
using UnityEngine;

/// <summary>
/// The ball. Input is read in Update, the push is applied in FixedUpdate (Lecture 06),
/// and BoostSpeed gives a speed power-up whose timer lives here, on the player (Lecture 07).
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 10f;

    Rigidbody rb;
    Vector3 input;
    float speedMultiplier = 1f;
    Coroutine boost;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);   // diagonals are not faster
    }

    void FixedUpdate() => rb.AddForce(input * moveForce * speedMultiplier);

    // A second pickup stops the running boost and starts a new one, so the newest pickup wins.
    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }
}
