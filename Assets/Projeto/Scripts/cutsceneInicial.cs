using UnityEngine;

public class cutsceneInicial : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject colisor;
    [SerializeField] private GameObject transformDaCutscene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerMovement.enabled = false; // Disable player movement at the start of the cutscene      
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player.transform.position = Vector3.Lerp(player.transform.position, transformDaCutscene.transform.position, Time.fixedDeltaTime * 1f);
        StartCoroutine(Dialogo(1f));
    }

    System.Collections.IEnumerator Dialogo(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialoguePanel.SetActive(true);
        colisor.SetActive(true);
        this.enabled = false; // Disable this script to stop the Update loop
        
    }
    
}
