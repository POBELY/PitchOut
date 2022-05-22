using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardPawn : PawnController
{
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        role = GameController.Role.GUARD;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
