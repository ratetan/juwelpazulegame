using UnityEngine;

public class GemBehavior : MonoBehaviour
{
    private GameManager manager;
    public bool hasTouchedSomething = false;

    void Start()
    {
        manager = Object.FindFirstObjectByType<GameManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Gem"))
        {
            manager.HandleGemCollision(gameObject, collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Gem") || collision.gameObject.CompareTag("Ground"))
        {
            hasTouchedSomething = true;
        }
    }
}
