using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Dialogo typewriter;

    void Update()
    {
        // Example: Press Spacebar to interact
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (typewriter.IsTyping)
            {
                // If it's typing, finish the line instantly
                typewriter.SkipToFullText();
            }
            else
            {
                // If it's done, show the next line of dialogue
                typewriter.ShowDialogue("Hello adventurer! Welcome to the kingdom.");
            }
        }
    }
}