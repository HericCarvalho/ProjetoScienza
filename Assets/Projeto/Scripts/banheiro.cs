using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class banheiro : MonoBehaviour
{
    [SerializeField] private GameObject luzBanheiro; // Referência à luz do banheiro
    [SerializeField] private Light2D luz; // Referência à luz 2D

    void Start()
    {
        luz = luzBanheiro.GetComponent<Light2D>();
    }
  
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            luz.intensity = 1f; // Aumenta a intensidade da luz do banheiro
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            luz.intensity = 0f; // Diminui a intensidade da luz do banheiro
        }
    }
   
}
