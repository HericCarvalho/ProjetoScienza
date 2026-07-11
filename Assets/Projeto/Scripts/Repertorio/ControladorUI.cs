using UnityEngine;
using TMPro; 

public class ControladorUI : MonoBehaviour
{
    public static ControladorUI Instancia { get; private set; }

    [Header("Contadores de Pontos")]
    public TMPro.TextMeshProUGUI textoPontosJogador;
    public TMPro.TextMeshProUGUI textoPontosInimigo;


    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AtualizarTextoContador(0);
    }

    public void AtualizarTextoContador(int pontos)
    {
        if (textoPontosJogador != null)
        {
            textoPontosJogador.text = "Jogador: " + pontos;
        }
    }

    public void AtualizarTextoContadorInimigo(int pontos)
    {
        if (textoPontosInimigo != null)
        {
            textoPontosInimigo.text = "Inimigo: " + pontos;
        }
    }

}