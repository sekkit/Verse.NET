using Client;
using Fenix.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Button btnStart;
    public ClientApp app;
    // Start is called before the first frame update
    void Start()
    {
        btnStart.onClick.AddListener(OnLogin);
    }

    void OnLogin()
    {
        app.Login("sekkit", "password", (code, avatar) =>
        {
            Log.Info("login_result", code);
        });
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
