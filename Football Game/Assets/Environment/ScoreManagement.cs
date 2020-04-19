using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ScoreManagement : MonoBehaviour
{
    public static int score = 0;
    
    public TMPro.TMP_Text text_score;
    public GameObject score_card;
    public Animation animation;

    void Start()
    {
    }

    IEnumerator UpdateScoreUI()
    {
        animation.Play();
        yield return new WaitForSeconds(0.66666f);
        text_score.text = score.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            score++;
            StartCoroutine("UpdateScoreUI");
        }
    }

}
