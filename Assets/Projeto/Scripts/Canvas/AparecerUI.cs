using System.Collections;
using UnityEngine;

public class AparecerUI : MonoBehaviour
{
    [Header("Configuracoes de Velocidade")] [Tooltip("Quanto maior o valor, mais rapido sera o Fade In.")]
    public float velocidadeFade = 5f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Comeca invisível
    }

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

    // Corotina unica que serve tanto para subir quanto para descer o alpha
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

}