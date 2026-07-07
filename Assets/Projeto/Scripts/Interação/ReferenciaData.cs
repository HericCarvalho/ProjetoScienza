using UnityEngine;

[CreateAssetMenu(fileName = "NovaReferencia", menuName = "Repertorio/Item de Referencia")]
public class ReferenciaData : ScriptableObject
{
    [Header("Identificação Única")]
    [Tooltip("ID único em texto para salvar no sistema (ex: fita_cassete_01). Não use espaços.")]
    public string idUnico;

    [Header("Informações de UI")]
    public string nomeExibicao;
    [TextArea(3, 5)]
    public string descricaoDetalhada;
    public Sprite iconeUI;
}