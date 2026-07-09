using UnityEngine;

[CreateAssetMenu(fileName = "NovaReferencia", menuName = "Repertorio/Item de Referencia")]
public class ReferenciaData : ScriptableObject
{
    [Header("Identificacao Unica")]
    public string idUnico;

    [Header("Informacoes de UI")]
    public string nomeExibicao;
    [TextArea(3, 5)]
    public string descricaoDetalhada;
    public Sprite iconeUI;
    
    [Header("Cena do Domino")]
    public GameObject prefabPecaDomino;
}