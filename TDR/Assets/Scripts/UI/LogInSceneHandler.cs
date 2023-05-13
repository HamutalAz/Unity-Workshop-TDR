using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LogInSceneHandler : MonoBehaviour
{
    public TextMeshProUGUI userNameInput;
    public TextMeshProUGUI feedbackLBL;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            buttonClicked();

    }


    public async void buttonClicked()
    {
        string userName = userNameInput.text;
        if (userName.Equals("Username") || userName.Equals("") || userName.Equals("username"))
        {
            feedbackLBL.text = "Empty Username is not valid.";
            return;
        }

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
