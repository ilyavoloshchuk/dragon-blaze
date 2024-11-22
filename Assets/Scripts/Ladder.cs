using System.Collections;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private float _speed = 100f;
    [SerializeField] private Collider2D _ladderCollider; 

    private void Awake()
    {
        if (_ladderCollider == null)
            _ladderCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        if (!other.TryGetComponent(out PlayerMovement _)) return;
        
        if (other.transform.position.y > _ladderCollider.bounds.max.y)
        {
            other.GetComponent<Rigidbody2D>().gravityScale = 3;
            _ladderCollider.enabled = false;
            StartCoroutine(MovePlayerInsideLadder());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement _)) return;

        var rbComponent = other.GetComponent<Rigidbody2D>();
        if (rbComponent == null) return;
        
        rbComponent.gravityScale = 0;
        
        var vertical = Input.GetAxis("Vertical");

        rbComponent.velocity = vertical != 0 
            ? new Vector2(rbComponent.velocity.x, vertical * _speed * Time.deltaTime) 
            : new Vector2(rbComponent.velocity.x, 0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement _)) return;
        
        var rbComponent = other.GetComponent<Rigidbody2D>();
        if (rbComponent != null)
            rbComponent.gravityScale = 3;
    }
    
    private IEnumerator MovePlayerInsideLadder()
    {
        yield return new WaitForSeconds(1f);
        _ladderCollider.enabled = true;
    }
}