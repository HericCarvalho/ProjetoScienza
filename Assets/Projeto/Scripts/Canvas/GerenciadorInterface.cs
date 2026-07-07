using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GerenciadorInterface : MonoBehaviour
{
    public static GerenciadorInterface Instancia { get; private set; }

    [Header("Configurações Gerais")]
    public float velocidadeFade = 8f;

    [Header("Sistema de Notificação")]
    public CanvasGroup grupoNotificacao;
    public TextMeshProUGUI textoNotificacao;
    public Image iconeNotificacao;
    public float tempoExibicaoNotificacao = 2.5f;

    [Header("Indicador de Interação [E]")]
    public CanvasGroup grupoIndicadorE; 

    [Header("MENU MOCHILA / REPERTÓRIO")]
    public CanvasGroup grupoMenuRepertorio;
    public ReferenciaData[] todasAsReferenciasDoJogo;
    public Transform containerItens;
    public GameObject prefabSlotItem;

    private PlayerControls controls;
    private bool menuAberto = false;
    private Coroutine coroutineNotificacao;
    private Coroutine coroutineMenu;
    private Coroutine coroutineFadeE;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else { Destroy(gameObject); return; }

        controls = new PlayerControls();

        // Inicializa os estados visuais zerados
        if (grupoNotificacao != null) grupoNotificacao.alpha = 0f;
        if (grupoMenuRepertorio != null)
        {
            grupoMenuRepertorio.alpha = 0f;
            grupoMenuRepertorio.blocksRaycasts = false;
        }
    }

    void OnEnable()
    {
        if (controls != null)
        {
            controls.Enable();
            controls.Player.OpenRepertorio.performed += OnOpenRepertorioPerformed;
        }
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.OpenRepertorio.performed -= OnOpenRepertorioPerformed;
            controls.Disable();
        }
    }

    #region CONTROLE DO MENU DE REPERTÓRIO
    private void OnOpenRepertorioPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            AlternarMochila();
        }
    }
    private void AlternarMochila()
    {
        menuAberto = !menuAberto;

        if (menuAberto)
        {
            AtualizarVisualDoRepertorio();
            ForçarFade(ref coroutineMenu, grupoMenuRepertorio, 1f, true); // Usando Unscaled Time (Corrige o bug do 0.03)
            grupoMenuRepertorio.blocksRaycasts = true;
            Time.timeScale = 0f;
        }
        else
        {
            ForçarFade(ref coroutineMenu, grupoMenuRepertorio, 0f, true);
            grupoMenuRepertorio.blocksRaycasts = false;
            Time.timeScale = 1f;
        }
    }
    private void AtualizarVisualDoRepertorio()
    {
        // Limpa os slots antigos
        foreach (Transform child in containerItens) { Destroy(child.gameObject); }

        // Vasculha a lista global de itens
        foreach (ReferenciaData refData in todasAsReferenciasDoJogo)
        {
            if (refData == null) continue;

            // Checa se o jogador possui o item
            if (RepertorioDados.Instancia != null && RepertorioDados.Instancia.JaPossuiReferencia(refData.idUnico))
            {
                // Cria o slot na tela
                GameObject slotObj = Instantiate(prefabSlotItem, containerItens);

                // Pega o componente do nosso novo script do Slot
                SlotMochila slotScript = slotObj.GetComponent<SlotMochila>();

                if (slotScript != null && slotScript.imagemDoIcone != null)
                {
                    // Alimenta os dados direto nos campos que você arrastou no Inspector
                    slotScript.imagemDoIcone.sprite = refData.iconeUI;
                    slotScript.textoDoNome.text = refData.nomeExibicao;

                    // Força a cor para branco com Alpha 1 para garantir que não fique transparente
                    slotScript.imagemDoIcone.color = Color.white;
                }
            }
        }
    }
    #endregion

    #region CONTROLE DO BOTÃO [E]
    // Chamado pelo Player ao chegar perto de um item
    public void MostrarBotaoE()
    {
        ForçarFade(ref coroutineFadeE, grupoIndicadorE, 1f, false);
    }

    // Chamado pelo Player ao se afastar ou interagir com o item
    public void EsconderBotaoE()
    {
        ForçarFade(ref coroutineFadeE, grupoIndicadorE, 0f, false);
    }
    #endregion

    #region LÓGICA DE NOTIFICAÇÃO
    public void MostrarNotificacao(string nomeDoItem, Sprite spriteDoIcone = null)
    {
        if (coroutineNotificacao != null) StopCoroutine(coroutineNotificacao);
        coroutineNotificacao = StartCoroutine(FluxoNotificacao(nomeDoItem, spriteDoIcone));
    }

    private IEnumerator FluxoNotificacao(string nomeDoItem, Sprite spriteDoIcone)
    {
        textoNotificacao.text = $"Você encontrou: {nomeDoItem}!";

        if (spriteDoIcone != null && iconeNotificacao != null)
        {
            iconeNotificacao.gameObject.SetActive(true);
            iconeNotificacao.sprite = spriteDoIcone;
        }
        else if (iconeNotificacao != null)
        {
            iconeNotificacao.gameObject.SetActive(false);
        }

        yield return FadeElemento(grupoNotificacao, 1f, false);

        float contador = 0;
        while (contador < tempoExibicaoNotificacao)
        {
            contador += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return FadeElemento(grupoNotificacao, 0f, false);
    }
    #endregion

    #region AUXILIARES DE FADE
    private void ForçarFade(ref Coroutine c, CanvasGroup g, float target, bool usarUnscaledTime)
    {
        if (c != null) StopCoroutine(c);
        c = StartCoroutine(FadeElemento(g, target, usarUnscaledTime));
    }

    private IEnumerator FadeElemento(CanvasGroup grupo, float targetAlpha, bool usarUnscaledTime)
    {
        if (grupo == null) yield break;

        while (!Mathf.Approximately(grupo.alpha, targetAlpha))
        {
            float delta = usarUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            grupo.alpha = Mathf.MoveTowards(grupo.alpha, targetAlpha, velocidadeFade * delta);
            yield return null;
        }
        grupo.alpha = targetAlpha;
    }
    #endregion
}