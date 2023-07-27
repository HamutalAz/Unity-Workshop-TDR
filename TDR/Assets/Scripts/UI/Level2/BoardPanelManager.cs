using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPanelManager : MonoBehaviour
{

    private List<int> choosenLocations = new();
    // 2D board nested inside the panel
    private GameObject innerBoard;
    [SerializeField]
    public TextMeshProUGUI feedbackLabel;
    // 3D board, on the tunnel wall
    [SerializeField]
    public GameObject outerBoard;

    public GameObject okBtn;

    private Color red = new Color32(219, 55, 55, 255);
    private Color black = new Color32(0, 0, 0, 255);
    private Color white = new Color32(255, 255, 255, 100);


    // Start is called before the first frame update
    void Start()
    {
        innerBoard = transform.Find("Board").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"key", "owner" }
            };

            outerBoard.GetComponent<Board>().dropObject(data);
            resetClicked();
        }
    }

    async public void okClicked()
    {
        if (choosenLocations.Count != 5) { 
            feedbackLabel.text = "Please select exactly 5 squares";
            return;
        }

        choosenLocations.Sort();

        // update UI (for the user only - so he'll think there's a progress).
        // It'll be updated again after a response from DB will be received.
        outerBoard.GetComponent<Board>().setRooksInLocation(choosenLocations);

        // send request to server
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            {"key", "isOpen" },
            {"code", choosenLocations }
        };

        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("checkCode", "board", data);

        
        if (response)   // if OK - close panel & disable board interactivity
        {
            outerBoard.GetComponent<BoxCollider>().enabled = false;
            gameObject.SetActive(false);
            DataBaseManager.instance.levelHandler.togglePlayerInputSystem(false);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            bool res = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("escapeTheRoom", "blabla", dict);
            if (res) //if res == true, it means the game is over, and a request to delete all the game docs is required
            {
                Debug.Log("res is true, game is over!");
                //send request to delete all game and rooms docs!
            }
        }
        else            // else: show feedback label
        {
            resetClicked();
            feedbackLabel.text = "Wrong. Try again.";
        }

    }

    public void resetClicked()
    {
        feedbackLabel.text = "";

        // change back color
        foreach (int c in choosenLocations)
        {
            GameObject currCube = getCubeByLoc(c);
            restoreColor(c, currCube);
        }

        // reset list of selected squared
        choosenLocations = new();
    }

    public void cubeClicked(int cube) // !!!!! DONOT CHANGE FUNCTION SIGNATURE!!!!
    {
        Debug.Log("&&&&&&& game object " + cube + " clicked!");
        GameObject currCube = getCubeByLoc(cube);

        if (choosenLocations.Contains(cube)) // if it's selected - clear selection
        {
            restoreColor(cube, currCube);
            choosenLocations.Remove(cube);
        }
        else
        {
            choosenLocations.Add(cube);
            currCube.GetComponent<Image>().color = red;
        } 
    }

    private void restoreColor(int loc, GameObject obj)
    {
        if ((loc/10) % 2 == loc % 2)
            obj.GetComponent<Image>().color = black;
        else
            obj.GetComponent<Image>().color = white;
    }

    private GameObject getCubeByLoc(int loc)
    {
        int colNum = loc / 10;
        GameObject col = innerBoard.transform.Find("col" + colNum).gameObject;
        return col.transform.Find(loc.ToString()).gameObject;
    }
}
