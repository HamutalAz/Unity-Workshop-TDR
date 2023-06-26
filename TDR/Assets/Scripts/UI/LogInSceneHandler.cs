using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInSceneHandler : MonoBehaviour
{
    public TextMeshProUGUI userNameInput;
    public TextMeshProUGUI feedbackLBL;
    [SerializeField]
    public Button logInButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            loginButtonClicked();
    }


    public async void loginButtonClicked()
    {
        logInButton.enabled = false;
        string userName = userNameInput.text;
        if (userName.Equals("Username") || userName.Equals("") || userName.Equals("username"))
        {
            feedbackLBL.text = "Empty Username is not valid.";
            logInButton.enabled = true;
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
            logInButton.enabled = true;
        }
    }

    public void tutorialButtonClicked()
    {
        Debug.Log("switching to tutorial!");

        SceneManager.LoadScene(sceneName: "level1");
    }
}
