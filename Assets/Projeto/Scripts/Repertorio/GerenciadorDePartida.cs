using UnityEngine;
using System.Collections.Generic;

public class GerenciadorDePartida : MonoBehaviour
{
    public List<ReferenciaData> todasAsReferenciasDoJogo;
    public Transform localDaMao;
    public float espacamentoMao = 1.5f;

    void Start()
    {
        SpawnarPecasDoJogador();
    }

    void SpawnarPecasDoJogador()
    {
        if (RepertorioDados.Instancia == null) return;

        HashSet<string> IDsColetados = RepertorioDados.Instancia.ObterReferenciasColetadas();
        int indicePeca = 0;

        foreach (ReferenciaData dados in todasAsReferenciasDoJogo)
        {
            if (IDsColetados.Contains(dados.idUnico) && dados.prefabPecaDomino != null)
            {
                Vector3 posicaoSpawn = localDaMao.position + new Vector3(indicePeca * espacamentoMao, 0f, 0f);

                GameObject novaPeca = Instantiate(dados.prefabPecaDomino, posicaoSpawn, Quaternion.identity);
                novaPeca.name = $"Peca_{dados.idUnico}";

                indicePeca++;
            }
        }
    }
}