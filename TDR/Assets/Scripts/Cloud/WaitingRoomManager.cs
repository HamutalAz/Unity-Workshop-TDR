using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    private DocumentReference lobbyDocument;
    private ListenerRegistration userListener;
    private ListenerRegistration gameListener;
    private string gameStatusKey;
    private bool isFirst = true;
    private int gameListenerCounter = 0;
    private int userListenerCounter = 0;

    // Start is called before the first frame update
    private void Start()
    {

        dbReference = FirebaseFirestore.DefaultInstance;
        lobbyDocument = dbReference.Collection("Lobby").Document("gYdtPMVaorwoc2jH3Iog");
        roomMembersCollection =lobbyDocument.Collection("lobby_members");
        updateMaxPlayers();
    }

    public async void updateMaxPlayers()
    {
        await lobbyDocument.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            DocumentSnapshot res = task.Result;
            DataBaseManager.MAXPLAYERS = res.GetValue<int>("MaxPlayers");
           // Debug.Log("MaxPlayers = " + DataBaseManager.MAXPLAYERS);
        });
    }

    public async void updateUsers()
    {
        await roomMembersCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DataBaseManager.instance.waitingRoomSceneHandler.updateNodes(DataBaseManager.userName, getListOfUsers(task.Result.Documents));
        });
        roomMembersCollection.Listen((collection) =>
        {
            DataBaseManager.instance.waitingRoomSceneHandler.updateNodes(DataBaseManager.userName, getListOfUsers(collection.Documents));
        });

        startListenToUserChanges();

        //DocumentReference userDoc = dbReference.Collection("Users").Document(DataBaseManager.userID);
        //// creating a listener on user's DOC
        //docListener = userDoc.Listen(async(docSnapshot) =>
        //{
        //    string roomID = docSnapshot.GetValue<string>("roomId");
        //    Debug.Log("roomID: " + roomID);
        //    string gameID = docSnapshot.GetValue<string>("gameId");
        //    Debug.Log("gameID: " + gameID);


        //    if (roomID != null)
        //    {
        //        // set room & game ID to DB Manager
        //        DataBaseManager.roomID = roomID;
        //        DataBaseManager.gameID = gameID;

        //        DocumentReference gameDoc = dbReference.Collection("Games").Document(gameID);
        //        await gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //        {
        //            Debug.Log("on listener");
        //            DocumentSnapshot res = task.Result;
        //            int currentLevel = res.GetValue<int>("currentLevelInd");
        //            Debug.Log("currentLevel" + currentLevel);
        //            string[] levelsOrder = res.GetValue<string[]>("levelsOrder"); // todo: save later
        //            Debug.Log("levelsOrder" + levelsOrder);
        //            DataBaseManager.instance.waitingRoomSceneHandler.loadScene(levelsOrder[currentLevel]);
        //        });

        //    }
        //});
       
    }

    public void startListenToUserChanges()
    {

        DocumentReference userDoc = dbReference.Collection("Users").Document(DataBaseManager.userID);
        // creating a listener on user's DOC
        userListener = userDoc.Listen( (docSnapshot) =>
        {
            userListenerCounter++;
            Debug.Log("user listener: " + userListenerCounter);
            string roomID = docSnapshot.GetValue<string>("roomId");
            Debug.Log("roomID: " + roomID);
            string gameID = docSnapshot.GetValue<string>("gameId");
            Debug.Log("gameID: " + gameID);
            DataBaseManager.roomID = roomID;
            DataBaseManager.gameID = gameID;

            if(gameID != null && isFirst == true)
            {
                isFirst = false;
                //start listen to Games/{gameID}/readyToLoad = true
                 startListenToGameChanges(gameID);
            }

            //if (roomID != null)
            //{
            //    // set room & game ID to DB Manager
            //    DataBaseManager.roomID = roomID;
            //    DataBaseManager.gameID = gameID;

            //    DocumentReference gameDoc = dbReference.Collection("Games").Document(gameID);
            //    await gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            //    {
            //        Debug.Log("on listener");
            //        DocumentSnapshot res = task.Result;
            //        int currentLevel = res.GetValue<int>("currentLevelInd");
            //        Debug.Log("currentLevel" + currentLevel);
            //        string[] levelsOrder = res.GetValue<string[]>("levelsOrder"); // todo: save later
            //        Debug.Log("levelsOrder" + levelsOrder);
            //        DataBaseManager.instance.waitingRoomSceneHandler.loadScene(levelsOrder[currentLevel]);
            //    });

            //}
        });
    }

    public void startListenToGameChanges(string gameID)
    {
        DocumentReference gameStatus = dbReference.Collection("Games").Document(gameID);
        gameListener = gameStatus.Collection("game_status").Listen(async (snapshot) =>
        {
            gameListenerCounter++;
            Debug.Log("game listener: " + gameListenerCounter);
            Debug.Log("number of docs in games/{gameID}/games_status : " + snapshot.Count);
            foreach (DocumentSnapshot childSnapshot in snapshot.Documents)
            {
                //Debug.Log("number of docs:" + snapshot.Count);
                if (childSnapshot.GetValue<bool>("readyToLoad"))
                {
                    Debug.Log("game is ready to load!");
                    DocumentReference gameDoc = dbReference.Collection("Games").Document(gameID);
                    await gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        DocumentSnapshot res = task.Result;
                        int currentLevel = res.GetValue<int>("currentLevelInd");
                        //Debug.Log("currentLevel" + currentLevel);
                        string[] levelsOrder = res.GetValue<string[]>("levelsOrder"); // todo: save later
                        stopListenToUserChanges();
                        stopListenToGameChanges();
                        //Debug.Log("levelsOrder" + levelsOrder);
                        DataBaseManager.instance.waitingRoomSceneHandler.loadScene(levelsOrder[currentLevel]);
                    });
                }
                else
                {
                    Debug.Log("game isn't ready to load!");
                }
            }
        });
        //gameListener = gameDoc.Listen(async(snapshot) =>
        //{
        //    Debug.Log("game listener is called!");
        //    var x = snapshot.GetValue<bool>("readyToLoad");
        //    if (x == true)
        //    {
        //        Debug.Log("game is ready to load!");
        //        DocumentReference gameDoc = dbReference.Collection("Games").Document(gameID);
        //        await gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //        {
        //            DocumentSnapshot res = task.Result;
        //            int currentLevel = res.GetValue<int>("currentLevelInd");
        //            Debug.Log("currentLevel" + currentLevel);
        //            string[] levelsOrder = res.GetValue<string[]>("levelsOrder"); // todo: save later
        //            Debug.Log("levelsOrder" + levelsOrder);
        //            DataBaseManager.instance.waitingRoomSceneHandler.loadScene(levelsOrder[currentLevel]);

        //        });
        //    }
            

        //});
    }
    public void stopListenToUserChanges()
    {
        userListener.Stop();
        Debug.Log("stopped listening to user!");
    }

    public void stopListenToGameChanges()
    {
        gameListener.Stop();
        Debug.Log("stopped listening to game!");
    }

    public List<RefUser> getListOfUsers(IEnumerable<DocumentSnapshot> users)
    {
        List<RefUser> list = new List<RefUser>();
        foreach (DocumentSnapshot childSnapshot in users)
        {
            RefUser user = childSnapshot.ConvertTo<RefUser>();
            list.Add(user);
        }
        return list;
    }
}
