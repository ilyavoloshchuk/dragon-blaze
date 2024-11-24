using UnityEngine;

namespace NPCs.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
    public class DialogueText : ScriptableObject
    {
        public string speakerName;

        [TextArea(5, 10)]
        public string[] paragraphs;
    }
}
