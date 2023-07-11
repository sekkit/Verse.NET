using System; 
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Module.Shared; 
using UnityEngine;
using UnityEngine.UI; 
using Module.Shared;

public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        DontDestroyOnLoad(this.gameObject);
        
        UnitySystemConsoleRedirector.Redirect();

        Global.AddSingleton<MainThreadSynchronizationContext>();
        //Global.AddSingleton<Module.Shared.Logger>().ILog = new NLogger("CLIENT", 0, "");
  
    }
     
    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        
    } 
}
