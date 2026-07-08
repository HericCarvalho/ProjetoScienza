using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastarPeca : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Camera cam;
    private Vector3 offset;
    private Vector3 posicaoOriginal;
    private PecaDomino peca;

    [Header("Configuracao de Encaixe")]
    [Tooltip("Quanto MENOR este numero, mais perto do circulo a peca precisa estar para grudar. Tente valores entre 0,2 e 0,5.")]
    public float distanciaMinimaEncaixe = 0.3f; // Criado para acesso facil no Inspector

    void Awake()
    {
        cam = Camera.main;
        peca = GetComponent<PecaDomino>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (peca != null && peca.foiPosicionada) return;

        posicaoOriginal = transform.position;

        Vector3 posicaoMouse = cam.ScreenToWorldPoint(eventData.position);
        posicaoMouse.z = 0f;
        offset = transform.position - posicaoMouse;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (peca != null && peca.foiPosicionada) return;

        Vector3 posicaoMouse = cam.ScreenToWorldPoint(eventData.position);
        posicaoMouse.z = 0f;
        transform.position = posicaoMouse + offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (peca != null && peca.foiPosicionada) return;

        
        NoGrid noMaisProximo = TabuleiroManager.Instancia.ObterNoMaisProximo(transform.position, distanciaMinimaEncaixe);

        if (noMaisProximo != null)
        {
            Debug.Log("No encontrado perto do soltar: " + noMaisProximo.name);
            bool encaixou = TabuleiroManager.Instancia.TentarPosicionarPeca(peca, noMaisProximo);

            if (!encaixou)
            {
                transform.position = posicaoOriginal;
            }
        }
        else
        {
            // Se o mouse estiver alem da distancia configurada, ela volta direto para a mao
            Debug.LogWarning("Soltou longe demais do grid!");
            transform.position = posicaoOriginal;
        }
    }
}