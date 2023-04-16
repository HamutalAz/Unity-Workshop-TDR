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
        //userID = DataBaseManager.userID; // todo: uncomment
        userID = "JQD1GEkcogVOfGodZ1Y5";    // todo: delete

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
        CollectionReference roomMembers = roomDoc.Collection("room_members");
        // todo: uncomment below line & delet following line
        //await roomMembers.WhereNotEqualTo("userName", DataBaseManager.userName).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        await roomMembers.WhereNotEqualTo("userName", "Hamutal​").GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                //Debug.Log(String.Format("Fetched player with ID: " + documentSnapshot.Id));
                otherUsersID.Add(documentSnapshot.Id);
            }
        });

        // Get the players document from the DB
        foreach (string id in otherUsersID)
        {
            otherPlayersDocRef.Add(dbReference.Collection("Users").Document(id));
            //DocumentReference otherPlayerDoc = dbReference.Collection("Users").Document(id);
            //await otherPlayerDoc.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
            //{
            //    DocumentSnapshot snapshot = task.Result;
            //    User player = snapshot.ConvertTo<User>();
            //    otherPlayersData.Add(player);
            //});
        }
        createPlayersAvatars();
    }

    // create player's "avatar" and add them to the scene
    private void createPlayersAvatars()
    {
        foreach (DocumentReference playerDocRef in otherPlayersDocRef)
        {
            GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("3RdPersonPlayer"));
            // the folloigng 2 lines generates random color to player - todo: delete when changing to avatars
            var playerRenderer = referencePlayer.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());
            GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);
            otherPlayersAvatars.Add(avatar);
            //todo: generate location to each player when allocating rooms & store it on db. in the following line fetch it from db
            float x = Random.Range(5f, 11f);
            float z = Random.Range(4f, 10f);
            avatar.transform.position = new Vector3(x, 1.06f, z);

            //todo: delete later
            string loc = avatar.transform.position.ToString();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "location", loc }
            };
            playerDocRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
            {
                Debug.Log("other player location updated to: " + loc);
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

        // Get the players document from the DB
        for(int i=0; i < otherUsersID.Count; i++)
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
