using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinPawn : PawnController
{
    private new void Start()
    {
        base.Start();
        role = GameController.Role.ASSASSIN;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Pawn"))
        {
            PawnController pawnControllerCollision = collision.gameObject.GetComponent<PawnController>();          
            if (pawnControllerCollision.team != team && gameController.currentPlayer.team == team && gameController.getPawn() == this)
            {
                pawnControllerCollision.destroy();
            }
        }
    }
}
