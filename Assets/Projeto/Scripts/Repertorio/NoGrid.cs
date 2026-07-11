using UnityEngine;

public class NoGrid : MonoBehaviour
{
    public int x;
    public int y;
    public bool estaOcupado = false;
    public bool jaEstaDourado = false;
    public TipoSimbolo simboloAtual = TipoSimbolo.Nenhum;

    [Header("Referências Visuais")]
    // Arraste o SpriteRenderer do próprio Nó aqui no Inspector
    public SpriteRenderer spriteRendererNo;

    // Se o seu símbolo for renderizado em um objeto FILHO do nó, use esta variável:
    public SpriteRenderer spriteRendererSimboloFilho;

    [Header("Configuração de Transparência")]
    [Range(0f, 1f)] public float opacidadeGridVazio = 0.6f; // Ajuste para menos transparente (ex: 0.6f ou 0.7f)
    [Range(0f, 1f)] public float opacidadeGridOcupado = 1.0f;

    void Awake()
    {
        if (spriteRendererNo == null) spriteRendererNo = GetComponent<SpriteRenderer>();
    }

    public void DefinirSimboloVisual(TipoSimbolo novoSimbolo, Sprite imagemDoSimbolo)
    {
        simboloAtual = novoSimbolo;

        // Se você usa apenas UM SpriteRenderer para o Nó inteiro (Moldura + Símbolo mudam juntos):
        if (spriteRendererSimboloFilho == null && spriteRendererNo != null)
        {
            if (novoSimbolo == TipoSimbolo.Nenhum)
            {
                // SE SUMIR AQUI: Significa que precisamos manter o sprite da MOLDURA VAZIA.
                // Não mude o sprite para null se o mesmo renderizador faz o fundo do grid!
                Color cor = spriteRendererNo.color;
                cor.a = opacidadeGridVazio; // Deixa menos transparente conforme configurado
                spriteRendererNo.color = cor;
            }
            else
            {
                spriteRendererNo.sprite = imagemDoSimbolo;
                Color cor = spriteRendererNo.color;
                cor.a = opacidadeGridOcupado;
                spriteRendererNo.color = cor;
            }
        }
        // Se você usa um objeto FILHO destacado apenas para mostrar o símbolo interno:
        else if (spriteRendererSimboloFilho != null)
        {
            if (novoSimbolo == TipoSimbolo.Nenhum)
            {
                spriteRendererSimboloFilho.sprite = null; // Remove o símbolo interno

                // Mantém a moldura visível com a opacidade desejada
                Color corNo = spriteRendererNo.color;
                corNo.a = opacidadeGridVazio;
                spriteRendererNo.color = corNo;
            }
            else
            {
                spriteRendererSimboloFilho.sprite = imagemDoSimbolo;

                Color corNo = spriteRendererNo.color;
                corNo.a = opacidadeGridOcupado;
                spriteRendererNo.color = corNo;
            }
        }
    }
}