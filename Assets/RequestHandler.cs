using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class RequestHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGetRequest(HttpListenerContext context)
    {
        Debug.Log("OnGetRequest");
        var request = context.Request;
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";// = "application/json";

        if (request.QueryString.AllKeys.Length > 0)
        {
            foreach(var key in request.QueryString.AllKeys)
            {
                object value = request.QueryString.GetValues(key)[0];
                Debug.Log("Key : " + key + ", value:" + value);
                switch (key)
                {
                    case "GetData":
                        break;
                }
            }
        }


        response.Close( System.Text.Encoding.UTF8.GetBytes("Hello Get") ,false);
    }


    public void OnPostRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";// = "application/json";

        if (request.QueryString.AllKeys.Length > 0)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                object value = request.QueryString.GetValues(key)[0];
                Debug.Log("Key : " + key + ", value:" + value);
                switch (key)
                {
                    case "GetData":
                        break;
                }
            }
        }

        response.Close(System.Text.Encoding.UTF8.GetBytes("Hello Post"), false);
    }
}
