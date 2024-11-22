using NPCs.Dialogue;
using UnityEngine;

namespace NPCs.Npc_s
{
    public class Emberon : Npc
    {
        [SerializeField] private DialogueText dialogueText;
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioClip dialogueSound;

        protected override void Interact()
        {
            Talk(dialogueText);
        }

        public void Talk(DialogueText dialogueText)
        {
            dialogueController.DisplayNextParagraph(dialogueText, dialogueSound);
        }
    }
}