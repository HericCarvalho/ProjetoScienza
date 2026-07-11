using UnityEngine;

public class NoGrid : MonoBehaviour
{
    public int x;
    public int y;
    public bool estaOcupado = false;
    public bool jaEstaDourado = false;
    public TipoSimbolo simboloAtual = TipoSimbolo.Nenhum;

    [Header("Referências Visuais")]
    public SpriteRenderer spriteRendererNo;
    public SpriteRenderer spriteRendererSimboloFilho;

    [Header("Configuração de Transparência")]
    [Range(0f, 1f)] public float opacidadeGridVazio = 0.6f;
    [Range(0f, 1f)] public float opacidadeGridOcupado = 1.0f;

    // Guarda e restaura as ordens de renderização originais das camadas do Unity
    private int sortingOrderNoOriginal;
    private int sortingOrderFilhoOriginal;
    private bool ordensSalvas = false;

    void Awake()
    {
        if (spriteRendererNo == null) spriteRendererNo = GetComponent<SpriteRenderer>();
        SalvarSortingOrdersOriginais();
    }

    private void SalvarSortingOrdersOriginais()
    {
        if (ordensSalvas) return;

        if (spriteRendererNo != null) sortingOrderNoOriginal = spriteRendererNo.sortingOrder;
        if (spriteRendererSimboloFilho != null) sortingOrderFilhoOriginal = spriteRendererSimboloFilho.sortingOrder;
        ordensSalvas = true;
    }

    public void DefinirSimboloVisual(TipoSimbolo novoSimbolo, Sprite imagemDoSimbolo)
    {
        simboloAtual = novoSimbolo;
        SalvarSortingOrdersOriginais();

        // --- SE JÁ ESTÁ DOURADO (Teve sobreposição pontuada) ---
        if (jaEstaDourado)
        {
            // Força a renderização por cima das peças físicas de dominó
            if (spriteRendererNo != null)
            {
                spriteRendererNo.sortingOrder = 15;
                spriteRendererNo.color = new Color(1f, 0.85f, 0f, opacidadeGridOcupado); // Amarelo Dourado
            }

            if (spriteRendererSimboloFilho != null && novoSimbolo != TipoSimbolo.Nenhum)
            {
                spriteRendererSimboloFilho.sortingOrder = 16;
                spriteRendererSimboloFilho.sprite = imagemDoSimbolo;
                spriteRendererSimboloFilho.color = new Color(1f, 0.85f, 0f, 1f);
            }
            return;
        }

        // --- VISUAL COMUM / PADRÃO (Restaura as camadas de fundo originais) ---
        if (spriteRendererNo != null)
        {
            spriteRendererNo.sortingOrder = sortingOrderNoOriginal;
        }
        if (spriteRendererSimboloFilho != null)
        {
            spriteRendererSimboloFilho.sortingOrder = sortingOrderFilhoOriginal;
            spriteRendererSimboloFilho.color = Color.white;
        }

        // --- CONTROLE DE EXIBIÇÃO DE SÍMBOLOS ---
        if (spriteRendererSimboloFilho != null)
        {
            if (novoSimbolo == TipoSimbolo.Nenhum || imagemDoSimbolo == null)
            {
                spriteRendererSimboloFilho.sprite = null;

                if (spriteRendererNo != null)
                {
                    Color corNo = Color.white;
                    corNo.a = opacidadeGridVazio;
                    spriteRendererNo.color = corNo;
                }
            }
            else
            {
                spriteRendererSimboloFilho.sprite = imagemDoSimbolo;

                if (spriteRendererNo != null)
                {
                    Color corNo = Color.white;
                    corNo.a = opacidadeGridOcupado;
                    spriteRendererNo.color = corNo;
                }
            }
        }
        else if (spriteRendererNo != null)
        {
            // Fallback caso não use objeto filho para o ícone
            if (novoSimbolo == TipoSimbolo.Nenhum || imagemDoSimbolo == null)
            {
                Color cor = Color.white;
                cor.a = opacidadeGridVazio;
                spriteRendererNo.color = cor;
            }
            else
            {
                Color cor = Color.white;
                cor.a = opacidadeGridOcupado;
                spriteRendererNo.color = cor;
            }
        }
    }
}