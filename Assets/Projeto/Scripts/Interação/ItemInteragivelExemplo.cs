using UnityEngine;

public class ItemInteragivelExemplo : MonoBehaviour, IInteractable
{
    [Header("Configuracoes do Objeto")]
    public string nomeDoItem = "Arma do Chao";

    private Collider2D meuCollider;

    void Awake()
    {
        // Garante que o collider seja um Trigger
        meuCollider = GetComponent<Collider2D>();
    }

    public void Interagir()
    {
        Debug.Log("Voce interagiu com: " + nomeDoItem);

        if (AparecerUI.Instancia != null)
        {
            AparecerUI.Instancia.MostrarNotificacao(nomeDoItem);
        }

        DesativarInteracao();
    }
    private void DesativarInteracao()
    {
        // Desativa o colisor
        
        meuCollider.enabled = false;
        this.enabled = false;
        Debug.Log($"O script de interação de '{gameObject.name}' foi desativado.");
    }
}