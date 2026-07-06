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
    public AparecerUI scriptFadeUI;
    #endregion

    #region VARIAVEIS: REFERÊNCIAS INTERNAS
    private Rigidbody2D rb;                         // Guarda o rigidbody2d do jogador
    private Camera mainCam;                         // Guarda a referência da câmera
    private PlayerControls controls;                // classe gerada pelo inputsystem
    private IInteractable objetoInteragivelAtual;   // Guarda quem está perto do player
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

        // Quando apertar o botão de interagir, chama a função "TentarInteragir"
        controls.Player.Interact.performed += ctx => TentarInteragir();
    }
    void OnDisable()
    {
        controls.Disable();
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

    #region LOGICA DE INTERAÇÃO
    private void TentarInteragir()
    {
        if (objetoInteragivelAtual != null)
        {
            // guardamos a referência do item em uma variável local temporária
            IInteractable itemParaInteragir = objetoInteragivelAtual;

            objetoInteragivelAtual = null;

            // forçamos o Canvas a sumir na hora, sem depender da física
            if (scriptFadeUI != null)
            {
                scriptFadeUI.AtivarFadeOut();
            }

            // por último, chamamos a interação que vai destruir o item
            itemParaInteragir.Interagir();
        }
    }
    #endregion

    #region DETECCAO POR PROXIMIDADE
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que interagimos tem o componente de Interação
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            objetoInteragivelAtual = interactable;
            Debug.Log("Perto de um objeto interagível. Aperte o botão de interação!");

            //Ativa o Fade In do "E" na tela
            if (scriptFadeUI != null)
            {
                scriptFadeUI.AtivarFadeIn();
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable == objetoInteragivelAtual)
        {
            objetoInteragivelAtual = null;
            Debug.Log("Se afastou do objeto.");

            // Ativa o Fade Out do "E" para sumir da tela
            if (scriptFadeUI != null)
            {
                scriptFadeUI.AtivarFadeOut();
            }
        }
    }
    #endregion
}