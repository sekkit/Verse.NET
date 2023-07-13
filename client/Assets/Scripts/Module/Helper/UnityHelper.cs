using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UnityHelper
{
    public static Vector3 GetMouseWorldPosition(float z=0)
    {
        Vector3 worldPos = Vector3.zero;
        var mousePos = Input.mousePosition;
        
        if(Camera.main.orthographic)
        {
            worldPos = Camera.main.ScreenToWorldPoint(mousePos); 
        }
        else
        {
            //pos += Camera.main.transform.forward * 1000;
            mousePos.z = -Camera.main.transform.position.z;// Camera.main.nearClipPlane;
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        }
        
        worldPos.z = z;
        return worldPos;
    }

    public static async void RunAsync(Action function, Action callback =null) 
    { 
        Func<Task> taskFunc = () => Task.Run(function);
        await taskFunc();
        if (callback != null) 
            callback();
    } 
}
