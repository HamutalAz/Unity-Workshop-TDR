using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialHandler : SceneHandler
{
    [SerializeField]
    private List<GameObject> panelsArr = new();
    int currInd = 0;

    void Start() {
        player.GetComponent<PlayerMotor>().setIsTutorial(true);
    }

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
        if(currInd < panelsArr.Count-1)
        {
            currInd++;
            if (currInd >= 1)
                panelsArr[currInd - 1].SetActive(false);
            panelsArr[currInd].SetActive(true);
        }
        else
        {
            panelsArr[currInd].SetActive(false);
            currInd = 0;
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

    public void okClicked()
    {
        if (currInd <= panelsArr.Count - 1)
        {
            if (currInd >= 1)
                panelsArr[currInd].SetActive(false);
        }
    }

    public void showNextGuidePanel(int ind)
    {
        while(currInd < ind)
        {
            panelsArr[currInd - 1].SetActive(false);
            currInd++;
        }
        panelsArr[currInd - 1].SetActive(false);

        panelsArr[currInd].SetActive(true);
    }

    public void backToMainMenu()
    {
        Debug.Log("switching to login page!");

        SceneManager.LoadScene(sceneName: "LoginV2");
    }

}
