using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    public GameObject pawn;
    private Rigidbody pawnRB;
    public GameObject linePrefab;
    public GameObject currentLine;
    public float force;
    public float maxDistLine;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        pawnRB = pawn.GetComponent<Rigidbody>();
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, pawn.transform.position);
        lineRenderer.SetPosition(1, pawn.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, pawn.transform.position);
        Vector2 mousePosition = Input.mousePosition;
        Vector3 tempFingerPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.y - pawn.transform.position.y));
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(pawn.transform.position, tempFingerPos) > maxDistLine)
            {
                lineRenderer.SetPosition(1, pawn.transform.position + (tempFingerPos - pawn.transform.position).normalized * maxDistLine);
            }
            else
            {
                lineRenderer.SetPosition(1, tempFingerPos);
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 line = lineRenderer.GetPosition(0) - lineRenderer.GetPosition(1);
            pawnRB.AddForce(force * line, ForceMode.Impulse);
            lineRenderer.SetPosition(1, pawn.transform.position); // Destroy plutot !!!
        }
    }
}