using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using TMPro;
using UnityEngine;

public class KeyPad : Interactable
{
    [SerializeField]
    private GameObject door;
    private bool isOpen = false;
    [SerializeField]
    GameObject digitLock;
    string code = "";
    bool isPanelOpen = false;
    [SerializeField]
    public TMP_Text feedbackLabel;
    [SerializeField]
    private tutorialHandler tHandler;

    private void Update()
    {
        if (isPanelOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
                digitPressed(0);
            else if (Input.GetKeyDown(KeyCode.Alpha1))
                digitPressed(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                digitPressed(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                digitPressed(3);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                digitPressed(4);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                digitPressed(5);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                digitPressed(6);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                digitPressed(7);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                digitPressed(8);
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                digitPressed(9);
            else if (Input.GetKeyDown(KeyCode.Return))
                okClicked();
        }
    }
    protected override void Interact()
    {
        isPanelOpen = !isPanelOpen;
        digitLock.SetActive(isPanelOpen);
        resetClicked();
    }

    public void okClicked()
    {
        if (code == "1234")
        {
            isOpen = !isOpen;
            door.GetComponent<Animator>().SetBool("IsOpen", isOpen);
            isPanelOpen = false;
            digitLock.SetActive(isPanelOpen);
            tHandler.showNextGuidePanel(8);
        }
        else
        {
            updateFeedBackLabel("Wrong code. Try again.");
            code = "";
        }
    }

    public void digitPressed(int num)
    {
        code += num.ToString();
        updateFeedBackLabel(code);
    }

    public void resetClicked()
    {
        code = "";
        updateFeedBackLabel(code);
    }

    private void updateFeedBackLabel(string text)
    {
        feedbackLabel.text = text;
    }
}
