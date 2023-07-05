using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> panelsArr = new();
    int currInd = 0;
    [SerializeField]
    public GameObject chatHandler;
    [SerializeField]
    public TextMeshProUGUI teamsPassaedLabel;
    [SerializeField]
    public GameObject backPack;
    [SerializeField]
    public GameObject backPackPanel;
    private bool isBPVisable = true;
    private bool isBPPanelVisable = false;
    [SerializeField]
    public GameObject player;

    private Dictionary<string, Interactable> levelObjects = new();

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleBackPackVisability();
        }

    }

    public void nextClick()
    {
        Debug.Log("count:" + panelsArr.Count);
        if(currInd < panelsArr.Count-1)
        {
            Debug.Log("currInd:" + currInd);
            currInd++;
            if (currInd >= 1)
                panelsArr[currInd - 1].SetActive(false);
            panelsArr[currInd].SetActive(true);
        }
        else
        {
            panelsArr[currInd].SetActive(false);
            currInd = 0;
            Debug.Log("Done!");
        }
    }

    public void backClick()
    {
        if (currInd > 0)
        {
            currInd--;
            if (currInd + 1 < panelsArr.Count)
                panelsArr[currInd + 1].SetActive(false);
            panelsArr[currInd].SetActive(true);
        }
    }

    public void backToMainMenu()
    {
        Debug.Log("switching to login page!");

        SceneManager.LoadScene(sceneName: "LoginV2");
    }

    public void toggleBackPackVisability()
    {
        Debug.Log("toggle back pack visability!!!");

        isBPPanelVisable = !isBPPanelVisable;
        backPackPanel.SetActive(isBPPanelVisable);

        isBPVisable = !isBPVisable;
        backPack.SetActive(isBPVisable);

        togglePlayerInputSystem(isBPPanelVisable);
    }

    public bool togglePlayerInputSystem(bool val)
    {
        InputManager input = player.GetComponent<InputManager>();

        // able/disable player's movment
        if (val)
            input.OnDisable();
        else
            input.OnEnable();

        return input.isActiveAndEnabled;
    }
    public void UpdateRoomObjectUI(string name, Dictionary<string, object> data)
    {
        try
        {
            Debug.Log("at UpdateRoomObjectUI: trying to send data to: " + name);
            levelObjects[name].UpdateUI(data);
        }
        catch (Exception e)
        {
            Debug.Log("Error at UpdateRoomObjectUI while trying to send data to: " + name + "\n the error: " + e.Message);
        }
    }

    public void addLevelObject(string name, Interactable obj)
    {
        levelObjects.Add(name, obj);
        Debug.Log("at addLevelObject:: trying to add " + name + ". levelObjects size:" + levelObjects.Count);
    }

}
