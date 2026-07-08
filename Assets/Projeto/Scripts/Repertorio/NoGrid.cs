using UnityEngine;

public class NoGrid : MonoBehaviour
{
    public int x; // Coluna na matriz
    public int y; // Linha na matriz

    // Indica qual simbolo esta ocupando este circulo agora
    public TipoSimbolo simboloAtual = TipoSimbolo.Nenhum;

    // Propriedade rapida para saber se o no ja tem um simbolo
    public bool estaOcupado => simboloAtual != TipoSimbolo.Nenhum;
}