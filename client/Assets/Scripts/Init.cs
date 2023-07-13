 
using Module.Channel;
using Module.Shared;
using Newtonsoft.Json;
using UnityEngine; 

public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        DontDestroyOnLoad(this.gameObject);
        
        UnitySystemConsoleRedirector.Redirect();

        Global.AddSingleton<EnvironmentV2>();
        Global.AddSingleton<MainThreadSynchronizationContext>();
        Global.AddSingleton<Module.Shared.Logger>().ILog = new NLogger("CLIENT", 0, "");
        Global.AddSingleton<ProtocolProvider>();
        Global.AddSingleton<WsChannel>();
        
        Global.Start();
        
        new GameObject("ClientStub").AddComponent<ClientStub>();

        Log.Info(JsonConvert.SerializeObject(EnvironmentV2.Instance.systemInfo, Formatting.Indented));
    }
     
    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        Global.Update();
    }

    private void LateUpdate()
    {
        Global.LateUpdate();
        Global.FrameFinishedUpdate();
    }  
}
