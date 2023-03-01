using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using System;

public class DataBaseManager : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI feedbackLBL;
    private string userID;
    private DatabaseReference dbReference;
    private const int MAXPLAYERS = 5;

    // Start is called before the first frame update
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    public void CreateUser()
    {
        Debug.Log("trying to create user:" + userName.text);
        try
        {
            CheckIfUserNameExists(userName.text);
            User newUser = new User(userName.text);
            string json = JsonUtility.ToJson(newUser);
            string key = dbReference.Child("Lobby").Push().Key;
            dbReference.Child("Lobby").Child(key).SetRawJsonValueAsync(json);
            Debug.Log("Successfully creatued user '" + newUser.user_name + "'!");
            //move to next scene!
        } catch (Exception e)
        {
            feedbackLBL.text = e.Message;
        }
        
        
        
       
        
        //dbReference.Child("Lobby").SetRawJsonValueAsync(json);



        //idea: add a list of users. before adding, get the list from datasnapshot, and check if users exists there. If not, add him and add the list back to
        // /Lobby!
          
        //todo: later check if such user exists, if so change feedback label to a better feedback.
        //feedbackLBL.text = "trying to create user with username: " + userName.text;
    }

    public void CheckIfUserNameExists(string userName)
    {
        dbReference.Child("Lobby").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsCompleted)
            {
                var json = task.Result.GetRawJsonValue();

                var users = task.Result.Value as Dictionary<string, object>;
                if (users!= null)
                {
                    //check if lobby isn't full
                    if (users.Count > MAXPLAYERS)
                    {
                        throw new Exception("Lobby is full! can't join!");
                    }
                    //check if user name is unique
                    foreach (KeyValuePair<string, object> item in users.Values)
                    {
                        if (item.Value.Equals(userName)){
                            throw new Exception("A user with the name of '" + userName + "' already exists!");
                        }
                    }
                } 
            }
        });
    }
}
