using System.Collections.Generic;
using UnityEngine;

public class RepertorioDados : MonoBehaviour
{
    public static RepertorioDados Instancia { get; private set; }

    // Lista que guarda os IDs de tudo o que o jogador já coletou
    private HashSet<string> referenciasColetadas = new HashSet<string>();

    void Awake()
    {
        // Garante que só exista um banco de dados no jogo inteiro
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Chamado pelo ItemInteragivel ao ser coletado
    public bool AdicionarReferencia(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;

        // HashSet.Add retorna false se o item já existia na lista
        if (referenciasColetadas.Add(id))
        {
            return true;
        }
        return false;
    }

    // Usado pelo menu para saber se deve exibir o item ou deixá-lo oculto/com interrogação
    public bool JaPossuiReferencia(string id)
    {
        return referenciasColetadas.Contains(id);
    }
    // Retorna a lista de todas as referencias coletadas
    public HashSet<string> ObterReferenciasColetadas()
    {
        return referenciasColetadas;
    }
}