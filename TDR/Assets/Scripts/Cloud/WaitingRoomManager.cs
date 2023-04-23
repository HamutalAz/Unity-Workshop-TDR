using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    private DocumentReference lobbyDocument;
    private ListenerRegistration docListener;
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
            Debug.Log("MaxPlayers = " + DataBaseManager.MAXPLAYERS);
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

        DocumentReference userDoc = dbReference.Collection("Users").Document(DataBaseManager.userID);
        // creating a listener on user's DOC
        docListener = userDoc.Listen(async(docSnapshot) =>
        {
            string roomID = docSnapshot.GetValue<string>("roomId");
            Debug.Log("roomID: " + roomID);
            string gameID = docSnapshot.GetValue<string>("gameId");
            Debug.Log("gameID: " + gameID);


            if (roomID != null)
            {
                // set room & game ID to DB Manager
                DataBaseManager.roomID = roomID;
                DataBaseManager.gameID = gameID;

                DocumentReference gameDoc = dbReference.Collection("Games").Document(gameID);
                await gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    Debug.Log("on listener");
                    DocumentSnapshot res = task.Result;
                    int currentLevel = res.GetValue<int>("currentLevelInd");
                    Debug.Log("currentLevel" + currentLevel);
                    string[] levelsOrder = res.GetValue<string[]>("levelsOrder"); // todo: save later
                    Debug.Log("levelsOrder" + levelsOrder);
                    DataBaseManager.instance.waitingRoomSceneHandler.loadScene(levelsOrder[currentLevel]);
                });

            }
        });
       
    }
    public void stopDocListener()
    {
        docListener.Stop();
        Debug.Log("stopped listening!");
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
