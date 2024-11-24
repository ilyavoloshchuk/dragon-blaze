using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

namespace NPCs.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI NPCNameText;
        [SerializeField] private TextMeshProUGUI NPCDialogueText;
        [SerializeField] private float typeSpeed = 10f;
        [SerializeField] private AudioClip dialogueSound;
        
        public PlayerMovement playerMovement;
        
        private readonly Queue<string> paragraphs = new();
        private bool conversationEnded;
        private string p;
        private Coroutine typeDialogueCoroutine;
        private bool isTyping;
        private const float MaxTypeTime = 0.1f;
        
        public void DisplayNextParagraph(DialogueText dialogueText, AudioClip dialogueSound = null)
        {
            if (paragraphs.Count == 0)
            {
                if (!conversationEnded)
                {
                    StartConversation(dialogueText, dialogueSound);
                }
                else if (conversationEnded && !isTyping)
                {
                    EndConversation();
                    return;
                }
            }

            if (!isTyping)
            {
                p = paragraphs.Dequeue();
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
            }
            else
            {
                FinishParagraphEarly();
            }

            if (paragraphs.Count == 0)
            {
                conversationEnded = true;
            }
        }
        
        private void StartConversation(DialogueText dialogueText, AudioClip dialogueSound = null)
        {
            playerMovement.SetInteracting(true);
            playerMovement.enabled = false;

            if (dialogueSound != null)
            {
                SoundManager.instance.PlaySound(dialogueSound);
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            Time.timeScale = 1f;

            NPCNameText.text = dialogueText.SpeakerName;

            foreach (string paragraph in dialogueText.paragraphs)
            {
                paragraphs.Enqueue(paragraph);
            }
        }

        private void EndConversation()
        {
            playerMovement.SetInteracting(false);
            playerMovement.enabled = true;

            paragraphs.Clear();
            conversationEnded = false;

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            Time.timeScale = 1f;
        }

        private IEnumerator TypeDialogueText(string p)
        {
            isTyping = true;
            int maxVisibleChars = 0;

            NPCDialogueText.text = p;
            NPCDialogueText.maxVisibleCharacters = maxVisibleChars;        

            foreach (var unused in p)
            {
                maxVisibleChars++;
                NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

                yield return new WaitForSeconds(MaxTypeTime / typeSpeed);
            }

            isTyping = false;
        }

        private void FinishParagraphEarly()
        {
            StopCoroutine(typeDialogueCoroutine);

            NPCDialogueText.maxVisibleCharacters = p.Length;
            NPCDialogueText.text = p;
        
            isTyping = false;
        }
    }
}