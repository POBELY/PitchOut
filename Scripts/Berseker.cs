using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berseker : PawnController
{

    private new void Start()
    {
        base.Start();
        role = GameController.Role.BERSERKER;
    }
}
