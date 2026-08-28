using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximoLevel : MonoBehaviour
{
    private string CenaBatalha = "Batalha";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            SceneManager.LoadScene(CenaBatalha);
        }
    }
}
