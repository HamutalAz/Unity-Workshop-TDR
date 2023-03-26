using System;
using System.Collections.Generic;
public class User
{
    
    public string userName;

    public User(string userName)
    {
        this.userName = userName;
    }


    public string user_name
    {
        get => userName;
    }

    public override string ToString() {
        return userName;
    }

}
