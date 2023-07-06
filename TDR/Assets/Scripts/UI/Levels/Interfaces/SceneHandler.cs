using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public abstract class SceneHandler : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI teamsPassaedLabel;
    [SerializeField]
    public GameObject backPack;
    [SerializeField]
    public GameObject backPackPanel;
    public bool isBPVisable = true;
    public bool isBPPanelVisable = false;
    [SerializeField]
    public GameObject player;
    public Dictionary<string, Interactable> levelObjects = new();

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

    public Interactable getLevelObject(string name)
    {
        return levelObjects[name];
    }
}
