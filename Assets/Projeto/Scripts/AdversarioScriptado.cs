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
        if (TabuleiroManager.Instancia == null) return;

        int indiceDoTurnoDela = TabuleiroManager.Instancia.numeroDoTurnoAtual - 1;

        Debug.Log($"[IA MADU] Lendo jogadas para o {TabuleiroManager.Instancia.numeroDoTurnoAtual}º Turno (Índice da Lista: {indiceDoTurnoDela})");

        if (indiceDoTurnoDela >= 0 && indiceDoTurnoDela < listaDeTurnos.Count)
        {
            StartCoroutine(RotinaTurnoInimigo(listaDeTurnos[indiceDoTurnoDela]));
        }
        else
        {
            Debug.LogWarning("[INIMIGO] Nenhuma jogada programada encontrada para este turno. Passando vez.");
            // Modifique a última linha do turno da Madu para chamar esta função específica:
            if (TabuleiroManager.Instancia != null)
            {
                // Usamos Invoke para dar um pequeno respiro de 1 segundo após ela colocar a última peça, para ficar natural
                TabuleiroManager.Instancia.Invoke("FinalizarTurnoInimigoCompleto", 1.0f);
            }
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