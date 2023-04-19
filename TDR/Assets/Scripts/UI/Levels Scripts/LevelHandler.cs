using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;

public class LevelHandler : MonoBehaviour
{
    private static FirebaseFirestore dbReference;
    private DocumentReference roomDoc;          // --> for future use: maybe we'll need it for syncing other data
    private string userID;
    private string roomID;

    // other players data
    private List<string> otherUsersID = new List<string>();
    private List<DocumentReference> otherPlayersDocRef = new List<DocumentReference>();
    private List<GameObject> otherPlayersAvatars = new List<GameObject>();

    // Start is called before the first frame update
    async void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance;
        userID = DataBaseManager.userID; 

        // Get user's room ID
        DocumentReference userDoc = dbReference.Collection("Users").Document(userID);
        await userDoc.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            DocumentSnapshot snapshot = task.Result;
            User user = snapshot.ConvertTo<User>();
            roomID = user.roomId;
        });

        await createOtherPlayers();
    }

    private async Task createOtherPlayers()
    { 
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
        Debug.Log("otherUsersID size: " + otherUsersID.Count);

        // Get the players document from the DB
        foreach (string id in otherUsersID)
        {
            otherPlayersDocRef.Add(dbReference.Collection("Users").Document(id));
        }
        createPlayersAvatars();
    }

    // create player's "avatar" and add them to the scene
    private async void createPlayersAvatars()
    {

        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("3RdPersonPlayer"));
            // the folloigng 2 lines generates random color to player - todo: delete when changing to avatars
            var playerRenderer = referencePlayer.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());
            GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);
            otherPlayersAvatars.Add(avatar);

            await playerDocRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            {
                DocumentSnapshot snapshot = task.Result;
                User player = snapshot.ConvertTo<User>();
                avatar.transform.position = stringToVec(player.location);
                Debug.Log("another player created at: " + player.location);
            });

            Destroy(referencePlayer);
        }
    }

    private Vector3 stringToVec(string s)
    {
        string[] temp = s.Substring(1, s.Length - 2).Split(',');
        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
    }

    // Update is called once per frame
    async void Update()
    {
        Vector3 newLoc;
        Debug.Log("UPDATE");

        // Get the players document from the DB
        for (int i=0; i < otherUsersID.Count; i++)
        {
            DocumentReference docRef = otherPlayersDocRef[i];
            GameObject avatar = otherPlayersAvatars[i];
            await docRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            {
                DocumentSnapshot snapshot = task.Result;
                User player = snapshot.ConvertTo<User>();
                if (player.location != avatar.transform.position.ToString())
                {
                    newLoc = stringToVec(player.location);
                    avatar.transform.position = newLoc;
                    Debug.Log("changing otherPlayerLoc to: " + newLoc);
                }
            });
        }
       
    }
}
