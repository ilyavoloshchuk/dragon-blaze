using NPCs.Dialogue;
using UnityEngine;

namespace NPCs.Npc_s
{
    public class Lumina : Npc
    {
        [SerializeField] private DialogueText dialogueText;
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioClip dialogueSound;

        protected override void Interact()
        {
            Talk(dialogueText);
        }

        private void Talk(DialogueText dialogueText)
        {
            dialogueController.DisplayNextParagraph(dialogueText, dialogueSound);
        }
    }
}
