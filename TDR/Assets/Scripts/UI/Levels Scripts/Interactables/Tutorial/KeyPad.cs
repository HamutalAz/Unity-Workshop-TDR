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
    // Start is called before the first frame update
    void Start()
    {
        //DataBaseManager.instance.levelHandler.addLevelObject("door", this);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    protected override void Interact()
    {

        isPanelOpen = !isPanelOpen;
        digitLock.SetActive(isPanelOpen);

    }

    public void okClicked()
    {
        if (code == "1234")
        {
            isOpen = !isOpen;
            door.GetComponent<Animator>().SetBool("IsOpen", isOpen);
            isPanelOpen = false;
            digitLock.SetActive(isPanelOpen);
        }
        else
            updateFeedBackLabel("Wrong code. Try again.");
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
