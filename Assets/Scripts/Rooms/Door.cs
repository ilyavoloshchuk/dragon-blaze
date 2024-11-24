using UnityEngine;

namespace Rooms
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room room;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                ActivateConnectedRoom();
            }
        }

        private void ActivateConnectedRoom()
        {
            room.ActivateRoom(true);
        }
    }
}
