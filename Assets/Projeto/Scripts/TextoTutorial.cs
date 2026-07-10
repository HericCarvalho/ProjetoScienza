using UnityEngine;

public class TextoTutorial : MonoBehaviour
{
    [SerializeField] private GameObject proximaCaixa;
    [SerializeField] private GameObject proximaCamera;
    [SerializeField] private GameObject assetsAdicionais;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (proximaCaixa != null)
            {
                proximaCaixa.SetActive(true);
                
            }
            if (proximaCamera != null)
            {
                proximaCamera.SetActive(true);
            }
            if (assetsAdicionais != null)
            {
                assetsAdicionais.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
}
