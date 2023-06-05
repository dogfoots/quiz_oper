
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

using TMPro;

public class SystemInit : MonoBehaviour
{
    JObject scoreJsonObj = null;
    GameObject teamScoreView = null;
    GameObject effectGround = null;
    
    GameObject teamScore1Grp = null;
    GameObject teamScore2Grp = null;
    GameObject teamScore3Grp = null;
    GameObject teamScore4Grp = null;

    TMP_Text score1 = null;
    TMP_Text scoreTitle1 = null;
    
    TMP_Text score2 = null;
    TMP_Text scoreTitle2 = null;
    
    TMP_Text score3 = null;
    TMP_Text scoreTitle3 = null;
    
    TMP_Text score4 = null;
    TMP_Text scoreTitle4 = null;


    GameObject settingDlg = null;
    TMP_Text teamText = null;
    TMP_Text winText = null;
    
    TMP_InputField winInputField = null;
    TMP_InputField failInputField  = null;

    AudioSource okSound=null;
    AudioSource noSound=null;

    public RequestHandler requestHandler;
    public UnityMainThreadDispatcher unityMainThreadDispatcher;

    public TMP_FontAsset effectFontTMP;

    float startY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        unityMainThreadDispatcher.CreateInstance();
        effectGround =  GameObject.Find("EffectGround");

        teamScoreView = GameObject.Find("TeamScores");
        
        teamScore1Grp = GameObject.Find("TeamScore1Grp");
        teamScore2Grp = GameObject.Find("TeamScore2Grp");
        teamScore3Grp = GameObject.Find("TeamScore3Grp");
        teamScore4Grp = GameObject.Find("TeamScore4Grp");

        scoreTitle1 = GameObject.Find("ScoreTitleText1").GetComponent<TMP_Text>();
        score1 = GameObject.Find("ScoreText1").GetComponent<TMP_Text>();
        scoreTitle2 = GameObject.Find("ScoreTitleText2").GetComponent<TMP_Text>();
        score2 = GameObject.Find("ScoreText2").GetComponent<TMP_Text>();
        scoreTitle3 = GameObject.Find("ScoreTitleText3").GetComponent<TMP_Text>();
        score3 = GameObject.Find("ScoreText3").GetComponent<TMP_Text>();
        scoreTitle4 = GameObject.Find("ScoreTitleText4").GetComponent<TMP_Text>();
        score4 = GameObject.Find("ScoreText4").GetComponent<TMP_Text>();


        GameObject obj = GameObject.Find("WinInputField");
        winInputField = obj.GetComponent<TMP_InputField>();
        obj = GameObject.Find("FailInputField");
        failInputField = obj.GetComponent<TMP_InputField>();

        obj = GameObject.Find("OkSound");
        okSound = obj.GetComponent<AudioSource>();
        
        obj = GameObject.Find("NoSound");
        noSound = obj.GetComponent<AudioSource>();
        

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

        AddTextEffect("hi!");
    }


    public float timer = 0.0f;
    int seconds = 0;
    bool winTextFlag = false;

    float refreshTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        //뭔가가 떠있으면 나타나면 점수현황 안됨.
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
        
        if(winTextFlag == false &&
            requestHandler.teamTextOnFlag == false ){
            //점수현황 표시할것
            RectTransform rf = teamScoreView.GetComponent<RectTransform>();
            rf.sizeDelta = new Vector2(Screen.width , Screen.height);

            teamScore1Grp.transform.position = new Vector3(0, Screen.height, teamScore1Grp.transform.position.z);
            teamScore2Grp.transform.position = new Vector3(Screen.width, Screen.height, teamScore2Grp.transform.position.z);
            teamScore3Grp.transform.position = new Vector3(0, 0, teamScore3Grp.transform.position.z);
            teamScore4Grp.transform.position = new Vector3(Screen.width, 0, teamScore4Grp.transform.position.z);
            //Debug.Log("d : "+teamScoreView.GetComponent<RectTransform>().rect.width);
            //Debug.Log("d : "+teamScore1Grp.transform.position);
            
            refreshTeamName(1, scoreTitle1);
            refreshTeamName(2, scoreTitle2);
            refreshTeamName(3, scoreTitle3);
            refreshTeamName(4, scoreTitle4);

            teamScoreView.transform.position = new Vector3(0, teamScoreView.transform.position.y, teamScoreView.transform.position.z);

            if(scoreJsonObj != null){
                setScoreText((int)scoreJsonObj["1"],1);
                setScoreText((int)scoreJsonObj["2"],2);
                setScoreText((int)scoreJsonObj["3"],3);
                setScoreText((int)scoreJsonObj["4"],4);
            }

        }else{
            teamScoreView.transform.position = new Vector3(-8192, teamScoreView.transform.position.y, teamScoreView.transform.position.z);
        }


        //점수현황 갱신
        refreshTimer += Time.deltaTime;
        int refreshSeconds = (int)(refreshTimer % 60);
        
        if(refreshSeconds > 5){
            refreshScores();
            refreshTimer = 0;
        }
    }

    class EffectText{
        GameObject gameObject = null;

        public void make(string text, GameObject effectGround, TMP_FontAsset effectFontTMP){
            gameObject = new GameObject("EffectText");
            gameObject.AddComponent<CanvasRenderer>();
            gameObject.AddComponent<TextMeshProUGUI>();
            gameObject.transform.parent = effectGround.transform;
            gameObject.transform.position = new Vector3((Screen.width/2) + UnityEngine.Random.Range(-25.0f,25.0f), 100, 0);

            TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
            tmp.font = effectFontTMP;
            tmp.fontSize = 36;
            tmp.fontSharedMaterial = effectFontTMP.material;

            gameObject.GetComponent<TMP_Text>().text = text;
        }
    }

    public void AddTextEffect(string text){
        EffectText ef = new EffectText();
        ef.make(text, effectGround, effectFontTMP);
    }

    protected void setScoreText(int score, int val){
        GameObject obj = GameObject.Find("ScoreText" + val);
        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.text = ""+score;
    }

    public void refreshTeamName(int  val, TMP_Text textBox){
        GameObject obj = GameObject.Find("Team" + val + "InputField");
        TMP_InputField teamNameInputField = obj.GetComponent<TMP_InputField>();
        if (teamNameInputField == null) return;
        //ShowTeamText(teamNameInputField.text);
        textBox.text = teamNameInputField.text;
    }
    
    public void ClearWinFailText()
    {
        winText.text = "";
        seconds = 0;
        timer = 0.0f;
        winTextFlag = false;

    }

    void showWinFailText(string text){
        winText.text = text;//winInputField.text;
        seconds = 0;
        timer = 0.0f;
        winTextFlag = true;
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
            ClearWinFailText();
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Alpha0.ToString()))||
            Event.current.Equals(Event.KeyboardEvent(KeyCode.Keypad0.ToString())))
        {   
            Debug.Log("0 key is pressed.");
            requestHandler.ClearTeamText();
            showWinFailText(failInputField.text);
            noSound.Play();
        }
    
        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Alpha1.ToString()))||
            Event.current.Equals(Event.KeyboardEvent(KeyCode.Keypad1.ToString())))
        {
            Debug.Log("1 key is pressed.");
            requestHandler.ClearTeamText();
            showWinFailText(winInputField.text);
            okSound.Play();
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
            scoreJsonObj = JObject.Parse(scoreText);

            for(int i=0;i<4;i++){
                int score = (int)scoreJsonObj[""+(i+1)];
                Debug.Log("score " +score);
            }
        }
        else
        {
            Debug.LogError("score.json 파일이 존재하지 않습니다.");
        }
    }
}
