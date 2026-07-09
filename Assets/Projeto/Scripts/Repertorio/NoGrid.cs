using UnityEngine;

public class NoGrid : MonoBehaviour
{
    public int x;
    public int y;

    // Indica qual simbolo esta ocupando este circulo agora
    public TipoSimbolo simboloAtual = TipoSimbolo.Nenhum;

    // Propriedade rapida para saber se o no ja tem um simbolo
    public bool estaOcupado => simboloAtual != TipoSimbolo.Nenhum;
}