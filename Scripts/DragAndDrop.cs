using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public Player player;
    private Rigidbody cubeRB;

    private void Start()
    {
        cubeRB = GetComponent<Rigidbody>();
    }

    private void OnMouseDrag()
    {
        cubeRB.isKinematic = false;
        Vector2 mousePosition = Input.mousePosition;
        Vector3 tempFingerPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.y - this.transform.position.y));
        if (player != null && player.gameController.state == GameController.State.INITIALISATION && !player.isReady())
        {
            cubeRB.velocity = (player.zone.GetComponent<BoxCollider>().ClosestPoint(tempFingerPos) + (player.zone.transform.position - this.transform.position).normalized * this.transform.localScale.x / 2f - this.transform.position) * 10;
        }
        
    }

    private void OnMouseUp()
    {
        if (player != null && player.gameController.state == GameController.State.INITIALISATION && !player.isReady())
        {
            cubeRB.isKinematic = true;
            cubeRB.velocity = Vector3.zero;
        }
        
    }
}
