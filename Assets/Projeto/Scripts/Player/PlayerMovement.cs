using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody2D rb;
    private Vector2 InputMoviment;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Movimento (WASD)
        InputMoviment.x = Input.GetAxisRaw("Horizontal");
        InputMoviment.y = Input.GetAxisRaw("Vertical");

        InputMoviment = InputMoviment.normalized;

    }

    void FixedUpdate()
    {
        // Aplica a movimentação física
        rb.MovePosition(rb.position + InputMoviment * velocidade * Time.fixedDeltaTime);

    }
}
