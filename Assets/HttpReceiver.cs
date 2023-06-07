using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class HttpReceiver : MonoBehaviour
{

    HttpListener listener = null;// new HttpListener();
    private Thread listenerThread = null;

    [System.Serializable]
    public class OnGetReqeustRevent : UnityEvent<HttpListenerContext> { }
    public OnGetReqeustRevent OnGetRequest;

    [System.Serializable]
    public class OnPostReqeustRevent : UnityEvent<HttpListenerContext> { }
    public OnPostReqeustRevent OnPostRequest;

    [SerializeField]
    public TMP_InputField httpPortInputField;

    // Start is called before the first frame update
    void Start()
    {
        StartHttpListen();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestory()
    {
        StopHttpListen();
    }

    private void startListener()
    {
        while (listener.IsListening)
        {
            var result = listener.BeginGetContext(ListenerCallBack, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    void ListenerCallBack(IAsyncResult result)
    {
        if (!listener.IsListening) return;

        HttpListenerContext context = listener.EndGetContext(result);

        Debug.Log("Method: " + context.Request.HttpMethod);
        Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        try
        {
            if (ProcessGetRequest(context)) return;
            if (ProcessPostRequest(context)) return;
        }
        catch(Exception e)
        {
            ReturnInternalError(context.Response, e);
        }
    }

    bool CanAccept(HttpMethod expected, string requested)
    {
        return string.Equals(expected.Method, requested, StringComparison.CurrentCultureIgnoreCase);
    }

    private bool ProcessGetRequest(HttpListenerContext context)
    {
        if (!CanAccept(HttpMethod.Get, context.Request.HttpMethod) ||
            context.Request.IsWebSocketRequest) return false;

        UnityMainThreadDispatcher.Instance().Enqueue(() => OnGetRequest.Invoke(context));

        return true;
    }


    private bool ProcessPostRequest(HttpListenerContext context)
    {
        if (!CanAccept(HttpMethod.Post, context.Request.HttpMethod)) return false;

        UnityMainThreadDispatcher.Instance().Enqueue(() => OnPostRequest.Invoke(context));

        return true;
    }

    private void ReturnInternalError(HttpListenerResponse response, Exception cause)
    {
        Console.Error.WriteLine(cause);

        response.StatusCode = (int) HttpStatusCode.InternalServerError;
        response.ContentType = "text/plain";

        try
        {
            using(var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                writer.Write(cause.ToString());
            response.Close();
        }catch(Exception e)
        {
            Console.Error.Write(e);
            response.Abort();
        }
    }

    public void StartHttpListen()
    {
        //Console.WriteLine("http port {0}", httpPortInputField.text);
        //int port = System.Convert.ToInt32(httpPortInputFieldText.text);// 9090;
        String portStr = httpPortInputField.text.Trim();
        Debug.Log(portStr);

        //int.TryParse(portStr, out port);

        string url = "http://localhost:" + portStr + "/";
        string url2 = "http://127.0.0.1:" + portStr + "/";
        //string url3 = "http://192.168.35.96:" + portStr + "/";
        string url4 = "http://192.168.100.101:" + portStr + "/";
        Debug.Log(url);
        Debug.Log(url2);
        //Debug.Log(url3);
        Debug.Log(url4);
        listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Prefixes.Add(url2);
        //listener.Prefixes.Add(url3);
        //listener.Prefixes.Add(url4);
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

        listenerThread = new Thread(startListener);
        listenerThread.Start();

        Console.WriteLine("Listening on {0}", url);
    }

    public void StopHttpListen()
    {
        if(listener != null)
        {
            listener.Stop();
        }

        if (listenerThread != null)
        {
            listenerThread.Join();
        }

        listener = null;
        listenerThread = null;
    }
}
