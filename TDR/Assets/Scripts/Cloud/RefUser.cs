using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public class RefUser
{
    [FirestoreProperty]
    public string userName { get; set; }

    public RefUser()
    {
    }
    public RefUser(string userName)
    {
        this.userName = userName;

    }
}
