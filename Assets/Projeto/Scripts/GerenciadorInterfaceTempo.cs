using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GerenciadorInterfaceTempo : MonoBehaviour
{
    public static GerenciadorInterfaceTempo Instancia { get; private set; }

    [Header("Textos de Turno")]
    public TextMeshProUGUI textoIdentificadorTurno; // Ex: "Turno 1º - Seu turno"

    [Header("Barra de Tempo (Temporizador)")]
    public Image barraTempoPreenchimento; // Image com Image Type configurada como 'Filled'
    public TextMeshProUGUI textoNomeDaBarra; // Ex: "Tempo Restante"

    [Header("Configurações de Tempo")]
    public float tempoMaximoTurno = 30f;
    private float tempoAtual;
    private bool cronometroAtivo = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!cronometroAtivo) return;

        tempoAtual -= Time.deltaTime;

        // Calcula a fração para a barra de UI (vai de 1 de volta para 0)
        float percentualNormalizado = Mathf.Clamp01(tempoAtual / tempoMaximoTurno);
        AtualizarBarraTempo(percentualNormalizado, "Tempo Restante");

        if (tempoAtual <= 0)
        {
            PararTemporizador();
            if (TabuleiroManager.Instancia != null)
            {
                TabuleiroManager.Instancia.ForçarDerrotaPorTempo();
            }
        }
    }

    // --- MÉTODOS DO TEMPORIZADOR ---
    public void IniciarTemporizador()
    {
        tempoAtual = tempoMaximoTurno;
        cronometroAtivo = true;
    }

    public void PararTemporizador()
    {
        cronometroAtivo = false;
    }

    // --- MÉTODOS DE CONTROLE DA UI ---
    public void AtualizarTextoDeTurno(int numeroTurno, bool ehVezDoJogador)
    {
        if (textoIdentificadorTurno == null) return;

        if (ehVezDoJogador)
        {
            textoIdentificadorTurno.text = $"Turno {numeroTurno}º - Seu turno";
        }
        else
        {
            textoIdentificadorTurno.text = $"Turno {numeroTurno}º - Turno da Madu";
        }
    }

    private void AtualizarBarraTempo(float percentual, string nomeEstadoTempo)
    {
        if (barraTempoPreenchimento != null)
        {
            barraTempoPreenchimento.fillAmount = percentual;
        }

        if (textoNomeDaBarra != null)
        {
            textoNomeDaBarra.text = nomeEstadoTempo;
        }
    }

    // --- MÉTODO DISPARADOR DO FIM DO TUTORIAL (Chame pela Timeline) ---
    public void IniciarJogoAposTutorial()
    {
        Debug.Log("[TUTORIAL] Tutorial finalizado. Iniciando partida!");

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