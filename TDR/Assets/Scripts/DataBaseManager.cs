using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.Tilemaps;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

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

    public async void CreateUser()
    {
        Debug.Log("trying to create user:" + userName.text);
        try
        {
            // check if username 'userName.text' already exists
            await dbReference.Child("Lobby").OrderByChild("userName").EqualTo(userName.text).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("FirebaseDatabaseError: IsCanceled: " + task.Exception);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("FirebaseDatabaseError: IsFaulted:" + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.ChildrenCount != 0)
                {
                    throw new Exception("Username '" + userName.text + "' already exists. please try again.");

                }
            });

            // create new user
            User newUser = new User(userName.text);
            string json = JsonUtility.ToJson(newUser);
            string key = dbReference.Child("Lobby").Push().Key;
            await dbReference.Child("Lobby").Child(key).SetRawJsonValueAsync(json);
            Debug.Log("Successfully created user '" + newUser.user_name + "'!");
            feedbackLBL.text = "Successfully created user '" + newUser.user_name + "'";

            // move to next scene!
            SceneManager.LoadScene("WaitingRoom");
            return;
        }
        catch (Exception e)
        {
            feedbackLBL.text = e.Message;
            return;
        }
    }

    // used for previous version: I didn't erase it in case you'll need it (hamutal)
    //public async void CheckIfUserNameExists(string userName)
    //{
    //    dbReference.Child("Lobby").GetValueAsync().ContinueWith(task =>
    //    {

    //        if (task.IsCompleted)
    //        {
    //            var json = task.Result.GetRawJsonValue();

    //            var users = task.Result.Value as Dictionary<string, object>;
    //            if (users != null)
    //            {
    //                //check if lobby isn't full
    //                if (users.Count > MAXPLAYERS)
    //                {
    //                    throw new Exception("Lobby is full! can't join!");
    //                }
    //                //check if user name is unique
    //                foreach (KeyValuePair<string, object> item in users.Values)
    //                {
    //                    if (item.Value.Equals(userName))
    //                    {
    //                        throw new Exception("A user with the name of '" + userName + "' already exists!");
    //                    }
    //                }
    //            }
    //        }
    //    });
    //}
}
