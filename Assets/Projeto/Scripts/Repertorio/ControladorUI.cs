using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorUI : MonoBehaviour
{
    public static ControladorUI Instancia { get; private set; }

    [Header("Textos de Turno")]
    public TextMeshProUGUI textoIdentificadorTurno; // Ex: "Turno 1º - Seu turno"

    [Header("Barra de Tempo (Temporizador)")]
    public Image barraTempoPreenchimento; // Uma Image do tipo 'Filled'
    public TextMeshProUGUI textoNomeDaBarra; // Ex: "Tempo Restante"

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Atualiza o texto do topo baseado no número do turno e em quem está jogando.
    /// </summary>
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

    /// <summary>
    /// Atualiza visualmente a barra de tempo (valores entre 0.0f e 1.0f)
    /// </summary>
    public void AtualizarBarraTempo(float percentual, string nomeEstadoTempo)
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
}