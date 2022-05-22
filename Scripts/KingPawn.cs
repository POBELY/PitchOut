using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPawn : PawnController
{
    public GameController.Role roleCopy;

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        role = GameController.Role.KING;
        roleCopy = GameController.Role.DEFAULT;
    }

    public override bool isBerseker()
    {
        return roleCopy == GameController.Role.BERSERKER;
    }

    public override bool isDoubleTap()
    {
        return roleCopy == GameController.Role.DOUBLETAP;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (roleCopy == GameController.Role.DOUBLETAP)
        {
            if (collision.gameObject.CompareTag("Pawn") && gameController.getPawn() == this && gameController.state == GameController.State.SECONDSHOOTED)
            {
                gameController.state = GameController.State.SHOOTED;
            }
        }
        else if (roleCopy == GameController.Role.ASSASSIN)
        {
            if (collision.gameObject.CompareTag("Pawn"))
            {
                PawnController pawnControllerCollision = collision.gameObject.GetComponent<PawnController>();
                if (pawnControllerCollision.team != team && gameController.currentPlayer.team == team && gameController.getPawn() == (PawnController)this)
                {
                    pawnControllerCollision.destroy();
                }
            }
        }
    }

}
