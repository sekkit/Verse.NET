////using Client;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class Main : MonoBehaviour
//{
//    public Button btnLogin;
//    public InputField inputName;
//    public InputField inputPassword; 
//    // Start is called before the first frame update
//    void Start()
//    {
//        //Application.logMessageReceived += Application_logMessageReceived;
//        //Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;

//        this.gameObject.AddComponent<App>();

//        btnLogin.onClick.AddListener(OnLogin);
        
//    }

//    private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
//    {
//        Debug.Log(string.Format("{0}:{1}", condition, stackTrace));
//    }

//    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
//    {
//        Debug.Log(string.Format("{0}:{1}", condition, stackTrace));
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    void OnLogin()
//    {

//    }
//}
