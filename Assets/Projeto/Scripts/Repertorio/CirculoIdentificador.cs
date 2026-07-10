using UnityEngine;

public class CirculoIdentificador : MonoBehaviour
{
    [Tooltip("Defina na Unity: 0 para o Centro, 1 para Asa Esquerda, 2 para Asa Direita (de acordo com o seu array circulosDaPeca)")]
    public int indiceLogico;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}