using UnityEngine;

public class PecaDomino : MonoBehaviour
{
    [System.Serializable]
    public struct SubCirculo
    {
        public Vector2Int posicaoRelativa;
        public TipoSimbolo simbolo;
    }

    public SubCirculo[] circulosDaPeca = new SubCirculo[3];

    public bool foiPosicionada = false;
}