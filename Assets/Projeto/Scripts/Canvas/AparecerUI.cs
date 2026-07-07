using System.Collections;
using UnityEngine;
using TMPro;

public class AparecerUI : MonoBehaviour
{
    public static AparecerUI Instancia { get; private set; } // Permite que qualquer script acesse o gerenciador facilmente

    [Header("Configuracoes de Velocidade")] [Tooltip("Quanto maior o valor, mais rapido sera o Fade In.")]
    public float velocidadeFade = 5f;

    [Header("NOTIFICACAO: Componentes de UI")] [Tooltip("Componentes de UI que aparecem na notificação.")]
    public TextMeshProUGUI textoNotificacao;

    [Header("NOTIFICACAO: Configurações do Efeito")] [Tooltip("Configuracoes do efeito de notificação.")]
    public float velocidadeFadeNotificacao = 4f;
    public float tempoExibicaoNotificacao = 3f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private Coroutine notificacaoCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Comeca invisível
    }
    #region FUNÇÕES DE FADE
    public void AtivarFadeIn()
    {
        ReiniciarCoroutine(Fade(1f));
    }

    public void AtivarFadeOut()
    {
        ReiniciarCoroutine(Fade(0f));
    }

    private void ReiniciarCoroutine(IEnumerator novoFade)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(novoFade);
    }
    private IEnumerator Fade(float targetAlpha)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            // Move o alpha em direção ao objetivo (0 ou 1) de forma suave
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, velocidadeFade * Time.deltaTime);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
    #endregion

    #region FUNÇÕES DE NOTIFICAÇÃO
    public void MostrarNotificacao(string nomeDoItem)
    {
        // Se já tiver uma notificação passando, para ela para começar a nova
        if (notificacaoCoroutine != null) StopCoroutine(notificacaoCoroutine);

        // Inicia o processo de aparecer, esperar e sumir
        notificacaoCoroutine = StartCoroutine(FluxoNotificacao(nomeDoItem));
    }

    private IEnumerator FluxoNotificacao(string nomeDoItem)
    {
        textoNotificacao.text = "Você descobriu essa referência!"; nomeDoItem.ToUpper(); // Deixa em maiúsculo

        // FADE IN
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += velocidadeFade * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Guenta a xereka
        yield return new WaitForSeconds(tempoExibicaoNotificacao);

        // FADE OUT
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= velocidadeFadeNotificacao * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
    #endregion
}