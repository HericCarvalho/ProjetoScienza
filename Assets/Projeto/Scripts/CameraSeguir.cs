using UnityEngine;

public class CameraSeguir : MonoBehaviour
{
    [Header("Alvo")]
    public Transform alvo;

    [Header("Configurações de Movimento")]
    public float suavidade = 5f;
    public float offsetZ = -10f;

    void FixedUpdate()
    {
        if (alvo == null) return;

        Vector3 posicaoAlvo = new Vector3(alvo.position.x, alvo.position.y, offsetZ);
        Vector3 posicaoSuave = Vector3.Lerp(transform.position, posicaoAlvo, suavidade * Time.fixedDeltaTime);

        transform.position = posicaoSuave;
    }
}
//fim do script, agora rebola pra mim, faz o favor.