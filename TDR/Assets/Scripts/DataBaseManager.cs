using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;

public class DataBaseManager : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI feedbackLBL;
    private string userID;
    private DatabaseReference dbReference;

    // Start is called before the first frame update
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    public void CreateUser()
    {
        User newUser = new User(userName.text);
        string json = JsonUtility.ToJson(newUser);
        Debug.Log("trying to create user:" + userName.text);
        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);

        //todo: later check if such user exists, if so change feedback label to a better feedback.
        feedbackLBL.text = "trying to create user with username: " + userName.text;
    }
    
}
