using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;


using TMPro;


public class RequestHandler : MonoBehaviour
{
    public float timer = 0.0f;
    public int seconds;

    public bool teamTextOnFlag = false;
    int limitSeconds = 10;

    [SerializeField]
    public TMP_InputField delayInputField;

    TMP_Text teamText = null;
    public TMP_Text winText = null;

    AudioSource meSound=null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("TeamText");
        teamText = obj.GetComponent<TMP_Text>();

        obj = GameObject.Find("MeSound");
        meSound = obj.GetComponent<AudioSource>();

        ClearTeamText();
    }

    // Update is called once per frame
    void Update()
    {
        if(teamTextOnFlag == true)
        {
            // seconds in float
            timer += Time.deltaTime;
            // turn seconds in float to int
            seconds = (int)(timer % 60);
            //print(seconds);
            if (limitSeconds <= seconds)
            {
                ClearTeamText();
            }
        }
    }

    public void ClearTeamText()
    {
        timer = 0.0f;
        seconds = 0;
        teamText.text = "";
        teamTextOnFlag = false;
    }

    public void ShowTeamText(string text){
        ClearTeamText();
        teamText.text = text ;
        teamTextOnFlag = true;
    }

    public bool RequestProc(string key, string val)
    {
        if (key != "sig") return false;
        Debug.Log("sig in");

        Debug.Log("val : "+val);
        GameObject obj = GameObject.Find("Team" + val + "InputField");
        if (obj == null) return false;
        TMP_InputField teamNameInputField = obj.GetComponent<TMP_InputField>();
        if (teamNameInputField == null) return false;

        SystemInit systemInit = GameObject.Find("SystemInit").GetComponent<SystemInit>();
        systemInit.AddTextEffect(teamNameInputField.text);
        
        if (teamText.text.Trim() != ""){
            return false;
        }

        
        ShowTeamText(teamNameInputField.text);

        limitSeconds = System.Convert.ToInt32(delayInputField.text.Trim());
        //Debug.Log("db3");

        meSound.Play();
        return true;
    }

    public void OnGetRequest(HttpListenerContext context)
    {
        Debug.Log("OnGetRequest");
        var request = context.Request;
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";// = "application/json";

        string res = "N";
        if (request.QueryString.AllKeys.Length > 0)
        {
            foreach(var key in request.QueryString.AllKeys)
            {
                object value = request.QueryString.GetValues(key)[0];
                /*Debug.Log("Key : " + key + ", value:" + value);
                switch (key)
                {
                    case "GetData":
                        break;
                }*/
                if(RequestProc(key, "" + value))
                {
                    res = "Y";
                }

                break;
            }
        }


        response.Close( System.Text.Encoding.UTF8.GetBytes(res) ,false);
    }


    public void OnPostRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";// = "application/json";

        string res = "N";
        if (request.QueryString.AllKeys.Length > 0)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                object value = request.QueryString.GetValues(key)[0];
                /*Debug.Log("Key : " + key + ", value:" + value);
                switch (key)
                {
                    case "GetData":
                        break;
                }*/
                if (RequestProc(key, "" + value))
                {
                    res = "Y";
                }

                break;
            }
        }


        response.Close(System.Text.Encoding.UTF8.GetBytes(res), false);
    }
}
