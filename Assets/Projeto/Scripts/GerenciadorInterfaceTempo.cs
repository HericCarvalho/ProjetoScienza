using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GerenciadorInterfaceTempo : MonoBehaviour
{
    public static GerenciadorInterfaceTempo Instancia { get; private set; }

    [Header("Textos de Turno")]
    public TextMeshProUGUI textoIdentificadorTurno;

    [Header("Barra de Tempo (Ritmo)")]
    public Image barraTempoPreenchimento;
    public TextMeshProUGUI textoNomeDaBarra;

    [Header("Barras de Barulho (Pontuação)")]
    public Image barraBarulhoJogador;
    public Image barraBarulhoAdversario;

    [Header("Configurações de Tempo")]
    public float tempoMaximoTurno = 30f;
    private float tempoAtual;
    private bool cronometroAtivo = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (textoNomeDaBarra != null) textoNomeDaBarra.text = "Ritmo";

        if (barraBarulhoJogador != null) barraBarulhoJogador.fillAmount = 0f;
        if (barraBarulhoAdversario != null) barraBarulhoAdversario.fillAmount = 0f;
    }

    void Update()
    {
        if (!cronometroAtivo) return;

        tempoAtual -= Time.deltaTime;
        tempoAtual = Mathf.Clamp(tempoAtual, 0f, tempoMaximoTurno);

        float percentualNormalizado = tempoAtual / tempoMaximoTurno;

        if (barraTempoPreenchimento != null)
        {
            barraTempoPreenchimento.fillAmount = percentualNormalizado;
        }

        if (tempoAtual <= 0)
        {
            PararTemporizador();

            ArrastarPeca[] pecasSendoArrastadas = FindObjectsByType<ArrastarPeca>(FindObjectsSortMode.None);
            foreach (ArrastarPeca arrastavel in pecasSendoArrastadas)
            {
                if (arrastavel != null && arrastavel.sendoSegurada)
                {
                    arrastavel.DevolverParaAMao();
                }
            }

            if (TabuleiroManager.Instancia != null)
            {
                // CRUCIAL: Só pune com derrota se o tempo acabar NO TURNO DO JOGADOR
                if (TabuleiroManager.Instancia.turnoAtual == TabuleiroManager.EstadoTurno.Jogador)
                {
                    TabuleiroManager.Instancia.ForçarDerrotaPorTempo();
                }
                // Se o tempo acabar no turno da Madu, força a finalização do turno dela de forma limpa
                else if (TabuleiroManager.Instancia.turnoAtual == TabuleiroManager.EstadoTurno.Inimigo)
                {
                    Debug.LogWarning("[TEMPO] Tempo esgotado para a Madu! Forçando passagem de turno.");
                    TabuleiroManager.Instancia.FinalizarTurnoInimigoCompleto();
                }
            }
        }
    }

    public void IniciarTemporizador()
    {
        tempoAtual = tempoMaximoTurno;
        cronometroAtivo = true;

        // CORREÇÃO VISUAL: Garante que a barra volte a ficar 100% cheia imediatamente no clique/troca de turno
        if (barraTempoPreenchimento != null)
        {
            barraTempoPreenchimento.fillAmount = 1f;
        }

        Debug.Log($"[TEMPO] Temporizador iniciado com {tempoMaximoTurno} segundos.");
    }

    public void PararTemporizador()
    {
        cronometroAtivo = false;
    }

    public void AtualizarTextoDeTurno(int numeroTurno, bool ehVezDoJogador)
    {
        if (textoIdentificadorTurno == null) return;

        if (ehVezDoJogador)
        {
            textoIdentificadorTurno.text = $"{numeroTurno}°Turno - Sua vez";
        }
        else
        {
            textoIdentificadorTurno.text = $"{numeroTurno}°Turno - Vez da madu";
        }
    }

    public void AtualizarVisualBarulho(int pontosJogador, int pontosInimigo)
    {
        float tetoPontos = Mathf.Max(10f, Mathf.Max(pontosJogador, pontosInimigo));

        if (barraBarulhoJogador != null)
        {
            barraBarulhoJogador.fillAmount = (float)pontosJogador / tetoPontos;
        }

        if (barraBarulhoAdversario != null)
        {
            barraBarulhoAdversario.fillAmount = (float)pontosInimigo / tetoPontos;
        }
    }

    public void IniciarJogoAposTutorial()
    {
        IniciarTemporizador();

        if (TabuleiroManager.Instancia != null)
        {
            AtualizarTextoDeTurno(
                TabuleiroManager.Instancia.numeroDoTurnoAtual,
                TabuleiroManager.Instancia.turnoAtual == TabuleiroManager.EstadoTurno.Jogador
            );
        }
    }
}