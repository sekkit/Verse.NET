using System.Collections;
using System.Collections.Generic;
using DataModel.Shared.Message;
using Module.Channel;
using Module.Shared;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public Text TxtStatus;

    public InputField InputUrl;
    
    public InputField InputUsername;

    public InputField InputPassword;

    public Button BtnLogin;
    
    public Button BtnTestNtf;
    
    // Start is called before the first frame update
    void Start()
    {
        BtnLogin.onClick.AddListener(OnLogin); 
        BtnTestNtf.onClick.AddListener(OnTestNtf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLogin()
    {
        var username = InputUsername.text.Trim();
        var password = InputPassword.text.Trim();

        var url = InputUrl.text.Trim();
        
        WsChannel.Instance.Connect();
    }

    void OnTestNtf()
    {
        WsChannel.Instance.Invoke(ProtoCode.TEST_NTF, new TestNtfReq());
    }
}
