using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct HighScore
{
    public string usrName;
    public int score;

    public HighScore(string _usrName, int _score)
    {
        usrName = _usrName;
        score = _score;
    }
}

public class Leaderboard : MonoBehaviour
{
    public GameObject[] entriesUI;
    public TMPro.TMP_InputField nameInput;
    public Button addHSButton;
    public HighScore[] highscores = new HighScore[5];

    public ScoreManagement scoreManager;
    
    const string privCode = "ZMI2OeH44EW4ZhsfcnduEgOVVisu4b5kOeaBeQ68UH2Q";
    const string pubCode = "5e9c14000cf2aa0c28abfbad";
    const string webUrl = "http://dreamlo.com/lb/";

    void Awake()
    {
        DownloadHighScore();
        UpdateLeaderbaordUI();
    }

    public void AddNewHighScore()
    {
        StartCoroutine(UploadNewHighscore(nameInput.text.ToString()));
        nameInput.enabled = false;
        addHSButton.enabled = false;
        nameInput.text = "Score Added";
    }

    IEnumerator UploadNewHighscore(string username)
    {
        WWW www = new WWW(webUrl + privCode + "/add/" + WWW.EscapeURL(username) + "/" + ScoreManagement.score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Leaderboard Uploaded");
        } else
        {
            print("Error Uploading Leaderboard : "+www.error);
        }
    }

    public void DownloadHighScore()
    {
        StartCoroutine("DownloadHighscores");
    }

    IEnumerator DownloadHighscores()
    {
        WWW www = new WWW(webUrl + pubCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Leaderboard Downloaded");
            // Store the data in www.text

            // store only get top 5
            string data = www.text;
            string[] entries = data.Split(new char[] { '\n' });

            if (entries.Length >= highscores.Length)
            {
                for (int i = 0; i < highscores.Length; i++)
                {
                    string[] entryData = entries[i].Split(new char[] { '|' });
                    string username = entryData[0];
                    int score = int.Parse(entryData[1]);
                    highscores[i] = new HighScore(username, score);
                }
            } else
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    string[] entryData = entries[i].Split(new char[] { '|' });
                    string username = entryData[0];
                    int score = int.Parse(entryData[1]);
                    highscores[i] = new HighScore(username, score);
                }
            }
            

            UpdateLeaderbaordUI();
        }
        else
        {
            print("Error Downloading Leaderboard : " + www.error);
        }
    }

    public void UpdateLeaderbaordUI()
    {

        for (int i = 0; i < entriesUI.Length; i++)
        {
            entriesUI[i].transform.FindChild("Text_Place").GetComponent<TMPro.TMP_Text>().text = (i + 1).ToString();
            entriesUI[i].transform.FindChild("Text_Name").GetComponent<TMPro.TMP_Text>().text = highscores[i].usrName;
            entriesUI[i].transform.FindChild("Text_Score").GetComponent<TMPro.TMP_Text>().text = highscores[i].score.ToString();
        }
    }

}
