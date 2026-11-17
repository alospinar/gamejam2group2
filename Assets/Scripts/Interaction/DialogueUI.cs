using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueLineText;

    private NPCInteraction currentNpc;
    private PlayerInteractor currentInteractor;
    private PlayerController currentPlayerController;
    private int currentLineIndex;

    public bool IsOpen => currentNpc != null;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void Open(NPCInteraction npc, PlayerInteractor interactor)
    {
        if (npc == null ||
            interactor == null ||
            npc.DialogueLines == null ||
            npc.DialogueLines.Length == 0)
            return;

        Close();

        currentNpc = npc;
        currentInteractor = interactor;
        currentPlayerController = interactor.GetComponent<PlayerController>();
        currentLineIndex = 0;
        currentInteractor.InteractPressed += Advance;
        currentInteractor.SetInteractionLock(true);
        if (currentPlayerController != null)
            currentPlayerController.SetMovementLocked(true);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        RefreshVisibleLine();
    }

    public void Advance()
    {
        if (!IsOpen)
            return;

        currentLineIndex++;
        if (currentLineIndex >= currentNpc.DialogueLines.Length)
        {
            Close();
            return;
        }

        RefreshVisibleLine();
    }

    public void Close()
    {
        if (currentInteractor != null)
        {
            currentInteractor.InteractPressed -= Advance;
            currentInteractor.SetInteractionLock(false);
        }

        if (currentPlayerController != null)
            currentPlayerController.SetMovementLocked(false);

        currentNpc = null;
        currentInteractor = null;
        currentPlayerController = null;
        currentLineIndex = 0;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void RefreshVisibleLine()
    {
        if (speakerNameText != null)
            speakerNameText.text = currentNpc.NpcName;

        if (dialogueLineText != null)
            dialogueLineText.text = currentNpc.DialogueLines[currentLineIndex];
    }
}
