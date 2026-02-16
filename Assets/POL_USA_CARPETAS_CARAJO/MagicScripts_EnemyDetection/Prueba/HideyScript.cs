using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HideyScript : MonoBehaviour
{
    [SerializeField] private Movement _move;
    public Rigidbody2D agnes;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.H))
        {
            if (!_move.isHiding)
            {
                Hide();
            }
            else
            {
                Unhide();
            }
        }
        if (_move.isHiding == true)
        {
            agnes.transform.position = transform.position;
            agnes.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }

    private void Hide()
    {
        _move.isHiding = true;

    }

    private void Unhide()
    {
        _move.isHiding = false;
        agnes.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

