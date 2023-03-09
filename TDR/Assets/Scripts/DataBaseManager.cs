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
    public static DataBaseManager instance;

    public TextMeshProUGUI userName;
    public TextMeshProUGUI feedbackLBL;
    private string userID;
    private DatabaseReference dbReference;
    private GridManager gridManager;
    private static int MAXPLAYERS = 20;
    // Start is called before the first frame update
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        instance = this;
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
            userID = newUser.userName;
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
    public void assignGridManager(GridManager item)
    {
        gridManager = item;
    }

    public async void updateUsers()
    {
        await dbReference.Child("Lobby").GetValueAsync().ContinueWithOnMainThread(task =>
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
            gridManager.updateNodes(userID, getListOfUsers(task.Result));
            });
        dbReference.Child("Lobby").ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
         gridManager.updateNodes(userID, getListOfUsers(args.Snapshot));
        // Do something with the data in args.Snapshot
    }

    public List<User> getListOfUsers(DataSnapshot snapshot)
    {
    List<User> list = new List<User>();
    foreach (var childSnapshot in snapshot.Children)
    {
        User user = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());
        list.Add(user);
    }
    return list;
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
