using UnityEngine;
using UnityEngine.InputSystem;

public class MagicObtaining : MonoBehaviour
{
    private bool playerInRange;
    private GameObject player;
    public enum magicToGrant
    {
        Wind, Fire, Water, Electric
    }

    public magicToGrant givenMagic;
    private InputAction InputAction;

    private void Update()
    {
        if (playerInRange && InputAction.WasPressedThisFrame())
        {
            switch (givenMagic)
            {
                case magicToGrant.Wind:
                    player.GetComponent<WindMagic>().enabled = true;
                    this.gameObject.SetActive(false);
                    break;
                case magicToGrant.Fire:
                    player.GetComponent<FireMagic>().enabled = true;
                    this.gameObject.SetActive(false);
                    break;
                case magicToGrant.Water:
                    player.GetComponent<WaterMagic>().enabled = true;
                    this.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = true;
        player = collision.gameObject;
        InputAction = player.GetComponent<PlayerInput>().actions["Interact"];
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
}
