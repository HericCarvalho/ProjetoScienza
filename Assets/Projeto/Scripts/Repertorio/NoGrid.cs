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

        // SE VOCÊ USA APENAS UM SPRITERENDERER NO OBJETO PRINCIPAL:
        if (spriteRendererSimboloFilho == null && spriteRendererNo != null)
        {
            if (novoSimbolo == TipoSimbolo.Nenhum)
            {
                // Se não tem símbolo, mantemos o sprite padrão da moldura (se houver) e aplicamos a opacidade de vazio
                Color cor = spriteRendererNo.color;
                cor.a = opacidadeGridVazio;
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
        // SE VOCÊ USA O OBJETO FILHO (SimboloVisual) DESTACADO PARA A PEÇA INTERNA:
        else if (spriteRendererSimboloFilho != null)
        {
            if (novoSimbolo == TipoSimbolo.Nenhum)
            {
                spriteRendererSimboloFilho.sprite = null; // LIMPA VISUALMENTE A PEÇA VELHA!

                Color corNo = spriteRendererNo.color;
                corNo.a = opacidadeGridVazio; // Mantém a moldura do grid visível conforme configurado
                spriteRendererNo.color = corNo;
            }
            else
            {
                spriteRendererSimboloFilho.sprite = imagemDoSimbolo; // Aplica o novo símbolo (Triângulo, Círculo, etc)

                Color corNo = spriteRendererNo.color;
                corNo.a = opacidadeGridOcupado;
                spriteRendererNo.color = corNo;
            }
        }
    }
}