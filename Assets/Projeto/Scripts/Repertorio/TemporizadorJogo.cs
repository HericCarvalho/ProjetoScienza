using UnityEngine;
using TMPro; // Necessário para usar o TextMeshPro

public class TemporizadorJogo : MonoBehaviour
{
    public static TemporizadorJogo Instancia { get; private set; }

    [Header("Configurações de Tempo")]
    public float tempoPorTurno = 30f;
    private float tempoAtual;
    private bool cronometroAtivo = false;

    [Header("UI do Temporizador")]
    [Tooltip("Arraste o componente de texto do seu contador de tempo aqui")]
    public TextMeshProUGUI textoTemporizador;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!cronometroAtivo) return;

        if (tempoAtual > 0)
        {
            tempoAtual -= Time.deltaTime;
            AtualizarVisualTemporizador();
        }
        else
        {
            tempoAtual = 0f;
            cronometroAtivo = false;
            AtualizarVisualTemporizador();
            TempoEsgotado();
        }
    }

    public void IniciarTemporizador()
    {
        tempoAtual = tempoPorTurno;
        cronometroAtivo = true;
    }

    public void PararTemporizador()
    {
        cronometroAtivo = false;
    }

    private void AtualizarVisualTemporizador()
    {
        if (textoTemporizador != null)
        {
            textoTemporizador.text = Mathf.CeilToInt(tempoAtual).ToString() + "s";

            if (tempoAtual <= 5f)
            {
                textoTemporizador.color = Color.red;
            }
            else
            {
                textoTemporizador.color = Color.white;
            }
        }
    }

    private void TempoEsgotado()
    {
        Debug.LogWarning("[TEMPO] O tempo acabou! Passando a rodada para o inimigo.");
        if (TabuleiroManager.Instancia != null)
        {
            TabuleiroManager.Instancia.PassarRodadaParaInimigo();
        }
    }

    public float ObterTempoAtual() => tempoAtual;
}