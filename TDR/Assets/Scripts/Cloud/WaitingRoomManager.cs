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
        roomMembersCollection = dbReference.Collection("Lobby").Document("gYdtPMVaorwoc2jH3Iog").Collection("room_members");
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
    public List<User> getListOfUsers(IEnumerable<DocumentSnapshot> users)
    {
        List<User> list = new List<User>();
        foreach (DocumentSnapshot childSnapshot in users)
        {
            User user = childSnapshot.ConvertTo<User>();
            list.Add(user);
        }
        return list;
    }
}
