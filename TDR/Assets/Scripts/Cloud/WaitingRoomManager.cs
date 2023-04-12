using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomMembersCollection;
    // Start is called before the first frame update
    private void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance;
        roomMembersCollection = dbReference.Collection("Lobby").Document("gYdtPMVaorwoc2jH3Iog").Collection("lobby_members");
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
