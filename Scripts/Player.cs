using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour, ISerializationCallbackReceiver
{

    public List<PawnController> pawns;

    public List<GameController.Role> _keys = new List<GameController.Role> { GameController.Role.ASSASSIN, GameController.Role.GUARD, GameController.Role.KING, GameController.Role.BERSERKER, GameController.Role.DOUBLETAP};
    public List<Button> _values;
    public Dictionary<GameController.Role, Button> roleButtons;

    public List<GameController.Role> destroyedPawnsRole;
    public GameController.Team team;
    public int currentPawnInd;
    public GameController gameController;
    public GameObject zone;

    public Button _readyButton;
    private bool ready;

    public Image kingSticker;

    // Start is called before the first frame update
    void Start()
    {
        ready = false;
        currentPawnInd = 0;
        foreach (PawnController pawn in pawns)
        {
            pawn.setPlayer(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void nextPawn()
    {
        // unselect current pawn
        pawns[currentPawnInd].selectionRing.SetActive(false);
        // increment Pawn
        currentPawnInd = (currentPawnInd + 1) % pawns.Count;
        // select next pawn
        gameController.selectPawn(pawns[currentPawnInd]);
    }

    public void setCurrentPawnInd(GameController.Role newRole)
    {
        currentPawnInd = 0;
        foreach (PawnController pc in pawns) {
            if (pc.role == newRole)
            {
                break;
            }
            ++currentPawnInd;
        }
    }

    private void OnDestroy()
    {
        foreach (Button button in roleButtons.Values)
        {
            Color newMaterialColor = button.image.material.color;
            newMaterialColor.a = 0.5f;
            button.image.material.color = newMaterialColor;
        }
    }

    public void destroyPawn(PawnController pawn2Destroy)
    {
        for (int i = 0; i < pawns.Count; ++i)
        {
            if (pawns[i] == pawn2Destroy)
            {
                currentPawnInd = 0;
                destroyedPawnsRole.Add(pawns[i].role);
                pawns.RemoveAt(i);
                break;
            }
        }
    }

    public void unDestroyPawn(PawnController pawn2UnDestroy)
    {
        pawns.Add(pawn2UnDestroy);
        destroyedPawnsRole.Remove(pawn2UnDestroy.role);
    }

    public bool allPawnsAreStopped()
    {
        bool res = true;
        foreach (PawnController pawn in pawns) {
            if (Vector3.Distance(pawn.getRigidBody().velocity, Vector3.zero) > 0.05f)
            {
                res = false;
                break;
            }
        }
        return res;
    }

    public void readyButton()
    {
        Color newColor = _readyButton.image.color;
        newColor.a = 1.5f - newColor.a;
        _readyButton.image.color = newColor;
        ready = !ready;
    }

    public bool isReady()
    {
        return ready;
    }

    public void setGameController(GameController gameController)
    {
        this.gameController = gameController;
    }

    public PawnController getPawn() {
        return pawns[currentPawnInd];
    }

    public PawnController getPawn(GameController.Role searchedRole)
    {
        foreach (PawnController pc in pawns)
        {
            if (pc.role == searchedRole)
            {
                return pc;
            }
        }
        Debug.LogError(searchedRole + " not found in " + team + " team");
        return null;
    }

    public void activateRoleButtons()
    {
        foreach(Button button in roleButtons.Values) {
            button.gameObject.SetActive(true);
        }
    }
    public void desactivateRoleButtons()
    {
        foreach (Button button in roleButtons.Values)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void unKinematic()
    {
        foreach (PawnController pc in pawns)
        {
            pc.getRigidBody().isKinematic = false;
        }
    }

    public void kinematic()
    {
        foreach (PawnController pc in pawns)
        {
            pc.getRigidBody().isKinematic = true;
        }
    }

    public bool defeat()
    {
        return destroyedPawnsRole.Count >= 4 || destroyedPawnsRole.Contains(GameController.Role.KING);
    }


    // ********************************************************************
    // *********** ISerializationCallbackReceiver Implementation **********
    // ********************************************************************
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        roleButtons = new Dictionary<GameController.Role, Button>();

        for (int i = 0; i != System.Math.Min(_keys.Count, _values.Count); i++)
            roleButtons.Add(_keys[i], _values[i]);
    }

    void OnGUI()
    {
        /*foreach (var kvp in roleButtons)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);*/
    }
}
