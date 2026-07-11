using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RepertorioDados : MonoBehaviour
{
    public static RepertorioDados Instancia { get; private set; }

    // Lista que guarda os IDs de tudo o que o jogador j� coletou
    private HashSet<string> referenciasColetadas = new HashSet<string>();
    [SerializeField] private int quantidadeReferenciasColetadas = 0;
    public bool podeColetar = true;
    public GameObject notificação2;
    public GameObject quest;

    void Awake()
    {
        // Garante que s� exista um banco de dados no jogo inteiro
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

    void Update()
    {
        // Apenas para debug, mostra a quantidade de itens coletados no console
        if (quantidadeReferenciasColetadas >= 6)
        {
          podeColetar = false;
         
            if (quest != null)
            {
                quest.SetActive(false);
            }

             if (notificação2 != null)
           StartCoroutine(MostrarNotificacao(2f));
        }
    }

    public IEnumerator MostrarNotificacao(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!podeColetar)
        {
            if (notificação2 != null)
          notificação2.SetActive(true);
        }
        yield break;
        
    }

    // Chamado pelo ItemInteragivel ao ser coletado
    public bool AdicionarReferencia(string id)
    {
        if (string.IsNullOrEmpty(id) || !podeColetar) return false;

        // HashSet.Add retorna false se o item j� existia na lista
        if (referenciasColetadas.Add(id))
        {
            quantidadeReferenciasColetadas++;
            return true;
        }
        return false;
    }

    // Usado pelo menu para saber se deve exibir o item ou deix�-lo oculto/com interroga��o
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