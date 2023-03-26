using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.Tilemaps;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Firestore;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

<<<<<<< Updated upstream:TDR/Assets/Scripts/DataBaseManager.cs
    public TextMeshProUGUI userName;
    public TextMeshProUGUI feedbackLBL;
=======
    private string userName;
>>>>>>> Stashed changes:TDR/Assets/Scripts/Cloud/DataBaseManager.cs
    private string userID;
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    private GridManager gridManager;
    public static int MAXPLAYERS = 20;
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream:TDR/Assets/Scripts/DataBaseManager.cs
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
=======
        //userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseFirestore.DefaultInstance;
        roomMembersCollection = dbReference.Collection("Lobby").Document("gYdtPMVaorwoc2jH3Iog").Collection("room_members");
          
    }

    //getters
    public String getUserName()
    {
        return userName;
    }

    public async Task CreateUser(string newUserName) 
    {
        //create new user
        User newUser = new User(newUserName);
        CollectionReference usersCollection = dbReference.Collection("Users");
        Query query = usersCollection.WhereEqualTo("userName", newUserName);
        try
        {
            await query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                //if there is even one iteration of this loop, it means there is a user with that name in the db.
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    throw new Exception();
                    //Debug.Log(String.Format(" this is user number {0} ", documentSnapshot.Id));
                }
            });
            userName = newUserName;
            await usersCollection.AddAsync(newUser);
            await query.GetSnapshotAsync().ContinueWithOnMainThread((QuerySnapshotTask) =>
            {
                foreach (DocumentSnapshot documentSnapshot in QuerySnapshotTask.Result.Documents)
                {
                    userID = documentSnapshot.Id;
                    Debug.Log(userID);
                }
            });
            //Add userID to lobby
            //CollectionReference lobbyCollection = dbReference.Collection("Lobby");
            //DocumentReference lobbyDocument = lobbyCollection.Document("gYdtPMVaorwoc2jH3Iog");
            //lobbyDocument = lobbyDocument.Collection("room_members").Document(userID);
            await roomMembersCollection.Document(userID).SetAsync(newUser);
            





        } catch (Exception e)
        {
            Debug.Log(e.Message);
            throw new Exception("Username '" + newUserName + "' already exists. please try again.");
        }
        
        



        //try
        //{
        //    await dbReference.Child("Lobby").OrderByChild("userName").EqualTo(newUserName).GetValueAsync().ContinueWithOnMainThread(task =>
        //    {
        //        Debug.Log("inside task");
        //        if (task.IsCanceled)
        //        {
        //            Debug.Log("FirebaseDatabaseError: IsCanceled: " + task.Exception);
        //            return;
        //        }
        //        if (task.IsFaulted)
        //        {
        //            Debug.Log("FirebaseDatabaseError: IsFaulted:" + task.Exception);
        //            return;
        //        }

        //        DataSnapshot snapshot = task.Result;
        //        if (snapshot.ChildrenCount != 0)
        //        {
        //            Debug.Log("name is duplicated!");
        //            throw new Exception("Username '" + newUserName + "' already exists. please try again.");

        //        }
        //    });
        //    create new user
        //    User newUser = new User(newUserName);
        //    string json = JsonUtility.ToJson(newUser);
        //    string key = dbReference.Child("Lobby").Push().Key;
        //    await dbReference.Child("Lobby").Child(key).SetRawJsonValueAsync(json);
        //    Debug.Log("Successfully created user '" + newUserName + "'!");
        //    userName = newUserName;
        //    return;
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("we got here");
        //    throw new Exception("Username '" + userName + "' already exists. please try again.");
        //}
        //check if username 'userName.text' already exists

>>>>>>> Stashed changes:TDR/Assets/Scripts/Cloud/DataBaseManager.cs
    }
    public void assignGridManager(GridManager item)
    {
        gridManager = item;
    }



    public async void updateUsers()
    {
        await roomMembersCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            gridManager.updateNodes(userName, getListOfUsers(task.Result.Documents));
        });

        roomMembersCollection.Listen((collection) =>
        {
            gridManager.updateNodes(userName, getListOfUsers(collection.Documents));
        });
        //await dbReference.Child("Lobby").GetValueAsync().ContinueWithOnMainThread(task =>
        //{
        //    if (task.IsCanceled)
        //    {
        //        Debug.Log("FirebaseDatabaseError: IsCanceled: " + task.Exception);
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.Log("FirebaseDatabaseError: IsFaulted:" + task.Exception);
        //        return;
        //    }
        //    gridManager.updateNodes(userName, getListOfUsers(task.Result));
        //});
        //dbReference.Child("Lobby").ValueChanged += HandleValueChanged;
    }

    //private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }
    //    // gridManager.updateNodes(userName, getListOfUsers(args.Snapshot));
    //    // Do something with the data in args.Snapshot
    //}

    public List<User> getListOfUsers(IEnumerable<DocumentSnapshot> users)
    {
        List<User> list = new List<User>();
        foreach (DocumentSnapshot childSnapshot in users)
        {
            User user = childSnapshot.ConvertTo<User>();
            //User user = JsonUtility.FromJson<User>(childSnapshot.GetRawJsonValue());
            list.Add(user);
        }
        return list;
    }


}
