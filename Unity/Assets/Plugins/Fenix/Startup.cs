
using UnityEngine; 

public class Startup
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        UnitySystemConsoleRedirector.Redirect();
    }

#if UNITY_EDITOR


    [UnityEditor.InitializeOnLoadMethod]
    static void EditorInitialize()
    {
        Initialize();
    }

#endif
}