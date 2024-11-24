using UnityEngine;

namespace Traps.Moving_Platform
{
    public class WaypointFollower : MonoBehaviour
    {
        [SerializeField] private GameObject[] waypoints;
        [SerializeField] private float speed = 2f;
        
        private int currentWaypointIndex;
        
        private void Update()
        {
            UpdateWaypointIndex();
            MoveTowardsWaypoint();
        }
        
        private void UpdateWaypointIndex()
        {
            if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }
        }

        private void MoveTowardsWaypoint()
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                waypoints[currentWaypointIndex].transform.position, 
                Time.deltaTime * speed);
        }
    }
}
