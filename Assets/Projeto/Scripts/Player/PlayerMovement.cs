using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region VARIAVEIS: CONFIGURAÇÕES
    [Header("Movimentacao")] [Tooltip("Velocidade de movimento do jogador.")]
    public float moveSpeed = 5f;
    #endregion

    #region VARIAVEIS: CANVAS
    [Header("Efeitos Visuais de UI")] [Tooltip("Arraste o objeto do Canvas que possui o script AparecerUI aqui.")]
    public GerenciadorInterface scriptFadeUI;
    #endregion

    #region VARIAVEIS: REFERÊNCIAS INTERNAS
    private Rigidbody2D rb;                         // Guarda o rigidbody2d do jogador
    private Camera mainCam;                         // Guarda a referência da câmera
    private PlayerControls controls;                // classe gerada pelo inputsystem
    private IInteractable objetoInteragivelAtual;   // Guarda quem está perto do player
    private Animator animator;                         // Guarda a referência do Animator do player
    #endregion

    #region VARIAVEIS: DADOS DE ENTRADA (INPUTS)
    private Vector2 moveInput;                      // Movimento WASD
    private Vector2 mousePos;                       // Posição do mouse
    #endregion

    #region INICIALIZAÇÃO
    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        rb.WakeUp();
        mainCam = Camera.main;
        animator = GetComponent<Animator>();
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
        controls.Player.Look.performed += ctx => mousePos = ctx.ReadValue<Vector2>();

        // Quando apertar o botão de interagir, chama a função "TentarInteragir"
        controls.Player.Interact.performed += ctx => TentarInteragir();

    }
    void OnDisable()
    {
        controls.Disable();
        moveInput = Vector2.zero; 
    }

    #endregion

    private bool canMove = true;

   //public void SetMovementEnabled(bool enabled)
   //{
   //    canMove = enabled;
   //    if (!canMove)
   //        moveInput = Vector2.zero;
   //    else
   //    {
   //        if (controls != null)
   //        {
   //            moveInput = controls.Player.Move.ReadValue<Vector2>();
   //            mousePos = controls.Player.Look.ReadValue<Vector2>();
   //        }
   //        else
   //        {
   //            moveInput = Vector2.zero;
   //        }
   //    }
   //}

    #region ATUALIZAÇÃO DA FÍSICA
    void FixedUpdate()
    {
        ProcessarMovimento();
        ProcessarRotacao();
    }

    private void ProcessarMovimento()
    {
        if (!canMove)
            return;

        Vector2 novaPosicao = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(novaPosicao);
        bool isMoving = moveInput.sqrMagnitude > 0.001f;
        animator.SetBool("IsMoving", isMoving);
    }

    private void ProcessarRotacao()
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);

        Vector2 direcaoOlhar = mouseWorldPos - rb.position;
        float anguloGiro = Mathf.Atan2(direcaoOlhar.y, direcaoOlhar.x) * Mathf.Rad2Deg;

        rb.rotation = anguloGiro;
    }
    #endregion

    #region LOGICA DE INTERAÇÃO
    private void TentarInteragir()
    {
        if (objetoInteragivelAtual != null)
        {
            IInteractable itemParaInteragir = objetoInteragivelAtual;
            objetoInteragivelAtual = null;

            // Esconde o "E" da tela imediatamente assim que o jogador interagir
            if (GerenciadorInterface.Instancia != null)
                GerenciadorInterface.Instancia.EsconderBotaoE();

            itemParaInteragir.Interagir();
        }
    }
    #endregion

    #region DETECCAO POR PROXIMIDADE
    void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            objetoInteragivelAtual = interactable;

            // Faz o "E" aparecer na tela via Fade quando o player chega perto
            if (GerenciadorInterface.Instancia != null)
                GerenciadorInterface.Instancia.MostrarBotaoE();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == objetoInteragivelAtual)
        {
            objetoInteragivelAtual = null;

            // Faz o "E" sumir da tela via Fade se o player passar direto e se afastar
            if (GerenciadorInterface.Instancia != null)
                GerenciadorInterface.Instancia.EsconderBotaoE();
        }
    }
    #endregion
}
