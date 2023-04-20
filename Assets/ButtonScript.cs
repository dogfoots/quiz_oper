using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickHttpApply()
    {
        Debug.Log("HttpApply Clicked");

        GameObject httpReceiverObj = GameObject.Find("HttpReceiver");
        HttpReceiver httpReceiver = httpReceiverObj.GetComponent<HttpReceiver>();
        httpReceiver.StopHttpListen();
        httpReceiver.StartHttpListen();
    }
}
