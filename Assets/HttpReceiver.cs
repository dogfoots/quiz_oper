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

public class HttpReceiver : MonoBehaviour
{

    HttpListener listener = new HttpListener();
    private Thread listenerThread;

    [System.Serializable]
    public class OnGetReqeustRevent : UnityEvent<HttpListenerContext> { }
    public OnGetReqeustRevent OnGetRequest;

    [System.Serializable]
    public class OnPostReqeustRevent : UnityEvent<HttpListenerContext> { }
    public OnPostReqeustRevent OnPostRequest;

    // Start is called before the first frame update
    void Start()
    {
        string url = "http://localhost:8080/";
        listener.Prefixes.Add(url);
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

        listenerThread = new Thread(startListener);
        listenerThread.Start();

        Console.WriteLine("Listening on {0}", url);

    }

    // Update is called once per frame
    void Update()
    {
        /*HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        Console.WriteLine("{0} {1}", request.HttpMethod, request.Url);

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Hello, World!");
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();*/
    }

    void OnDestory()
    {
        listener.Stop();
        listenerThread.Join();
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
}
