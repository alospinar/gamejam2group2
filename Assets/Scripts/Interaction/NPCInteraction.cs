using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private DialogueUI dialogueUI;

    public string NpcName => npcName;
    public string[] DialogueLines => dialogueLines;
    public string InteractionPrompt => "E - Hablar";

    public bool CanInteract(PlayerInteractor interactor)
    {
        return dialogueUI != null && dialogueLines != null && dialogueLines.Length > 0;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (dialogueUI != null && !dialogueUI.IsOpen)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayNpcInteraction();

            dialogueUI.Open(this, interactor);
        }
    }

    public void StopInteracting(PlayerInteractor interactor)
    {
    }
}
