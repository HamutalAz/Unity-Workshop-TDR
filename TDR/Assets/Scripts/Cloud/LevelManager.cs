using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static FirebaseFirestore dbReference;
    private DocumentReference roomDoc;          // --> for future use: maybe we'll need it for syncing other data
    private string userID;
    private string roomID;

    // other players data
    private List<string> otherUsersID = new List<string>();
    private List<DocumentReference> otherPlayersDocRef = new List<DocumentReference>();
    private Dictionary<String, Vector3> playersLoc = new Dictionary<string, Vector3>();

    private void Start()
    {
        Debug.Log("level manager start!!");

        dbReference = FirebaseFirestore.DefaultInstance;
        //userID = DataBaseManager.userID;
        //roomID = DataBaseManager.roomID;
        //getOtherPlayersData();
    }

    public async void getOtherPlayersData()
    {
        userID = DataBaseManager.userID;
        roomID = DataBaseManager.roomID;

        // Loop over all of the players in the room (which aren't the current user & add their useID to a list.
        roomDoc = dbReference.Collection("Rooms").Document(roomID);
        Debug.Log("createOtherPlayers - roomID: " + roomID);

        CollectionReference roomMembers = roomDoc.Collection("room_members");
        await roomMembers.WhereNotEqualTo("userName", DataBaseManager.userName).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                Debug.Log(String.Format("Fetched player with ID: " + documentSnapshot.Id));
                otherUsersID.Add(documentSnapshot.Id);
            }
        });
        Debug.Log("amount of other players in the room: " + otherUsersID.Count);

        // Get the players document from the DB
        foreach (string id in otherUsersID)
            otherPlayersDocRef.Add(dbReference.Collection("Users").Document(id));

        // Get the players location from DB
        await getPlayersLocation();

        // listen on other players location & update their location
        listenOnOtherPlayersDoc();
    }

    public async Task getPlayersLocation()
    {
        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            await playerDocRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            {
                DocumentSnapshot snapshot = task.Result;
                User player = snapshot.ConvertTo<User>();
                playersLoc.Add(player.userName, stringToVec(player.location));
            });
        }
        //DataBaseManager.instance.levelHandler.setPlayersLoc(playersLoc);
    }

    public void listenOnOtherPlayersDoc()
    {
        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            playerDocRef.Listen((docSnapshot) =>
            {
                string location = docSnapshot.GetValue<string>("location");
                string userNAme = docSnapshot.GetValue<string>("userName");

                playersLoc[userNAme] = stringToVec(location);
                //DataBaseManager.instance.levelHandler.setPlayersLoc(playersLoc);
            });
        }
    }
    public Dictionary<string, Vector3> getOtherPlayersLoc()
    {
        return playersLoc;
    }

    private Vector3 stringToVec(string s)
    {
        string[] temp = s.Substring(1, s.Length - 2).Split(',');
        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
    }

}
