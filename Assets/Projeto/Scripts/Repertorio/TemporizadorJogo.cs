using UnityEngine;
using TMPro;

public class TemporizadorJogo : MonoBehaviour
{
    public static TemporizadorJogo Instancia { get; private set; }

    [Header("Configura��es de Tempo")]
    public float tempoPorTurno = 30f;
    private float tempoAtual;
    private bool cronometroAtivo = false;

    [Header("UI do Temporizador")]
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

    private void AtuaIizarVisualTemporizador() { } // Mantido oculto para focar na l�gica

    private void AtualizarVisualTemporizador()
    {
        if (textoTemporizador != null)
        {
            textoTemporizador.text = Mathf.CeilToInt(tempoAtual).ToString() + "s";
            textoTemporizador.color = (tempoAtual <= 5f) ? Color.red : Color.white;
        }
    }

    private void TempoEsgotado()
    {
        Debug.LogWarning("[TEMPO] Limpando pe�as do jogador e trocando turno.");
        if (TabuleiroManager.Instancia != null)
        {
            TabuleiroManager.Instancia.ForçarDerrotaPorTempo();
        }
    }
}