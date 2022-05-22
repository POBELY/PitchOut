using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTapPawn : PawnController
{

    private new void Start()
    {
        base.Start();
        role = GameController.Role.DOUBLETAP;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Pawn") && gameController.getPawn() == this && gameController.state == GameController.State.SECONDSHOOTED)
        {
            gameController.state = GameController.State.SHOOTED;
        }
    }


}
