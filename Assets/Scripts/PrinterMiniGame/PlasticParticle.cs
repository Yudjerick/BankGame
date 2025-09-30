using UnityEngine;

public class PlasticParticle : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.rigidbody is null || collision.rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.angularVelocity = 0;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
