using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GerenciadorDePartida : MonoBehaviour
{
    public static GerenciadorDePartida Instancia { get; private set; }

    public List<ReferenciaData> todasAsReferenciasDoJogo;
    public Transform localDaMao;
    public float espacamentoMao = 1.5f;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SpawnarPecasDoJogador();
    }

    // Método principal chamado pelo TabuleiroManager
    public void SpawnarPecasDoJogador()
    {
        // Executa a limpeza e o spawn de forma sequencial e protegida contra o mesmo frame
        StartCoroutine(RotinaRespawnMao());
    }

    private IEnumerator RotinaRespawnMao()
    {
        // 1. Destrói estritamente as peças antigas que restaram soltas na cena e NÃO foram fixadas
        PecaDomino[] todasAsPecas = FindObjectsByType<PecaDomino>(FindObjectsSortMode.None);
        foreach (PecaDomino peca in todasAsPecas)
        {
            if (peca != null && !peca.foiPosicionada && !peca.name.Contains("(Inimigo)"))
            {
                Destroy(peca.gameObject);
            }
        }

        // Aguarda o fim do frame para garantir que a Unity limpou a memória dessas peças antigas
        yield return new WaitForEndOfFrame();

        if (RepertorioDados.Instancia == null)
        {
            Debug.LogError("[GERENCIADOR] RepertorioDados.Instancia não foi encontrado na cena!");
            yield break;
        }

        HashSet<string> IDsColetados = RepertorioDados.Instancia.ObterReferenciasColetadas();

        // Mensagem de depuração para verificar se o repertório perdeu os dados no reset do turno
        if (IDsColetados == null || IDsColetados.Count == 0)
        {
            Debug.LogWarning("[GERENCIADOR] O RepertorioDados retornou 0 IDs coletados. Nenhuma peça será criada.");
        }

        int indicePeca = 0;

        foreach (ReferenciaData dados in todasAsReferenciasDoJogo)
        {
            if (dados == null) continue;

            if (IDsColetados.Contains(dados.idUnico) && dados.prefabPecaDomino != null)
            {
                Vector3 posicaoSpawn = localDaMao.position + new Vector3(indicePeca * espacamentoMao, 0f, 0f);

                GameObject novaPeca = Instantiate(dados.prefabPecaDomino, posicaoSpawn, Quaternion.identity);
                novaPeca.name = $"Peca_{dados.idUnico}";

                PecaDomino domino = novaPeca.GetComponent<PecaDomino>();
                if (domino != null)
                {
                    domino.foiPosicionada = false; // Garante o estado inicial livre
                }

                indicePeca++;
            }
        }

        Debug.Log($"[GERENCIADOR] Fluxo concluído. {indicePeca} peças geradas na mão do jogador.");
    }
}