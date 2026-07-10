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
    public TipoSimbolo simboloInicial = TipoSimbolo.Triangulo;

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

                if (y == tamanhoBaseTopo - 1 && x == 0)
                {
                    noScript.simboloAtual = simboloInicial;
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

        foreach (var sub in peca.circulosDaPeca)
        {
            int alvoX = noCentro.x + sub.posicaoRelativa.x;
            int alvoY = noCentro.y + sub.posicaoRelativa.y;
            Vector2Int coordenadaAlvo = new Vector2Int(alvoX, alvoY);

            if (!dicionarioGrid.ContainsKey(coordenadaAlvo))
            {
                Debug.LogWarning("[BLOQUEADO] A peça sairia para fora dos limites do tabuleiro.");
                return false;
            }

            nosAlvos.Add(dicionarioGrid[coordenadaAlvo]);
        }

        bool jogoJaComecou = TotalNosOcupados() > 1;
        bool encostouEmPecaAtiva = false;

        if (!jogoJaComecou && noCentro.name != "No_Col_0_Lin_5")
        {
            Debug.LogWarning("[BLOQUEADO] O jogo deve começar obrigatoriamente na ponta inferior (No_Col_0_Lin_5)!");
            return false;
        }

        if (noCentro.name == "No_Col_0_Lin_5" || noCentro.estaOcupado)
        {
            encostouEmPecaAtiva = true;
        }

        foreach (NoGrid no in nosAlvos)
        {
            if (no.estaOcupado)
            {
                encostouEmPecaAtiva = true;
            }
        }

        if (!encostouEmPecaAtiva)
        {
            Debug.LogWarning("[BLOQUEADO] A peça precisa se conectar a uma parte ativa do tabuleiro.");
            return false;
        }

        for (int i = 0; i < peca.circulosDaPeca.Length; i++)
        {
            var partePeca = peca.circulosDaPeca[i];
            NoGrid noTabuleiro = nosAlvos[i];

            if (noTabuleiro.simboloAtual != TipoSimbolo.Nenhum)
            {
                if (noTabuleiro.simboloAtual != partePeca.simbolo)
                {
                    Debug.LogWarning($"[BLOQUEADO] Erro de correspondência em {noTabuleiro.name}. Tabuleiro: {noTabuleiro.simboloAtual}, Peça: {partePeca.simbolo}");
                    return false;
                }
            }
        }

        peca.transform.position = noCentro.transform.position;
        peca.foiPosicionada = true;

        List<int> indicesQueSobrepuseram = new List<int>();

        for (int i = 0; i < peca.circulosDaPeca.Length; i++)
        {
            if (nosAlvos[i].estaOcupado && nosAlvos[i].simboloAtual == peca.circulosDaPeca[i].simbolo)
            {
                indicesQueSobrepuseram.Add(i);
            }

            nosAlvos[i].simboloAtual = peca.circulosDaPeca[i].simbolo;
            nosAlvos[i].estaOcupado = true;
        }
        PecaFeedback feedback = peca.GetComponent<PecaFeedback>();
        if (feedback != null && indicesQueSobrepuseram.Count > 0)
        {
            feedback.AplicarBrilhoDourado(indicesQueSobrepuseram);
        }

        Debug.Log($"[SUCESSO] Peça posicionada corretamente em: {noCentro.name}");
        return true;
    }

    private int TotalNosOcupados()
    {
        int count = 0;
        foreach (var no in dicionarioGrid.Values)
        {
            if (no.estaOcupado) count++;
        }
        return count;
    }
}