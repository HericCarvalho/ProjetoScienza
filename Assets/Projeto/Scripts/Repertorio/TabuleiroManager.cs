using UnityEngine;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    public static TabuleiroManager Instancia { get; private set; }

    [Header("Configuracoes do Grid Triangular")]
    public int tamanhoBaseTopo = 6;
    public float tamanhoCirculo = 0.6f;

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
                noScript.simboloAtual = TipoSimbolo.Nenhum;
                noScript.estaOcupado = false;

                // Define explicitamente o ponto de partida do jogo na ponta de baixo
                if (y == tamanhoBaseTopo - 1 && x == 0)
                {
                    noScript.simboloAtual = TipoSimbolo.Triangulo;
                    noScript.estaOcupado = true;
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

    public bool TentarPosicionarPeca(PecaDomino peca, NoGrid noCentro)
    {
        List<NoGrid> nosAlvos = new List<NoGrid>();
        bool adjacenciaValida = false;

        // SEGUNDA JOGADA EM DIANTE: Para ser válida, o nó central onde soltou 
        // OU pelo menos um dos nós que as asas vão cobrir DEVE já estar ocupado (ou ser vizinho direto).
        // Se for a primeiríssima jogada no nó de baixo, liberamos direto.
        if (noCentro.name == "No_Col_0_Lin_5" || noCentro.estaOcupado)
        {
            adjacenciaValida = true;
        }

        // 1. Mapear os nós do grid correspondentes à posição da peça
        foreach (var sub in peca.circulosDaPeca)
        {
            int alvoX = noCentro.x + sub.posicaoRelativa.x;
            int alvoY = noCentro.y + sub.posicaoRelativa.y;
            Vector2Int coordenadaAlvo = new Vector2Int(alvoX, alvoY);

            // Se o V da peça sair para fora das bordas do triângulo, recusa na hora
            if (!dicionarioGrid.ContainsKey(coordenadaAlvo)) return false;

            NoGrid noAlvo = dicionarioGrid[coordenadaAlvo];

            // Se qualquer uma das asas tocar em um nó já ocupado por outra peça, a adjacência também se torna válida
            if (noAlvo.estaOcupado)
            {
                adjacenciaValida = true;

                // REGRA DE SÍMBOLO: Se o nó já tiver uma peça (Diferente de Nenhum), o símbolo DEVE ser igual
                if (noAlvo.simboloAtual != TipoSimbolo.Nenhum && noAlvo.simboloAtual != sub.simbolo)
                {
                    Debug.LogWarning($"Símbolo incompatível em {noAlvo.name}. Esperado: {noAlvo.simboloAtual}, Recebido: {sub.simbolo}");
                    return false;
                }
            }

            nosAlvos.Add(noAlvo);
        }

        // 2. Se a peça está conectada ao fluxo do jogo (adjacente)
        if (adjacenciaValida)
        {
            // SNAP: Fixa a peça visualmente
            peca.transform.position = noCentro.transform.position;
            peca.foiPosicionada = true;

            // 3. Aplica os novos símbolos ao tabuleiro e ativa os nós
            for (int i = 0; i < peca.circulosDaPeca.Length; i++)
            {
                nosAlvos[i].simboloAtual = peca.circulosDaPeca[i].simbolo;
                nosAlvos[i].estaOcupado = true;
            }

            Debug.Log($"Peça posicionada com sucesso em: {noCentro.name}");
            return true;
        }

        Debug.LogWarning("Jogada inválida: A peça precisa tocar em uma parte ativa do tabuleiro.");
        return false;
    }
}