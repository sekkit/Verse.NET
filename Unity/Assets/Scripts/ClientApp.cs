using Client;
using DotNetty.KCP;
using Fenix;
using Fenix.Common;
using Fenix.Common.Utils;
using Server;
using Shared.Protocol;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;
 
public class ClientApp : MonoBehaviour
{
    public void Start()
    {
        UnitySystemConsoleRedirector.Redirect();

        App.Instance.Init();
    }

    public void Update()
    {
        App.Instance.Update();
    }

    public void OnDestroy()
    {
        App.Instance.OnDestroy();
    }
}  
