using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdversarioScriptado : MonoBehaviour
{
    public static AdversarioScriptado Instancia { get; private set; }

    [System.Serializable]
    public struct JogadaScriptada
    {
        public GameObject prefabPeca;
        public int coordenadaX;
        public int coordenadaY;
    }

    [System.Serializable]
    public struct TurnoInimigo
    {
        [Header("Configuração do Turno")]
        public List<JogadaScriptada> jogadasDoTurno;
    }

    [Header("Cronograma de Múltiplos Turnos")]
    public List<TurnoInimigo> listaDeTurnos = new List<TurnoInimigo>();

    [Header("Configurações de Tempo")]
    public float tempoEntreJogadas = 1.2f;

    private int indiceTurnoAtual = 0;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void IniciarTurnoInimigo()
    {
        if (indiceTurnoAtual < listaDeTurnos.Count)
        {
            StartCoroutine(RotinaTurnoInimigo(listaDeTurnos[indiceTurnoAtual]));
            indiceTurnoAtual++;
        }
        else
        {
            Debug.LogWarning("[INIMIGO] Todos os turnos scriptados acabaram! Limpando e reiniciando ciclo.");
            TabuleiroManager.Instancia.SimularFimTurnoInimigo();
        }
    }

    private IEnumerator RotinaTurnoInimigo(TurnoInimigo dadosTurno)
    {
        yield return new WaitForSeconds(0.5f);

        var campoDicionario = typeof(TabuleiroManager).GetField("dicionarioGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dicionarioGrid = (Dictionary<Vector2Int, NoGrid>)campoDicionario.GetValue(TabuleiroManager.Instancia);

        for (int i = 0; i < dadosTurno.jogadasDoTurno.Count; i++)
        {
            JogadaScriptada jogada = dadosTurno.jogadasDoTurno[i];

            if (jogada.prefabPeca == null) continue;

            Vector2Int coordenadaAlvo = new Vector2Int(jogada.coordenadaX, jogada.coordenadaY);

            if (dicionarioGrid.TryGetValue(coordenadaAlvo, out NoGrid noAlvo))
            {
                GameObject novaPecaObj = Instantiate(jogada.prefabPeca, noAlvo.transform.position, Quaternion.identity);
                novaPecaObj.name += "(Inimigo)"; // Identificador de pontuação

                PecaDomino pecaScript = novaPecaObj.GetComponent<PecaDomino>();

                if (pecaScript != null)
                {
                    // Usa temporariamente o validador
                    bool sucesso = TabuleiroManager.Instancia.TentarPosicionarPeca(pecaScript, noAlvo);
                    if (!sucesso) Destroy(novaPecaObj);
                }
                else
                {
                    Destroy(novaPecaObj);
                }
            }

            yield return new WaitForSeconds(tempoEntreJogadas);
        }

        // Finaliza chamando a limpeza gradual idêntica à do jogador
        TabuleiroManager.Instancia.SimularFimTurnoInimigo();
    }

    public void ResetarContadorTurnos()
    {
        indiceTurnoAtual = 0;
    }
}