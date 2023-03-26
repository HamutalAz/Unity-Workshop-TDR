using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogInHanseler : MonoBehaviour
{
    public TextMeshProUGUI userNameInput;
    public TextMeshProUGUI feedbackLBL;

    public void buttonClicked()
    {
        string userName = userNameInput.text;
        try
        {
            DataBaseManager.instance.CreateUser(userName);
            SceneManager.LoadScene(sceneName: "WaitingRoom");

        }
        catch (Exception e)
        {
            feedbackLBL.text = e.Message;
        }
    }
}
