using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    private string name;
    private string userId;
    private int ratingPoint;

    public User(string id)
    {
        //서버랑 통신하여 위 데이터를 받아옴
    }

    string GetName()
    {
        return name;
    }

    string GetUserId()
    {
        return userId;
    }

    int GetRatingPoint()
    {
        return ratingPoint;
    }
}
