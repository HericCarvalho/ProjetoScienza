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
    public TipoSimbolo simboloInicial = TipoSimbolo.Triangulo; // Define o simbolo inicial do tabuleiro

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
                    noScript.simboloAtual = simboloInicial;
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

        // SEGUNDA JOGADA EM DIANTE: Para ser v�lida, o n� central onde soltou 
        // OU pelo menos um dos n�s que as asas v�o cobrir DEVE j� estar ocupado (ou ser vizinho direto).
        // Se for a primeir�ssima jogada no n� de baixo, liberamos direto.
        if (noCentro.name == "No_Col_0_Lin_5" || noCentro.estaOcupado)
        {
            adjacenciaValida = true;
        }

        // 1. Mapear os n�s do grid correspondentes � posi��o da pe�a
        foreach (var sub in peca.circulosDaPeca)
        {
            int alvoX = noCentro.x + sub.posicaoRelativa.x;
            int alvoY = noCentro.y + sub.posicaoRelativa.y;
            Vector2Int coordenadaAlvo = new Vector2Int(alvoX, alvoY);

            // Se o V da pe�a sair para fora das bordas do tri�ngulo, recusa na hora
            if (!dicionarioGrid.ContainsKey(coordenadaAlvo)) return false;

            NoGrid noAlvo = dicionarioGrid[coordenadaAlvo];

            // Se qualquer uma das asas tocar em um n� j� ocupado por outra pe�a, a adjac�ncia tamb�m se torna v�lida
            if (noAlvo.estaOcupado)
            {
                adjacenciaValida = true;

                // REGRA DE S�MBOLO: Se o n� j� tiver uma pe�a (Diferente de Nenhum), o s�mbolo DEVE ser igual
                if (noAlvo.simboloAtual != TipoSimbolo.Nenhum && noAlvo.simboloAtual != sub.simbolo)
                {
                    Debug.LogWarning($"S�mbolo incompat�vel em {noAlvo.name}. Esperado: {noAlvo.simboloAtual}, Recebido: {sub.simbolo}");
                    return false;
                }
            }

            nosAlvos.Add(noAlvo);
        }

        // 2. Se a pe�a est� conectada ao fluxo do jogo (adjacente)
        if (adjacenciaValida)
        {
            // SNAP: Fixa a pe�a visualmente
            peca.transform.position = noCentro.transform.position;
            peca.foiPosicionada = true;

            // 3. Aplica os novos s�mbolos ao tabuleiro e ativa os n�s
            for (int i = 0; i < peca.circulosDaPeca.Length; i++)
            {
                nosAlvos[i].simboloAtual = peca.circulosDaPeca[i].simbolo;
                nosAlvos[i].estaOcupado = true;
            }

            Debug.Log($"Pe�a posicionada com sucesso em: {noCentro.name}");
            return true;
        }

        Debug.LogWarning("Jogada inv�lida: A pe�a precisa tocar em uma parte ativa do tabuleiro.");
        return false;
    }
}