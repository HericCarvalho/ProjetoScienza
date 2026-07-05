using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region VARIÁVEIS: CONFIGURAÇÕES
    [Header("Movimentação")] [Tooltip("Velocidade de movimento do jogador.")]
    public float moveSpeed = 5f;
    #endregion
    #region VARIÁVEIS: REFERÊNCIAS INTERNAS
    private Rigidbody2D rb;          // Guarda o rigidbody2d do jogador
    private Camera mainCam;          // Guarda a referência da câmera
    private PlayerControls controls; // classe gerada pelo inputsystem
    #endregion

    #region VARIÁVEIS: DADOS DE ENTRADA (INPUTS)
    private Vector2 moveInput;       // Movimento WASD
    private Vector2 mousePos;        // Posição do mouse
    #endregion

    #region INICIALIZAÇÃO
    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }
    void OnEnable()
    {
        // Ativa os Botoes
        controls.Enable();

        // Evento de movimento WASD
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // Quando o jogador solta as teclas, zera o vetor de movimento para o personagem parar
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Evento de movimento do mouse
        controls.Player.Look.performed += ctx => mousePos = mainCam.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
    }

    #endregion

    #region ATUALIZAÇÃO DA FÍSICA
    void FixedUpdate()
    {
        ProcessarMovimento();
        ProcessarRotacao();
    }

    private void ProcessarMovimento()
    {
        Vector2 novaPosicao = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(novaPosicao);
    }

    private void ProcessarRotacao()
    {
        Vector2 direcaoOlhar = mousePos - rb.position;
        float anguloGiro = Mathf.Atan2(direcaoOlhar.y, direcaoOlhar.x) * Mathf.Rad2Deg - 90f;

        rb.rotation = anguloGiro;
    }
    #endregion
}