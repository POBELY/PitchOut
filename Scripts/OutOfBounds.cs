using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Pawn"))
        {
            PawnController pawn = other.gameObject.GetComponent<PawnController>();
            if (pawn.isBerseker() && pawn.gameController.currentPlayer.team == pawn.team && pawn.gameController.pawn == pawn)
            {
                float positionY = pawn.transform.position.y;
                Vector3 newPosition = pawn.player.zone.GetComponent<BoxCollider>().transform.position;
                newPosition.y = positionY;
                other.gameObject.transform.position = newPosition;
                pawn.getRigidBody().velocity = Vector3.zero;               
                pawn.player.desactivateRoleButtons();
                pawn.player.readyButton();
                pawn.player._readyButton.gameObject.SetActive(true);
                pawn.gameController.bluePlayer.kinematic();
                pawn.gameController.redPlayer.kinematic();
                foreach (GameObject obstacle in pawn.gameController.obstacles)
                {
                    obstacle.GetComponent<Rigidbody>().isKinematic = true;
                }
                pawn.gameController.state = GameController.State.BERSEKER;
            } else
            {
                pawn.destroy();
                if (pawn.gameController.state == GameController.State.SECONDSHOOTED || pawn.gameController.state == GameController.State.SECONDSHOT)
                {
                    pawn.gameController.state = GameController.State.SHOOTED;
                }
                
            }
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
