using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public enum Team { BLUE, RED };
    public enum State { INITIALISATION, SELECT, LOCK, SHOOTING, SHOOTED, SECONDSHOT, SECONDSHOOTING, SECONDSHOOTED, END, BERSEKER };
    public enum Role { DEFAULT, ASSASSIN, BERSERKER, KING, DOUBLETAP, GUARD };

    public PawnController pawn;
    public GameObject linePrefab;
    public GameObject currentLine;
    public LineRenderer lineRenderer;

    public Player bluePlayer;
    public Player redPlayer;
    public Button blueWinButton;
    public Button redWinButton;

    public Player currentPlayer;
    public State state;
    public float force;
    public float maxDistLine;

    public List<GameObject> obstacles;

    public AudioSource pawnCollisionAudio;
    public AudioSource pawnShootAudio;

    private bool isShooted;

    // Start is called before the first frame update
    void Start()
    {
        // Unactivate Roles Buttons
        bluePlayer.desactivateRoleButtons();
        redPlayer.desactivateRoleButtons();

        currentPlayer = bluePlayer;
        bluePlayer.setGameController(this);
        redPlayer.setGameController(this);
        isShooted = false;
        state = State.INITIALISATION;
        
    }

    private void OnDestroy()
    {
        Destroy(bluePlayer);
        Destroy(redPlayer);
    }

    private IEnumerator shooted()
    {
        yield return new WaitForSeconds(0.05f);
        isShooted = true;
    }

    // Update is called once per frame
    void Update()
    {

        // Quit Game
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Any => END
        if (bluePlayer.defeat())
        {
            state = State.END;
            redWinButton.gameObject.SetActive(true);
        } else if (redPlayer.defeat())
        {
            state = State.END;
            blueWinButton.gameObject.SetActive(true);
        }

        // INITIALISATION => LOCK
        if (state == State.INITIALISATION && bluePlayer.isReady() && redPlayer.isReady())
        {
            // Desactivate Ready Buttons;
            bluePlayer._readyButton.gameObject.SetActive(false);
            redPlayer._readyButton.gameObject.SetActive(false);
            // Activate Role Button
            bluePlayer.activateRoleButtons();
            redPlayer.activateRoleButtons();
            // Make Pawn/Obstacles none kinematic
            bluePlayer.unKinematic();
            redPlayer.unKinematic();
            foreach (GameObject obstacle in obstacles)
            {
                obstacle.GetComponent<Rigidbody>().isKinematic = false;
            }
            // Select First Pawn
            int alea = Random.Range(0, 2);
            if (alea == 0)
            {
                currentPlayer = bluePlayer;
                selectPawn(bluePlayer.pawns[0]);
            } else
            {
                currentPlayer = redPlayer;
                selectPawn(redPlayer.pawns[0]);
            }
            state = State.LOCK;
        }

        // BERSEKER => SHOOTED
        if (state == State.BERSEKER && currentPlayer.isReady())
        {
            currentPlayer._readyButton.gameObject.SetActive(false);
            currentPlayer.activateRoleButtons();
            bluePlayer.unKinematic();
            redPlayer.unKinematic();
            foreach (GameObject obstacle in obstacles)
            {
                obstacle.GetComponent<Rigidbody>().isKinematic = false;
            }
            state = State.SHOOTED;
        }

        // SHOOTED / SECONDSHOOTED => LOCK / SECONDSHOT
        if ((state == State.SHOOTED || state == State.SECONDSHOOTED) && allPawnsAreStopped() && isShooted)
        {
            isShooted = false;
            if (state == State.SECONDSHOOTED)
            {
                pawn.selectionRing.SetActive(true);
                state = State.SECONDSHOT;
            }
            else
            {
                pawn.locking();
                switchPlayer();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (state)
            {
                // LOCK = > LOCK
                case State.LOCK:
                    nextPawn();
                    break;
                // SHOOTING = > LOCK
                case State.SHOOTING:
                    Destroy(currentLine);
                    pawn.shootRing.SetActive(false);
                    locking();
                    nextPawn();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetMouseButton(0))
        {
            // SELECT / SECONDSHOT = > SHOOTING
            if (state == State.SELECT || state == State.SECONDSHOT)
            {
                pawn.shootRing.SetActive(true);
                createLine();
                drawLine();
                if (state == State.SELECT)
                {
                    state = State.SHOOTING;
                }
                else if (state == State.SECONDSHOT)
                {
                    state = State.SECONDSHOOTING;
                }

            }
            // SHOOTING => SHOOTING
            else if (state == State.SHOOTING || state == State.SECONDSHOOTING)
            {
                drawLine();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // SHOOTING => SHOOTED
            if (state == State.SHOOTING || state == State.SECONDSHOOTING)
            {
                shoot();            
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // SELECT = > LOCK
            if (state == State.SELECT)
            {
                state = State.LOCK;
                pawn.locking();
            }
            // LOCK = > SELECT
            else if (state == State.LOCK)
            {
                pawn.unlocking();
                state = State.SELECT;
            // SHOOTING => LOCK
            } else if (state == State.SHOOTING)
            {
                Destroy(currentLine);
                pawn.shootRing.SetActive(false);
                locking();
            }
        }
    }

    private void locking() {
        state = State.LOCK;
        pawn.locking();
    }

    public bool allPawnsAreStopped()
    {
        return bluePlayer.allPawnsAreStopped() && redPlayer.allPawnsAreStopped();
    }

    public PawnController getPawn()
    {
        return pawn;
    }

    public void setPawn(PawnController newPawn)
    {
        pawn = newPawn;
    }

    public void selectPawn(PawnController pawn)
    {
        if (this.pawn && this.pawn.role == Role.KING)
        {
            ((KingPawn)this.pawn).roleCopy = Role.DEFAULT;
            currentPlayer.kingSticker.gameObject.SetActive(false);
        }
        this.pawn = pawn;
        currentPlayer.setCurrentPawnInd(pawn.role);
        pawn.selectionRing.SetActive(true);
        locking();
    }

    public void switchPawn(PawnController pawn)
    {
        this.pawn.selectionRing.SetActive(false);
        currentPlayer.setCurrentPawnInd(pawn.role);
        selectPawn(pawn);
        
    }

    public void nextPawn()
    {
        currentPlayer.nextPawn();
    }

    void createLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, pawn.transform.position);
        lineRenderer.SetPosition(1, pawn.transform.position);
    }

    void drawLine()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector3 tempFingerPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.y - pawn.transform.position.y));
        if (Vector3.Distance(pawn.transform.position, tempFingerPos) > maxDistLine)
        {
            lineRenderer.SetPosition(1, pawn.transform.position + (tempFingerPos - pawn.transform.position).normalized * maxDistLine);
        }
        else
        {
            lineRenderer.SetPosition(1, tempFingerPos);
        }
    }

    public void shoot()
    {
        pawnShootAudio.Play(0);
        Vector3 line = lineRenderer.GetPosition(0) - lineRenderer.GetPosition(1);
        pawn.getRigidBody().AddForce(force * line, ForceMode.Impulse);
        pawn.selectionRing.SetActive(false);
        pawn.shootRing.SetActive(false);
        Destroy(currentLine);
        if (pawn.isDoubleTap()) {
            if (state == State.SECONDSHOOTING)
            {
                state = State.SHOOTED;
            } else
            {
                state = State.SECONDSHOOTED;
            }
        } else
        {
            state = State.SHOOTED;
        }
        StartCoroutine(shooted());
    }

    public void addPawn(PawnController pawn2Add)
    {
        player(pawn2Add.team).pawns.Add(pawn2Add);
    }


    public Player player(Team team)
    {
        switch (team)
        {

            case Team.BLUE:
                return bluePlayer;
            case Team.RED:
                return redPlayer;
            default:
                throw new System.Exception("Player not Found for team : " + team);
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void switchPlayer()
    {
        currentPlayer.kingSticker.gameObject.SetActive(false);
        ((KingPawn) currentPlayer.getPawn(GameController.Role.KING)).roleCopy = GameController.Role.DEFAULT;
        currentPlayer = currentPlayer.team == Team.BLUE ? redPlayer : bluePlayer;
        currentPlayer.currentPawnInd = 0;
        selectPawn(currentPlayer.pawns[0]);
    }

}
