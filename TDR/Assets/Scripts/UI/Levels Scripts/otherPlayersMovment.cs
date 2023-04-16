using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class otherPlayersMovment : MonoBehaviour
{
    private static FirebaseFirestore dbReference;
    private DocumentReference otherPlayerDoc;
    // Start is called before the first frame update
    void Start()
    {
        dbReference = FirebaseFirestore.DefaultInstance;
        otherPlayerDoc = dbReference.Collection("Users").Document("WlELfN9DofdgUnQWCyfB"); // todo: delete & get it directly from room collection
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newLoc;
        otherPlayerDoc.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            DocumentSnapshot snapshot = task.Result;
            User player = snapshot.ConvertTo<User>();
            newLoc = stringToVec(player.location);
            if (newLoc != transform.position)
            {
                transform.position = newLoc;
                Debug.Log("changing otherPlayerLoc to: " + newLoc);
            }
        });
    }

    public Vector3 stringToVec(string s)
    {
        string[] temp = s.Substring(1, s.Length - 2).Split(',');
        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
    }
}