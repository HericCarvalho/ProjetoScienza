using UnityEngine;
using System.Collections.Generic;

public class PecaFeedback : MonoBehaviour
{
    [Header("Configuração de Feedback")]
    public Color corDourada = new Color(1f, 0.85f, 0f);

    private List<int> ultimosIndicesDourados = new List<int>();

    public void AplicarBrilhoDourado(List<int> indicesSobrepostos)
    {
        ultimosIndicesDourados = indicesSobrepostos;
        PintarEsquemaVisual();
    }

    public void GarantirCorDourada()
    {
        PintarEsquemaVisual();
    }

    private void PintarEsquemaVisual()
    {
        SpriteRenderer[] bolinhasOrdenadas = new SpriteRenderer[3];

        Transform tCentro = transform.Find("centro");
        Transform tEsquerda = transform.Find("esquerda");
        Transform tDireita = transform.Find("direita");

        if (tCentro != null) bolinhasOrdenadas[0] = tCentro.GetComponent<SpriteRenderer>();
        if (tEsquerda != null) bolinhasOrdenadas[1] = tEsquerda.GetComponent<SpriteRenderer>();
        if (tDireita != null) bolinhasOrdenadas[2] = tDireita.GetComponent<SpriteRenderer>();

        HashSet<int> processados = new HashSet<int>();

        foreach (int indice in ultimosIndicesDourados)
        {
            if (processados.Contains(indice)) continue;

            if (indice >= 0 && indice < bolinhasOrdenadas.Length)
            {
                SpriteRenderer sr = bolinhasOrdenadas[indice];
                if (sr != null)
                {
                    // Aplica a cor dourada
                    sr.color = corDourada;

                    // Isso garante que a bolinha dourada fique visível mesmo que outras peças estejam sobrepondo-a.
                    sr.sortingOrder = 100;

                    // Aplica o mesmo para o SpriteRenderer do objeto pai, caso exista, para garantir que a peça inteira fique na frente 
                    SpriteRenderer srPai = GetComponent<SpriteRenderer>();
                    if (srPai != null) srPai.sortingOrder = 99;

                    processados.Add(indice);
                    Debug.Log($"[Camada Corrigida] {sr.gameObject.name} trazido para a frente de tudo (Order 100).");
                }
            }
        }
    }
}