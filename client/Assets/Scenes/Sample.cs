using System.Collections;
using System.Collections.Generic;
using Module.Channel;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public Text TxtStatus;

    public InputField InputUrl;
    
    public InputField InputUsername;

    public InputField InputPassword;

    public Button BtnLogin;
    
    // Start is called before the first frame update
    void Start()
    {
        BtnLogin.onClick.AddListener(OnLogin); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLogin()
    {
        var username = InputUsername.text.Trim();
        var password = InputPassword.text.Trim();
        
        
        
        WsChannel.Instance.Connect();
    }
}
