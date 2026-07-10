using UnityEngine;
using TMPro; 

public class ControladorUI : MonoBehaviour
{
    public static ControladorUI Instancia { get; private set; }

    [Header("Componentes de UI")]
    [SerializeField] private TextMeshProUGUI textoContadorTMP;

    [Header("Configuração de Texto")]
    [SerializeField] private string prefixoTexto = "Símbolos Combinados: ";

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

    public void AtualizarTextoContador(int novoTotal)
    {
        if (textoContadorTMP != null)
        {
            textoContadorTMP.text = prefixoTexto + novoTotal.ToString();
        }
        else
        {
            Debug.LogWarning("[UI] O componente TextMeshProUGUI não foi arrastado no Inspector!");
        }
    }
}