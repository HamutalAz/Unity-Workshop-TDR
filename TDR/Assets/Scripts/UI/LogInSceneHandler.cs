using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LogInSceneHandler : MonoBehaviour
{
    public TextMeshProUGUI userNameInput;
    public TextMeshProUGUI feedbackLBL;

    public async void buttonClicked()
    {
        string userName = userNameInput.text;
        try
        {
            await DataBaseManager.instance.loginManager.CreateUser(userName);
            SceneManager.LoadScene(sceneName: "WaitingRoom");
        }
        catch (Exception e)
        {
            feedbackLBL.text = e.Message;
        }
    }
}
