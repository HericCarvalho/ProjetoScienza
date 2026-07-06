using UnityEngine;

public class ItemInteragivelExemplo : MonoBehaviour, IInteractable
{
    [Header("Configurações do Objeto")]
    public string nomeDoItem = "Arma do Chão";

    void Awake()
    {
        // Garante que o collider seja um Trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interagir()
    {
        Debug.Log("Você interagiu com: " + nomeDoItem);

        Destroy(gameObject);
    }
}