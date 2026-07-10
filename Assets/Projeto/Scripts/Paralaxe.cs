    using UnityEngine;

public class Paralaxe : MonoBehaviour
{
     [SerializeField] private float speed = 3.0f;
    private float spriteWidth;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        // Grab the exact width of the sprite in world units
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move the sprite to the left
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // If the sprite has completely passed its own width, snap it back
        if (transform.position.x <= startPosition.x - spriteWidth)
        {
            transform.position = new Vector3(startPosition.x + spriteWidth, transform.position.y, transform.position.z);
        }
    }
}
