using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemInteragivel : MonoBehaviour, IInteractable
{
    [Header("Dados da Referência")]
    [Tooltip("Arraste aqui o arquivo ScriptableObject correspondente a este item.")]
    public ReferenciaData dadosDaReferencia;

    private Collider2D meuCollider;

    void Awake()
    {
        meuCollider = GetComponent<Collider2D>();
        meuCollider.isTrigger = true;
    }

    public void Interagir()
    {
        if (dadosDaReferencia == null) return;

        if (RepertorioDados.Instancia != null)
        {
            RepertorioDados.Instancia.AdicionarReferencia(dadosDaReferencia.idUnico);
        }

        // Dispara a interface
        if (GerenciadorInterface.Instancia != null)
        {
            GerenciadorInterface.Instancia.MostrarNotificacao(dadosDaReferencia.nomeExibicao, dadosDaReferencia.iconeUI);
        }

        DesativarInteracao();
    }

    private void DesativarInteracao()
    {
        if (meuCollider != null) meuCollider.enabled = false;
        this.enabled = false;
    }
}