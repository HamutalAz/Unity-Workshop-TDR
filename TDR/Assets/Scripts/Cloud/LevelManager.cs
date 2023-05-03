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
    private Dictionary<String, object> objectsData = new Dictionary<string, object>();

    private void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance;
    }

    public async Task<Dictionary<string, Vector3>> getOtherPlayersData()
    {
        userID = DataBaseManager.userID;
        roomID = DataBaseManager.roomID;
        //Debug.Log("**** LM: getOtherPlayersData: roomID: ****" + roomID);

        // Loop over all of the players in the room (which aren't the current user & add their useID to a list.
        roomDoc = dbReference.Collection("Rooms").Document(roomID);

        CollectionReference roomMembers = roomDoc.Collection("room_members");
        await roomMembers.WhereNotEqualTo("userName", DataBaseManager.userName).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                //Debug.Log(String.Format("LM: getOtherPlayersData: Fetched player with ID: " + documentSnapshot.Id));
                otherUsersID.Add(documentSnapshot.Id);
            }
        });
        //Debug.Log("**** LM: getOtherPlayersData: amount of other players in the room: ****" + otherUsersID.Count);

        // Get the players document from the DB
        foreach (string id in otherUsersID)
            otherPlayersDocRef.Add(dbReference.Collection("Users").Document(id));

        // Get the players location from DB
        return await setPlayersLocation();
    }

    public async Task<Dictionary<string,Vector3>> setPlayersLocation()
    {
        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            await playerDocRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            {
                DocumentSnapshot snapshot = task.Result;
                User player = snapshot.ConvertTo<User>();
                playersLoc.Add(player.userName, stringToVec(player.location));
                Debug.Log("**LM: setPlayersLocation() playerLoc: **" + stringToVec(player.location));
            });
        }

        //Debug.Log("**** LM: setPlayersLocation(): playersLoc.Count **** " + playersLoc.Count);
        return getOtherPlayersLoc();
    }

    public void listenOnOtherPlayersDoc()
    {
        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            playerDocRef.Listen((docSnapshot) =>
            {
                string location = docSnapshot.GetValue<string>("location");
                string userNAme = docSnapshot.GetValue<string>("userName");
                if (location != null)
                    playersLoc[userNAme] = stringToVec(location);
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

    public void listenOnRoomObjects()
    {
        CollectionReference roomObjects = roomDoc.Collection("room_objects");

        roomObjects.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                DocumentReference roomObjDocRef = documentSnapshot.Reference;

                roomObjDocRef.Listen((docSnapshot) =>
                {
                    // todo: make it generic ...
                    string name = docSnapshot.GetValue<string>("name"); // the name of the object
                    bool isOpen = docSnapshot.GetValue<bool>("isOpen"); // the change - in our case it's a door.
                    objectsData[name] = isOpen;

                });



            }
        });

    }

}
