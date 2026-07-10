using UnityEngine;

public class PecaFeedback : MonoBehaviour
{
    [Header("Configuração de Feedback")]
    [Tooltip("Cor dourada para o símbolo sobreposto")]
    public Color corDourada = new Color(1f, 0.85f, 0f); 

    public void AplicarBrilhoDourado(System.Collections.Generic.List<int> indicesSobrepostos)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (int indice in indicesSobrepostos)
        {
            if (indice >= 0 && indice < renderers.Length)
            {
                renderers[indice].color = corDourada;
            }
        }
    }
}