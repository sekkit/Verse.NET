using DotNetty.Common.Utilities;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using MessagePack.Resolvers;
using System;
using UnityEngine;

//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack;
using System.ComponentModel;
using System;

public class Startup
{
    static bool serializerRegistered = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        UnitySystemConsoleRedirector.Redirect();

        if (!serializerRegistered)
        { 
            StaticCompositeResolver.Instance.Register(
                 MessagePack.Resolvers.ClientAppResolver.Instance,
                 MessagePack.Resolvers.FenixRuntimeResolver.Instance,
                 MessagePack.Resolvers.SharedResolver.Instance,
                 MessagePack.Unity.UnityResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }

        Console.WriteLine(typeof(int).GUID);
        Console.WriteLine(typeof(float).GUID);
    }

#if UNITY_EDITOR


    [UnityEditor.InitializeOnLoadMethod]
    static void EditorInitialize()
    {
        Initialize();
    }

#endif
}