using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PawnController : MonoBehaviour
{
    private Rigidbody pawnRB;
    public GameController gameController;
    public Player player;

    public GameController.Team team;
    public GameController.Role role;
    public GameObject selectionRing;
    public GameObject shootRing;

    // Start is called before the first frame update
    protected void Start()
    {
        pawnRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDrag()
    {
        pawnRB.isKinematic = false;
        gameController.setPawn(this);
        Vector2 mousePosition = Input.mousePosition;
        Vector3 tempFingerPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.y - this.transform.position.y));
        if ((gameController.state == GameController.State.INITIALISATION || (gameController.state == GameController.State.BERSEKER && isBerseker())) && !player.isReady())
        {
            pawnRB.velocity = (player.zone.GetComponent<BoxCollider>().ClosestPoint(tempFingerPos) + (player.zone.transform.position - this.transform.position).normalized * this.transform.localScale.x / 2f - this.transform.position) * 10;
        }
    }

    private void OnMouseUp()
    {
        if ((gameController.state == GameController.State.INITIALISATION || (gameController.state == GameController.State.BERSEKER && isBerseker())) && !player.isReady())
        {
            pawnRB.isKinematic = true;
            pawnRB.velocity = Vector3.zero;
        }

            
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if ((gameController.state == GameController.State.INITIALISATION || (gameController.state == GameController.State.BERSEKER && isBerseker())) && !player.isReady())
        {
            pawnRB.velocity = Vector3.zero;
        } else if (collision.gameObject.CompareTag("Pawn") || collision.gameObject.CompareTag("Obstacle"))
        {
            gameController.pawnCollisionAudio.Play(0);
        }
    }

    private void OnMouseDown()
    {
        if (gameController.state == GameController.State.LOCK && team == gameController.currentPlayer.team)
        {
            gameController.switchPawn(this);
        }
    }

    public void destroy()
    {
        if (role == GameController.Role.KING && !player.destroyedPawnsRole.Contains(GameController.Role.GUARD))
        {
            PawnController guardPawn = player.getPawn(GameController.Role.GUARD);
            this.getRigidBody().velocity = Vector3.zero;
            this.transform.position = guardPawn.transform.position;
            guardPawn.destroy();
            
        } else
        {
            this.gameObject.SetActive(false);
            this.getRigidBody().velocity = Vector3.zero;
            Color newColor = player.roleButtons[role].image.material.color;
            newColor.a = 1f;
            player.roleButtons[role].image.material.color = newColor;
            player.destroyPawn(this);
        }
    }

    public void unDestroy()
    {
        

        Color newColor = player.roleButtons[role].image.material.color;
        newColor.a = 0.5f;
        player.roleButtons[role].image.material.color = newColor;

        player.unDestroyPawn(this);
        this.gameObject.SetActive(true);
    }

    public void locking()
    {
        var ringRenderer = selectionRing.GetComponent<Renderer>();
        ringRenderer.material.SetColor("_Color", (Color.red + Color.magenta) / 1.5f);
    }

    public void unlocking()
    {
        var ringRenderer = selectionRing.GetComponent<Renderer>();
        ringRenderer.material.SetColor("_Color", Color.yellow);
    }

    public void buttonPressed()
    {
        if (gameController.getPawn().role == GameController.Role.GUARD && gameController.getPawn().team == team  && player.destroyedPawnsRole.Contains(role))
        {
            PawnController guardPawn = gameController.getPawn();
            this.transform.position = guardPawn.transform.position;
            for (int i = 0; i < player.destroyedPawnsRole.Count; ++i)
            {
                if (player.destroyedPawnsRole[i] == role)
                {
                    player.destroyedPawnsRole.RemoveAt(i);
                    break;
                }
            }
            guardPawn.destroy();
            this.selectionRing.SetActive(true);    
            this.unDestroy();
            gameController.switchPawn(this);
        } else if (gameController.getPawn().role == GameController.Role.KING && gameController.getPawn().team == team && player.destroyedPawnsRole.Contains(role))
        {
            KingPawn king = (KingPawn)gameController.getPawn();
            if (king.roleCopy == role)
            {
                king.roleCopy = GameController.Role.DEFAULT;
                player.kingSticker.gameObject.SetActive(false);
            } else if (role != GameController.Role.GUARD)
            {
                king.roleCopy = role;
                Button roleButton = player.roleButtons[role];
                Vector3 translation = new Vector3(100f, 0f, 0f);
                if (team == GameController.Team.BLUE)
                {
                    translation *= -1;
                }
                player.kingSticker.gameObject.transform.position = roleButton.gameObject.transform.position + translation;
                player.kingSticker.gameObject.SetActive(true);
            }
            
        }
    }

    public Rigidbody getRigidBody()
    {
        return pawnRB;
    }

    public GameController.Team getTeam()
    {
        return team;
    }

    public void addPawn()
    {
        gameController.addPawn(this);
    }

    public virtual bool isBerseker() { 
        return role == GameController.Role.BERSERKER; 
    }

    public virtual bool isDoubleTap()
    {
        return role == GameController.Role.DOUBLETAP;
    }

    public void setTeam(GameController.Team team)
    {
        this.team = team;
    }
    
    public void setGameController(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void setPlayer(Player player)
    {
        this.player = player;
        this.team = player.team;
        this.gameController = player.gameController;
    }
}
