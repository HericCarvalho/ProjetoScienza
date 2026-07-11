using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ConfiguracaoSimbolo
{
    public TipoSimbolo tipo;
    public Sprite imagem;
}

public class TabuleiroManager : MonoBehaviour
{
    public static TabuleiroManager Instancia { get; private set; }

    public enum EstadoTurno { Jogador, Inimigo, Transicao }

    [Header("Sistema de Turnos")]
    public EstadoTurno turnoAtual = EstadoTurno.Jogador;
    public int jogadasDoJogador = 0;
    public int maximoJogadasPorTurno = 4;

    [Header("Progressão de Turnos Globais")]
    [Tooltip("O número do turno atual do jogo (começa em 1 e aumenta cada vez que o controle volta ao jogador).")]
    public int numeroDoTurnoAtual = 1;

    [Header("Telas de Fim de Jogo (UI)")]
    public GameObject painelVitoria;
    public GameObject painelDerrota;

    [Header("Contabilidade do Jogo")]
    public int totalSobreposicoesSimbolos = 0;
    public int totalSobreposicoesInimigo = 0;

    [Header("Configuracoes do Grid Triangular")]
    public int tamanhoBaseTopo = 6;
    public float tamanhoCirculo = 1.0f;

    [Header("Prefabs")]
    public GameObject prefabNoGrid;

    [Header("Configuração de Símbolo Inicial por Rodada")]
    [Tooltip("Coloque aqui a sequência de símbolos para cada turno. Elemento 0 = Turno 1, Elemento 1 = Turno 2, etc.")]
    public List<TipoSimbolo> sequenciaSimbolosPorTurno = new List<TipoSimbolo>() { TipoSimbolo.Triangulo };

    [HideInInspector] public TipoSimbolo simboloInicial;
    public string nomeNoInicial = "No_Col_0_Lin_5";

    [Header("Banco de Imagens dos Símbolos")]
    public List<ConfiguracaoSimbolo> bancoDeSimbolos;

    [Header("Posicionamento do Tabuleiro")]
    public Transform ancoragemTabuleiro;
    public Vector2 offsetManual = Vector2.zero;

    private Dictionary<Vector2Int, NoGrid> dicionarioGrid = new Dictionary<Vector2Int, NoGrid>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        AtualizarSimboloDoTurnoAtual();
        GerarTabuleiroTriangular();
    }

    void Start()
    {
        // CORREÇÃO: Garante que o temporizador e os textos visuais comecem funcionando no Turno 1
        if (GerenciadorInterfaceTempo.Instancia != null)
        {
            GerenciadorInterfaceTempo.Instancia.AtualizarTextoDeTurno(numeroDoTurnoAtual, true);
            GerenciadorInterfaceTempo.Instancia.IniciarTemporizador();
        }
    }

    private void AtualizarSimboloDoTurnoAtual()
    {
        if (sequenciaSimbolosPorTurno == null || sequenciaSimbolosPorTurno.Count == 0)
        {
            simboloInicial = TipoSimbolo.Triangulo;
            return;
        }

        int indice = numeroDoTurnoAtual - 1;
        if (indice >= sequenciaSimbolosPorTurno.Count)
        {
            indice = sequenciaSimbolosPorTurno.Count - 1;
        }

        simboloInicial = sequenciaSimbolosPorTurno[indice];
    }

    public Sprite ObterSpriteDoSimbolo(TipoSimbolo tipo)
    {
        foreach (ConfiguracaoSimbolo conf in bancoDeSimbolos)
        {
            if (conf.tipo == tipo) return conf.imagem;
        }
        return null;
    }

    private void GerarTabuleiroTriangular()
    {
        dicionarioGrid.Clear();

        float alturaHexagonal = tamanhoCirculo * 0.866025f;
        float larguraTotalTopo = (tamanhoBaseTopo - 1) * tamanhoCirculo;
        float alturaTotal = (tamanhoBaseTopo - 1) * alturaHexagonal;

        Vector3 centroCalculado = new Vector3(-larguraTotalTopo * 0.5f, alturaTotal * 0.5f, 0f);
        Vector3 pontoDeOrigem = (ancoragemTabuleiro != null) ? ancoragemTabuleiro.position : Vector3.zero;
        Vector3 deslocamentoFinal = pontoDeOrigem + new Vector3(offsetManual.x, offsetManual.y, 0f);

        for (int y = 0; y < tamanhoBaseTopo; y++)
        {
            int circulosNestaLinha = tamanhoBaseTopo - y;
            float deslocamentoX = y * (tamanhoCirculo * 0.5f);

            for (int x = 0; x < circulosNestaLinha; x++)
            {
                float posX = (x * tamanhoCirculo) + deslocamentoX;
                float posY = -y * alturaHexagonal;

                Vector3 posicaoMundo = new Vector3(posX, posY, 0f) + centroCalculado + deslocamentoFinal;

                GameObject novoNoObj = Instantiate(prefabNoGrid, posicaoMundo, Quaternion.identity, transform);
                novoNoObj.name = $"No_Col_{x}_Lin_{y}";

                NoGrid noScript = novoNoObj.GetComponent<NoGrid>();
                noScript.x = x;
                noScript.y = y;

                if (novoNoObj.name == nomeNoInicial)
                {
                    noScript.estaOcupado = true;
                    noScript.jaEstaDourado = false;

                    Sprite spriteInicial = ObterSpriteDoSimbolo(simboloInicial);
                    noScript.DefinirSimboloVisual(simboloInicial, spriteInicial);
                }
                else
                {
                    noScript.DefinirSimboloVisual(TipoSimbolo.Nenhum, null);
                    noScript.estaOcupado = false;
                    noScript.jaEstaDourado = false;
                }

                dicionarioGrid.Add(new Vector2Int(x, y), noScript);
            }
        }
    }

    public void AlterarSimboloInicial(TipoSimbolo novoSimbolo)
    {
        simboloInicial = novoSimbolo;
        Sprite novoSprite = ObterSpriteDoSimbolo(novoSimbolo);

        foreach (var no in dicionarioGrid.Values)
        {
            if (no.name == nomeNoInicial)
            {
                no.estaOcupado = true;
                no.jaEstaDourado = false;
                no.DefinirSimboloVisual(novoSimbolo, novoSprite);
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
        }
        else
        {
            totalSobreposicoesInimigo += pontosGanhos;
        }

        if (GerenciadorInterfaceTempo.Instancia != null)
        {
            GerenciadorInterfaceTempo.Instancia.AtualizarVisualBarulho(totalSobreposicoesSimbolos, totalSobreposicoesInimigo);
        }
    }

    public bool TCanvasInimigo { get; set; }

    public bool TentarPosicionarPeca(PecaDomino peca, NoGrid noCentro)
    {
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
            // CORREÇÃO DO DOURADO: Define se é dourado ANTES de chamar a função visual!
            if (indicesQueSobrepuseram.Contains(i))
            {
                nosAlvos[i].jaEstaDourado = true;
            }

            Sprite spriteDoSimbolo = ObterSpriteDoSimbolo(peca.circulosDaPeca[i].simbolo);
            nosAlvos[i].DefinirSimboloVisual(peca.circulosDaPeca[i].simbolo, spriteDoSimbolo);
            nosAlvos[i].estaOcupado = true;
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

        if (GerenciadorInterfaceTempo.Instancia != null)
            GerenciadorInterfaceTempo.Instancia.PararTemporizador();

        StartCoroutine(RotinaLimpezaGradualTabuleiro(proximoEhInimigo: true));
    }

    public void ForçarDerrotaPorTempo()
    {
        if (turnoAtual == EstadoTurno.Jogador)
        {
            turnoAtual = EstadoTurno.Transicao;
            StartCoroutine(RotinaLimpezaGradualTabuleiro(true));
        }
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
        // Trava o turno em transição para ninguém mexer em nada durante a limpeza
        EstadoTurno turnoAnterior = turnoAtual;
        turnoAtual = EstadoTurno.Transicao;

        PecaDomino[] pecasNaCena = FindObjectsByType<PecaDomino>(FindObjectsSortMode.None);

        foreach (PecaDomino peca in pecasNaCena)
        {
            if (peca.foiPosicionada)
            {
                float tempoSumir = 0.2f;
                float cronometro = 0f;
                Vector3 escalaOriginal = peca.transform.localScale;

                while (cronometro < tempoSumir)
                {
                    cronometro += Time.deltaTime;
                    if (peca != null) peca.transform.localScale = Vector3.Lerp(escalaOriginal, Vector3.zero, cronometro / tempoSumir);
                    yield return null;
                }

                Destroy(peca.gameObject);
                yield return new WaitForSeconds(0.05f);
            }
        }

        // --- REGRA DE PROGRESSÃO DE TURNOS ---
        if (proximoEhInimigo)
        {
            turnoAtual = EstadoTurno.Inimigo;
            if (GerenciadorInterfaceTempo.Instancia != null)
                GerenciadorInterfaceTempo.Instancia.AtualizarTextoDeTurno(numeroDoTurnoAtual, false);

            LimparDadosEVisuaisDoGrid();

            if (AdversarioScriptado.Instancia != null)
                AdversarioScriptado.Instancia.IniciarTurnoInimigo();

            // CORREÇÃO: Inicia o temporizador também para o turno da Madu
            if (GerenciadorInterfaceTempo.Instancia != null)
                GerenciadorInterfaceTempo.Instancia.IniciarTemporizador();
        }
        else
        {
            // Só avança para o próximo turno se o jogo for continuar!
            numeroDoTurnoAtual++;

            jogadasDoJogador = 0;
            turnoAtual = EstadoTurno.Jogador;
            if (GerenciadorInterfaceTempo.Instancia != null)
                GerenciadorInterfaceTempo.Instancia.AtualizarTextoDeTurno(numeroDoTurnoAtual, true);

            LimparDadosEVisuaisDoGrid();

            if (GerenciadorDePartida.Instancia != null)
                GerenciadorDePartida.Instancia.SpawnarPecasDoJogador();

            if (GerenciadorInterfaceTempo.Instancia != null)
                GerenciadorInterfaceTempo.Instancia.IniciarTemporizador();
        }
    }
    private void LimparDadosEVisuaisDoGrid()
    {
        AtualizarSimboloDoTurnoAtual();
        Sprite spriteInicial = ObterSpriteDoSimbolo(simboloInicial);

        foreach (var no in dicionarioGrid.Values)
        {
            // CORREÇÃO: Garante o reset do estado dourado
            no.jaEstaDourado = false;

            if (no.name == nomeNoInicial)
            {
                no.estaOcupado = true;
                no.DefinirSimboloVisual(simboloInicial, spriteInicial);
            }
            else
            {
                no.estaOcupado = false;
                no.DefinirSimboloVisual(TipoSimbolo.Nenhum, null);
            }
        }
    }

    private IEnumerator RotinaLimpezaFinalEEncerramento()
    {
        turnoAtual = EstadoTurno.Transicao;

        PecaDomino[] pecasNaCena = FindObjectsByType<PecaDomino>(FindObjectsSortMode.None);
        foreach (PecaDomino peca in pecasNaCena)
        {
            if (peca != null && peca.foiPosicionada)
            {
                Destroy(peca.gameObject);
            }
        }
        yield return new WaitForSeconds(0.5f);

        VerificarFimDeJogo();
    }

    public void FinalizarTurnoInimigoCompleto()
    {
        Debug.Log($"[TABULEIRO] Madu terminou as jogadas dela no Turno Global: {numeroDoTurnoAtual}.");

        if (numeroDoTurnoAtual >= 3)
        {
            if (GerenciadorInterfaceTempo.Instancia != null)
                GerenciadorInterfaceTempo.Instancia.PararTemporizador();

            StartCoroutine(RotinaLimpezaFinalEEncerramento());
            return;
        }

        StartCoroutine(RotinaLimpezaGradualTabuleiro(proximoEhInimigo: false));
    }

    private void VerificarFimDeJogo()
    {
        turnoAtual = EstadoTurno.Transicao;
        Debug.Log("[FIM DE JOGO] Partida finalizada! Total Jogador: " + totalSobreposicoesSimbolos + " | Total Madu: " + totalSobreposicoesInimigo);

        if (painelVitoria != null) painelVitoria.SetActive(false);
        if (painelDerrota != null) painelDerrota.SetActive(false);

        if (totalSobreposicoesSimbolos > totalSobreposicoesInimigo)
        {
            Debug.Log("RESULTADO: VITÓRIA DO JOGADOR!");
            if (painelVitoria != null) painelVitoria.SetActive(true);

            if (GerenciadorInterfaceTempo.Instancia != null && GerenciadorInterfaceTempo.Instancia.textoIdentificadorTurno != null)
                GerenciadorInterfaceTempo.Instancia.textoIdentificadorTurno.text = "Vitória!";
        }
        else
        {
            Debug.Log("RESULTADO: DERROTA DO JOGADOR!");
            if (painelDerrota != null) painelDerrota.SetActive(true);

            if (GerenciadorInterfaceTempo.Instancia != null && GerenciadorInterfaceTempo.Instancia.textoIdentificadorTurno != null)
                GerenciadorInterfaceTempo.Instancia.textoIdentificadorTurno.text = "Derrota!";
        }
    }

    public void SimularFimTurnoInimigo()
    {
        if (GerenciadorInterfaceTempo.Instancia != null) GerenciadorInterfaceTempo.Instancia.PararTemporizador();
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