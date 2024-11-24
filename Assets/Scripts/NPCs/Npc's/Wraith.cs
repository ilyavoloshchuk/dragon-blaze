using NPCs.Dialogue;
using UnityEngine;

namespace NPCs.Npc_s
{
    public class Wraith : Npc
    {
        [SerializeField] private DialogueText dialogueText;
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioClip dialogueSound;

        protected override void Interact()
        {
            dialogueController.DisplayNextParagraph(dialogueText, dialogueSound);
        }
    }
}
