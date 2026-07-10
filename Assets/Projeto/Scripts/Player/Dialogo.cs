using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogo : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Settings")]
    [SerializeField] private float timePerCharacter = 0.05f;
    [SerializeField] private float hideDelay = 2f;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private string diaologue;
    [SerializeField] private bool hideDialogue = true;

    private Coroutine typingCoroutine;
    private string currentFullText = "";
    private Coroutine hideCoroutine;

    // Property to check if text is currently animating
    public bool IsTyping { get; private set; }

    void Start() { ShowDialogue(diaologue); }
    private void Awake()
    {
        if (dialogueText == null)
            dialogueText = GetComponentInChildren<TextMeshProUGUI>();

        if (dialogueText == null)
            Debug.LogWarning($"Dialogo: TextMeshProUGUI reference not set on '{name}'. Assign it in the inspector or place a TMP component as a child.");

        if (dialoguePanel == null && dialogueText != null)
            dialoguePanel = dialogueText.transform.parent != null ? dialogueText.transform.parent.gameObject : dialogueText.gameObject;

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    /// <summary>
    /// Starts the typewriter effect for a line of dialogue.
    /// </summary>
    public void ShowDialogue(string text)
    {
        currentFullText = text;
        
        // Stop any currently running typing sequence
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Cancel any pending hide
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Ensure the panel is visible
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Disable player movement while dialog is showing
                

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        IsTyping = true;
        // Guard: ensure TMP reference is present
        if (dialogueText == null)
        {
            Debug.LogError("Dialogo: dialogueText is null. Cannot show dialogue.");
            IsTyping = false;
            yield break;
        }

        // Assign full text immediately so TMP can calculate correct word-wrapping bounds
        dialogueText.text = currentFullText;
        dialogueText.ForceMeshUpdate();
        // Wait one frame to ensure the mesh/textInfo is populated
        yield return null;

        int totalCharacters = dialogueText.textInfo.characterCount;
        int visibleCount = 0;

        while (visibleCount <= totalCharacters)
        {
            dialogueText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(timePerCharacter);
        }

        // Ensure all characters are visible at the end
        dialogueText.maxVisibleCharacters = totalCharacters;
        IsTyping = false;

        // Start hide timer after finished typing
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// Instantly completes the text reveal if the player clicks to skip.
    /// </summary>
    public void SkipToFullText()
    {
        if (!IsTyping) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText == null)
        {
            Debug.LogError("Dialogo: dialogueText is null. Cannot skip to full text.");
            IsTyping = false;
            return;
        }

        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        IsTyping = false;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (hideDialogue)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (questPanel != null)
                questPanel.SetActive(true);

            if (playerMovement != null)
                playerMovement.enabled = true; // Re-enable player movement after dialog is hidden
        }

        hideCoroutine = null;
    }
}