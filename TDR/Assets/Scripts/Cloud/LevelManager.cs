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

    // Docs & Collections
    private DocumentReference roomDoc;
    private DocumentReference gameDoc;
    private CollectionReference roomObjects;
    private DocumentReference userDoc;

    private string userID;
    private string roomID;

    // other players data
    private List<string> otherUsersID = new();
    private List<DocumentReference> otherPlayersDocRef = new();
    private Dictionary<string, Vector3> playersLoc = new(); // players locations

    private void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance; 

    }
    
    public async Task<Dictionary<string, Vector3>> getOtherPlayersData()
    {
        userID = DataBaseManager.userID;
        roomID = DataBaseManager.roomID;
        userDoc = dbReference.Collection("Users").Document(userID);

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
                //Debug.Log("**LM: setPlayersLocation() playerLoc: **" + stringToVec(player.location));
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

    // convert a string represents a 3D vector into a Vector3
    private Vector3 stringToVec(string s)
    {
        string[] temp = s.Substring(1, s.Length - 2).Split(',');
        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
    }

    // create listeners on the objects in the room and set their initial value
    public void listenOnRoomObjects()
    {
        roomObjects = roomDoc.Collection("room_objects");

        roomObjects.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                DocumentReference roomObjDocRef = documentSnapshot.Reference;
                Debug.Log("inside listen loop on room obj!");

                string name = documentSnapshot.GetValue<string>("name"); 
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                //objectsData[name] = data;
                DataBaseManager.instance.levelHandler.UpdateRoomObjectUI(name, data);

                roomObjDocRef.Listen((docSnapshot) =>
                {
                    Debug.Log("something changed in game object!");
                    string name = docSnapshot.GetValue<string>("name");             // the name of the object
                    Dictionary<string, object> data = docSnapshot.ToDictionary();   // it's data
                    //objectsData[name] = data;

                    DataBaseManager.instance.levelHandler.UpdateRoomObjectUI(name, data);
                });
            }
        });

    }

    public void listenOnGameDocument()
    {
        gameDoc.Listen(snapshot =>
        {
            int qualified = snapshot.GetValue<int>("Qualified");
            int totalTeams = snapshot.GetValue<int>("numberOfRoomsToQualify");
            DataBaseManager.instance.levelHandler.UpdateTeamPassedLabel(qualified, totalTeams);
        });
    }

    public void getTeamPassedInfo()
    {
        gameDoc = dbReference.Collection("Games").Document(DataBaseManager.gameID);
        gameDoc.GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
        {
            DocumentSnapshot doc = snapshot.Result;
            int qualified = doc.GetValue<int>("Qualified");
            int totalTeams = doc.GetValue<int>("numberOfRoomsToQualify");
            DataBaseManager.instance.levelHandler.UpdateTeamPassedLabel(qualified, totalTeams);
        });
    }


    public async Task<bool> WriteToDb(string objName, Dictionary<string,object> data)
    {
        try
        {
            await roomObjects.WhereEqualTo("name", objName).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            {
                QuerySnapshot snapshot = task.Result;

                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents) // there's actually one like that
                {

                    documentSnapshot.Reference.UpdateAsync(data).ContinueWithOnMainThread(task =>
                    {
                        Debug.Log(
                                "**** Updated" + objName + " document.");
                    });
                }
            });

            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    public void updatePlayerLocation(Dictionary<string,object> data)
    {
        userDoc.UpdateAsync(data).ContinueWithOnMainThread(task =>
        {
            //Debug.Log("player's location updated to: " + loc);
        });
    }

    public async Task<Vector3> getInitialPlayerLoc()
    {
        userDoc = dbReference.Collection("Users").Document(DataBaseManager.userID);
        return await userDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            string location = task.Result.GetValue<string>("location");
            Debug.Log("111: location is:" + location);
            return stringToVec(location);
        });        
    }

}
