using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    // Save other player references in here
    // Movement , Collector , Trigger Handlers , etc

    private int guid;

    public int TransformGUID()
    {
        if (guid == 0)
            guid = transform.GetInstanceID();

        return guid;
    }
}