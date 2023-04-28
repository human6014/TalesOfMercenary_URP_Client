using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
public static class MyUUIDGeneration 
{
    public static string GenrateUUID()
    {
        return Guid.NewGuid().ToString();
    }
   
}
