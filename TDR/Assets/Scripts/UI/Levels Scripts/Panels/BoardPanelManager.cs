using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPanelManager : MonoBehaviour
{

    private List<int> choosenLocations = new();
    private GameObject innerBoard;

    [SerializeField]
    public TextMeshProUGUI feedbackLabel;

    [SerializeField]
    public GameObject outerBoard;

    private Color red = new Color32(219, 55, 55, 255);
    private Color black = new Color32(0, 0, 0, 255);
    private Color white = new Color32(255, 255, 255, 255);

    // Start is called before the first frame update
    void Start()
    {
        innerBoard = transform.Find("Board").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void okClicked()
    {
        choosenLocations.Sort();

        bool result = false;

        // send request to server
      
        // update UI
        outerBoard.GetComponent<Board>().setRooksInLocation(choosenLocations);

        // if OK - close panel; else: show feedback label
        if (result) {
            gameObject.SetActive(false);
            DataBaseManager.instance.levelHandler.togglePlayerInputSystem(false);
        }
        else
            feedbackLabel.text = "Wrong locations. Try again.";

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
