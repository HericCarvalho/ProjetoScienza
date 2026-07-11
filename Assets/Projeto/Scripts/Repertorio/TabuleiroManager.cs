using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    public static TabuleiroManager Instancia { get; private set; }

    public enum EstadoTurno { Jogador, Inimigo, Transicao }

    [Header("Sistema de Turnos")]
    public EstadoTurno turnoAtual = EstadoTurno.Jogador;
    public int jogadasDoJogador = 0;
    public int maximoJogadasPorTurno = 4;

    [Header("Contabilidade do Jogo")]
    public int totalSobreposicoesSimbolos = 0;   // Pontos do Jogador
    public int totalSobreposicoesInimigo = 0;    // Pontos do Inimigo

    [Header("Configuracoes do Grid Triangular")]
    public int tamanhoBaseTopo = 6;
    public float tamanhoCirculo = 0.6f;

    [Header("Prefabs")]
    public GameObject prefabNoGrid;

    [Header("Configuração de Símbolo Inicial")]
    public TipoSimbolo simboloInicial = TipoSimbolo.Triangulo;
    [Tooltip("Nome do nó que se comporta como o ponto de partida do jogo.")]
    public string nomeNoInicial = "No_Col_0_Lin_5";

    private Dictionary<Vector2Int, NoGrid> dicionarioGrid = new Dictionary<Vector2Int, NoGrid>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        GerarTabuleiroTriangular();
    }

    void Start()
    {
        if (TemporizadorJogo.Instancia != null)
        {
            TemporizadorJogo.Instancia.IniciarTemporizador();
        }
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

                if (novoNoObj.name == nomeNoInicial)
                {
                    noScript.simboloAtual = simboloInicial;
                    noScript.estaOcupado = true;
                }

                dicionarioGrid.Add(new Vector2Int(x, y), noScript);
            }
        }
    }

    public void AlterarSimboloInicial(TipoSimbolo novoSimbolo)
    {
        simboloInicial = novoSimbolo;
        foreach (var no in dicionarioGrid.Values)
        {
            if (no.name == nomeNoInicial)
            {
                no.simboloAtual = novoSimbolo;
                no.estaOcupado = true;
                no.jaEstaDourado = false;
                break;
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

    private void ContabilizarPontuacao(int pontosGanhos)
    {
        if (turnoAtual == EstadoTurno.Jogador)
        {
            totalSobreposicoesSimbolos += pontosGanhos;
            if (ControladorUI.Instancia != null)
                ControladorUI.Instancia.AtualizarTextoContador(totalSobreposicoesSimbolos);
        }
        else // Se for turno do inimigo ou se estiver rodando o script simulado dele
        {
            totalSobreposicoesInimigo += pontosGanhos;
            if (ControladorUI.Instancia != null)
                ControladorUI.Instancia.AtualizarTextoContadorInimigo(totalSobreposicoesInimigo);
        }
    }

    public bool TCanvasInimigo { get; set; } 

    public bool TentarPosicionarPeca(PecaDomino peca, NoGrid noCentro)
    {
        // Aceita a jogada se for o jogador OU se o script do inimigo estiver simulando no momento correto
        if (turnoAtual != EstadoTurno.Jogador && !peca.name.Contains("(Inimigo)")) return false;

        List<NoGrid> nosAlvos = new List<NoGrid>();

        foreach (var sub in peca.circulosDaPeca)
        {
            int alvoX = noCentro.x + sub.posicaoRelativa.x;
            int alvoY = noCentro.y + sub.posicaoRelativa.y;
            Vector2Int coordenadaAlvo = new Vector2Int(alvoX, alvoY);

            if (!dicionarioGrid.ContainsKey(coordenadaAlvo)) return false;
            nosAlvos.Add(dicionarioGrid[coordenadaAlvo]);
        }

        bool jogoJaComecou = TotalNosOcupados() > 1;
        bool encostouEmPecaAtiva = false;

        if (!jogoJaComecou && noCentro.name != nomeNoInicial) return false;

        if (noCentro.name == nomeNoInicial || noCentro.estaOcupado) encostouEmPecaAtiva = true;
        foreach (NoGrid no in nosAlvos)
        {
            if (no.estaOcupado) encostouEmPecaAtiva = true;
        }

        if (!encostouEmPecaAtiva) return false;

        for (int i = 0; i < peca.circulosDaPeca.Length; i++)
        {
            if (nosAlvos[i].simboloAtual != TipoSimbolo.Nenhum && nosAlvos[i].simboloAtual != peca.circulosDaPeca[i].simbolo)
            {
                return false;
            }
        }

        peca.transform.position = noCentro.transform.position;
        peca.foiPosicionada = true;

        List<int> indicesQueSobrepuseram = new List<int>();

        for (int i = 0; i < peca.circulosDaPeca.Length; i++)
        {
            if ((nosAlvos[i].simboloAtual == peca.circulosDaPeca[i].simbolo && nosAlvos[i].simboloAtual != TipoSimbolo.Nenhum) || nosAlvos[i].jaEstaDourado)
            {
                indicesQueSobrepuseram.Add(i);
            }
            else if (nosAlvos[i].name == nomeNoInicial && !jogoJaComecou)
            {
                indicesQueSobrepuseram.Add(i);
            }
        }

        for (int i = 0; i < peca.circulosDaPeca.Length; i++)
        {
            nosAlvos[i].simboloAtual = peca.circulosDaPeca[i].simbolo;
            nosAlvos[i].estaOcupado = true;
            if (indicesQueSobrepuseram.Contains(i))
            {
                nosAlvos[i].jaEstaDourado = true;
            }
        }

        if (indicesQueSobrepuseram.Count > 0)
        {
            ContabilizarPontuacao(indicesQueSobrepuseram.Count);
        }

        PecaFeedback feedback = peca.GetComponent<PecaFeedback>();
        if (feedback != null && indicesQueSobrepuseram.Count > 0)
        {
            feedback.Invoke("GarantirCorDourada", 0.02f);
            feedback.AplicarBrilhoDourado(indicesQueSobrepuseram);
        }

        if (turnoAtual == EstadoTurno.Jogador)
        {
            jogadasDoJogador++;
            if (jogadasDoJogador >= maximoJogadasPorTurno)
            {
                PassarRodadaParaInimigo();
            }
        }

        return true;
    }

    public void PassarRodadaParaInimigo()
    {
        if (turnoAtual != EstadoTurno.Jogador) return;

        if (TemporizadorJogo.Instancia != null) TemporizadorJogo.Instancia.PararTemporizador();

        turnoAtual = EstadoTurno.Inimigo; // Atualizado diretamente para Inimigo para o timer rodar

        if (TemporizadorJogo.Instancia != null)
        {
            TemporizadorJogo.Instancia.IniciarTemporizador(); // Inicia o tempo do Inimigo
        }

        if (AdversarioScriptado.Instancia != null)
        {
            AdversarioScriptado.Instancia.IniciarTurnoInimigo();
        }
        else
        {
            Invoke("SimularFimTurnoInimigo", 2.0f);
        }
    }

    public void ForçarDerrotaPorTempo()
    {
        // Se o tempo esgotou no turno do jogador
        if (turnoAtual == EstadoTurno.Jogador)
        {
            turnoAtual = EstadoTurno.Transicao;
            StartCoroutine(RotinaLimpezaGradualTabuleiro(true));
        }
        // Se o tempo esgotou no turno do inimigo
        else if (turnoAtual == EstadoTurno.Inimigo)
        {
            turnoAtual = EstadoTurno.Transicao;
            StartCoroutine(RotinaLimpezaGradualTabuleiro(false));
        }
    }

    public void ForçarLimpezaTurnoInimigo()
    {
        StartCoroutine(RotinaLimpezaGradualTabuleiro(false));
    }

    private IEnumerator RotinaLimpezaGradualTabuleiro(bool proximoEhInimigo)
    {
        PecaDomino[] pecasNaCena = FindObjectsByType<PecaDomino>(FindObjectsSortMode.None);

        foreach (PecaDomino peca in pecasNaCena)
        {
            if (peca.foiPosicionada)
            {
                float tempoSumir = 0.3f;
                float cronometro = 0f;
                Vector3 escalaOriginal = peca.transform.localScale;

                while (cronometro < tempoSumir)
                {
                    cronometro += Time.deltaTime;
                    if (peca != null) peca.transform.localScale = Vector3.Lerp(escalaOriginal, Vector3.zero, cronometro / tempoSumir);
                    yield return null;
                }

                Destroy(peca.gameObject);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Reseta dados lógicos da Grid
        foreach (var no in dicionarioGrid.Values)
        {
            if (no.name == nomeNoInicial)
            {
                no.simboloAtual = simboloInicial;
                no.estaOcupado = true;
                no.jaEstaDourado = false;
            }
            else
            {
                no.simboloAtual = TipoSimbolo.Nenhum;
                no.estaOcupado = false;
                no.jaEstaDourado = false;
            }
        }

        if (proximoEhInimigo)
        {
            jogadasDoJogador = 0;
            turnoAtual = EstadoTurno.Inimigo;

            // Reinicia o temporizador focando no turno do Inimigo
            if (TemporizadorJogo.Instancia != null) TemporizadorJogo.Instancia.IniciarTemporizador();

            if (AdversarioScriptado.Instancia != null) AdversarioScriptado.Instancia.IniciarTurnoInimigo();
            else SimularFimTurnoInimigo();
        }
        else
        {
            FinalizarTurnoInimigoCompleto();
        }
    }

    private void FinalizarTurnoInimigoCompleto()
    {
        Debug.Log("[TABULEIRO] Finalizando turno do inimigo. Devolvendo controle ao Jogador.");

        jogadasDoJogador = 0;
        turnoAtual = EstadoTurno.Jogador;

        // Manda spawnar as peças novamente
        if (GerenciadorDePartida.Instancia != null)
        {
            GerenciadorDePartida.Instancia.SpawnarPecasDoJogador();
        }
        else
        {
            Debug.LogError("[TABULEIRO] Erro: Instância do GerenciadorDePartida não foi encontrada!");
        }

        if (TemporizadorJogo.Instancia != null)
        {
            TemporizadorJogo.Instancia.IniciarTemporizador();
        }
    }

    public void SimularFimTurnoInimigo()
    {
        if (TemporizadorJogo.Instancia != null) TemporizadorJogo.Instancia.PararTemporizador();
        ForçarLimpezaTurnoInimigo();
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