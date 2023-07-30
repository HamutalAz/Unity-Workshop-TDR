using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    private List<string> avatarsList; 
    private void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance;
        roomMembersCollection = dbReference.Collection("Lobby").Document("x3XGhSKLrwiXtcgyBuIr").Collection("lobby_members");
    }
    public async Task CreateUser(string newUserName)
    {
        //create new user
        RefUser newRefUser = new RefUser(newUserName);
        CollectionReference usersCollection = dbReference.Collection("Users");
        Query query = roomMembersCollection.WhereEqualTo("userName", newUserName);
        try
        {
            await query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                //if there is even one iteration of this loop, it means there is a user with that name in the db.
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    throw new Exception("Username '" + newUserName + "' already exists. please try again.");
                }
            });

            //Add RefUser to'Lobby/x3XGhSKLrwiXtcgyBuIr/room_members' collection
            DocumentReference newLobbyDoc = roomMembersCollection.Document();
            await newLobbyDoc.SetAsync(newRefUser);

            //Selecting an avatar for user
            var random = new System.Random();
            int avatarIndex = random.Next(DataBaseManager.avatarNames.Count);
            string avatar = DataBaseManager.avatarNames[avatarIndex];

            //Add user to 'Users' collection
            User newUser = new User(newUserName,newLobbyDoc.Id,avatar );
           
            await usersCollection.Document(newUser.userId).SetAsync(newUser);
            //await roomMembersCollection.Document()

            //Here we need to consider to just save an instance of User in databasemanager.
            DataBaseManager.userName = newUserName;
            DataBaseManager.userID = newUser.userId;
            DataBaseManager.avatar = avatar;


            //await usersCollection.AddAsync(newUser);
            
            //await query.GetSnapshotAsync().ContinueWithOnMainThread((QuerySnapshotTask) =>
            //{
            //    foreach (DocumentSnapshot documentSnapshot in QuerySnapshotTask.Result.Documents)
            //    {
            //        DataBaseManager.userID = documentSnapshot.Id;
            //    }
            //});
            ////Add userID to lobby
            //await roomMembersCollection.Document(DataBaseManager.userID).SetAsync(newUser);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }

    }

}
