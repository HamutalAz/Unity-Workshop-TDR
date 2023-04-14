using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    private DocumentReference lobbyDocument;
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
