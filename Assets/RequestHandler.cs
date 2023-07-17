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

    TMP_Text  timeOutText =null;

    AudioSource meSound=null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("TeamText");
        teamText = obj.GetComponent<TMP_Text>();

        obj = GameObject.Find("TimeOutText");
        timeOutText = obj.GetComponent<TMP_Text>();

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
            }else{
                
                timeOutText.text = "" + (limitSeconds-seconds);
            }

        }
    }

    public void ClearTeamText()
    {
        timer = 0.0f;
        seconds = 0;
        teamText.text = "";
        teamTextOnFlag = false;
        timeOutText.text = "";
    }

    public void ShowTeamText(string text){
        ClearTeamText();
        teamText.text = text ;
        teamTextOnFlag = true;
    }

    public int teamNumFromSig(string val){
        TMP_InputField obj1 = GameObject.Find("Sig1InputField").GetComponent<TMP_InputField>();
        TMP_InputField obj2 = GameObject.Find("Sig2InputField").GetComponent<TMP_InputField>();
        TMP_InputField obj3 = GameObject.Find("Sig3InputField").GetComponent<TMP_InputField>();
        TMP_InputField obj4 = GameObject.Find("Sig4InputField").GetComponent<TMP_InputField>();

        if(obj1.text.Trim() == val) return 1;
        else if(obj2.text.Trim() == val) return 2;
        else if(obj3.text.Trim() == val) return 3;
        else if(obj4.text.Trim() == val) return 4;


        return 0;
    }

    public bool RequestProc(string key, string val)
    {
        if (key != "sig") return false;
        Debug.Log("sig in : "+val);

        int teamNum = teamNumFromSig(val);
        
        if(teamNum == 0) return false;


        Debug.Log("teamNum : " + teamNum);
        GameObject obj = GameObject.Find("Team" + teamNum + "InputField");
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
