
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json.Linq;


public class SystemInit : MonoBehaviour
{
    GameObject settingDlg = null;
    TMP_Text teamText = null;
    TMP_Text winText = null;
    
    TMP_InputField winInputField = null;
    TMP_InputField failInputField  = null;


    public RequestHandler requestHandler;

    float startY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        
        GameObject obj = GameObject.Find("WinInputField");
        winInputField = obj.GetComponent<TMP_InputField>();
        obj = GameObject.Find("FailInputField");
        failInputField = obj.GetComponent<TMP_InputField>();

        obj = GameObject.Find("TeamText");
        teamText = obj.GetComponent<TMP_Text>();


        settingDlg = GameObject.Find("SettingDlg");
        //settingDlg.SetActive(false);//.GetComponent<Renderer>().enabled = false;
        var tmp = settingDlg.transform.position;
        startY = tmp.y;
        tmp.y = 8192;
        settingDlg.transform.position = tmp;


        obj = GameObject.Find("WinText");
        winText = obj.GetComponent<TMP_Text>();

        
    }


    public float timer = 0.0f;
    int seconds = 0;
    bool winTextFlag = false;

    float refreshTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(winTextFlag == true)
        {
            // seconds in float
            timer += Time.deltaTime;
            // turn seconds in float to int
            seconds = (int)(timer % 60);
            //print(seconds);
            if (2 <= seconds)
            {
                winText.text = "";
                seconds = 0;
                timer = 0.0f;
                winTextFlag = false;
            }
        }
        

        refreshTimer += Time.deltaTime;
        int refreshSeconds = (int)(refreshTimer % 60);
        
        if(refreshSeconds > 5){
            refreshScores();
            refreshTimer = 0;
        }
    }

    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.S.ToString())))
        {
            Debug.Log("S key is pressed.");
            var tmp = settingDlg.transform.position;
            tmp.y = startY;
            settingDlg.transform.position = tmp;
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Escape.ToString())))
        {
            Debug.Log("Escape key is pressed.");
            if (settingDlg.transform.position.y == startY)
            {
                var tmp = settingDlg.transform.position;
                tmp.y = 8192;
                settingDlg.transform.position = tmp;
            }
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Space.ToString())))
        {
            Debug.Log("Space key is pressed.");
            requestHandler.ClearTeamText();
            winText.text = "";
            seconds = 0;
            timer = 0.0f;
            winTextFlag = false;
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Alpha0.ToString()))||
            Event.current.Equals(Event.KeyboardEvent(KeyCode.Keypad0.ToString())))
        {   
            Debug.Log("0 key is pressed.");
            requestHandler.ClearTeamText();
            winText.text = failInputField.text;
            seconds = 0;
            timer = 0.0f;
            winTextFlag = true;
        }
    
        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Alpha1.ToString()))||
            Event.current.Equals(Event.KeyboardEvent(KeyCode.Keypad1.ToString())))
        {
            Debug.Log("1 key is pressed.");
            requestHandler.ClearTeamText();
            winText.text = winInputField.text;
            seconds = 0;
            timer = 0.0f;
            winTextFlag = true;
        }



        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.R.ToString())))
        {
            Debug.Log("R key is pressed.");
            refreshScores();
        }
    }

    void refreshScores(){
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "score.json");

        if (File.Exists(filePath))
        {
            string scoreText = File.ReadAllText(filePath);
            Debug.Log(scoreText);
            JObject jsonObj = JObject.Parse(scoreText);

            for(int i=0;i<4;i++){
                int score = (int)jsonObj[""+(i+1)];
                Debug.Log("score " +score);
            }
        }
        else
        {
            Debug.LogError("score.json 파일이 존재하지 않습니다.");
        }
    }
}
