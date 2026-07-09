using UnityEngine;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    public static TabuleiroManager Instancia { get; private set; }

    [Header("Configuracoes do Grid Triangular")]
    public int tamanhoBaseTopo = 6; // Define o número de círculos que a linha mais larga
    public float tamanhoCirculo = 0.6f; // Define o diâmetro/tamanho que cada círculo terá no mundo do jogo

    [Header("Prefabs")]
    public GameObject prefabNoGrid;

    private Dictionary<Vector2Int, NoGrid> dicionarioGrid = new Dictionary<Vector2Int, NoGrid>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        GerarTabuleiroTriangular();
    }

    private void GerarTabuleiroTriangular()
    {
        dicionarioGrid.Clear();

        float alturaHexagonal = tamanhoCirculo * 0.866025f;
        float larguraTotalTopo = (tamanhoBaseTopo - 1) * tamanhoCirculo;
        float alturaTotal = (tamanhoBaseTopo - 1) * alturaHexagonal;

        Vector3 offsetCentralizado = new Vector3(-larguraTotalTopo * 0.5f, alturaTotal * 0.5f, 0f);

        for (int y = 0; y < tamanhoBaseTopo; y++)
        {
            int circulosNestaLinha = tamanhoBaseTopo - y;
            float deslocamentoX = y * (tamanhoCirculo * 0.5f);

            for (int x = 0; x < circulosNestaLinha; x++)
            {
                float posX = (x * tamanhoCirculo) + deslocamentoX;
                float posY = -y * alturaHexagonal;

                Vector3 posicaoMundo = new Vector3(posX, posY, 0f) + offsetCentralizado;

                GameObject novoNoObj = Instantiate(prefabNoGrid, posicaoMundo, Quaternion.identity, transform);
                novoNoObj.name = $"No_Col_{x}_Lin_{y}";

                NoGrid noScript = novoNoObj.GetComponent<NoGrid>();
                noScript.x = x;
                noScript.y = y;

                // Se o seu tamanhoBaseTopo for 6, a ultima linha e a 5.
                if (y == tamanhoBaseTopo - 1 && x == 0)
                {
                    noScript.simboloAtual = TipoSimbolo.Triangulo;
                }

                dicionarioGrid.Add(new Vector2Int(x, y), noScript);
            }
        }
    }

    public NoGrid ObterNoMaisProximo(Vector3 posicaoMundo, float raioMaximo)
    {
        NoGrid noMaisProximo = null;
        float menorDistancia = raioMaximo;

        foreach (var no in dicionarioGrid.Values)
        {
            float distancia = Vector3.Distance(posicaoMundo, no.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                noMaisProximo = no;
            }
        }
        return noMaisProximo;
    }

    // Valida se a peca pode ser colocada de acordo com as regras de adjacencia e simbolos
    public bool TentarPosicionarPeca(PecaDomino peca, NoGrid noCentro)
    {
        List<NoGrid> nosAlvos = new List<NoGrid>();
        bool encostouEmPecaExistente = false;
        bool simboloBateuComSucesso = false;

        // Mapeia e valida as posicoes das pontas da peca
        foreach (var sub in peca.circulosDaPeca)
        {
            int alvoX = noCentro.x + sub.posicaoRelativa.x;
            int alvoY = noCentro.y + sub.posicaoRelativa.y;

            Vector2Int coordenadaAlvo = new Vector2Int(alvoX, alvoY);

            // Se alguma parte essencial da peca ficar fora do grid do triangulo, cancela
            if (!dicionarioGrid.ContainsKey(coordenadaAlvo)) return false;

            NoGrid noAlvo = dicionarioGrid[coordenadaAlvo];

            // Checa as pecas ja existentes no tabuleiro
            if (noAlvo.estaOcupado)
            {
                encostouEmPecaExistente = true; // Achou uma peca colocada ali

                if (noAlvo.simboloAtual == sub.simbolo)
                {
                    simboloBateuComSucesso = true; // O simbolo da peca e igual ao do tabuleiro
                }
                else
                {
                    Debug.LogWarning("Bloqueado: O simbolo " + sub.simbolo + " nao bate com " + noAlvo.simboloAtual);
                    return false; // Simbolos diferentes quebram a jogada
                }
            }

            nosAlvos.Add(noAlvo);
        }

        // A peca obrigatoriamente precisa encostar em alguma peca que ja estava no tabuleiro e o simbolo sobreposto precisa ser valido.
        if (encostouEmPecaExistente && simboloBateuComSucesso)
        {

            // Fixa a peca na posicao
            peca.transform.position = noCentro.transform.position;
            peca.foiPosicionada = true;

            // Grava os novos simbolos no tabuleiro para as proximas pecas usarem
            for (int i = 0; i < peca.circulosDaPeca.Length; i++)
            {
                nosAlvos[i].simboloAtual = peca.circulosDaPeca[i].simbolo;
            }

            Debug.Log("Jogada Valida! Peca fixada.");
            return true;
        }

        Debug.LogWarning("Jogada Invalida: Voce tentou colocar a peca isolada do resto do tabuleiro!");
        return false;
    }
}