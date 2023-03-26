using Firebase.Firestore;
using System;
using System.Collections.Generic;
[FirestoreData]
public class User
{
    [FirestoreProperty]
    public string userName { get; set; }

    public User()
    {

    }

    public User(string userName)
    {
        this.userName = userName;
    }

    public override string ToString() {
        return userName;
    }

}
