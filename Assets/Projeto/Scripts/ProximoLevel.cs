using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximoLevel : MonoBehaviour
{
    private int ProximaScena;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ProximaScena = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(ProximaScena);
        }
    }
}
