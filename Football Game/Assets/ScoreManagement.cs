using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ScoreManagement : MonoBehaviour
{
    public static int goals;
    
    public TMPro.TMP_Text text_goals;
    public GameObject score_card;
    public Animation animation;

    void Start()
    {
    }

    IEnumerator UpdateScoreUI()
    {
        animation.Play();
        yield return new WaitForSeconds(0.3333f);
        text_goals.text = goals.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            goals++;
            StartCoroutine("UpdateScoreUI");
        }
    }

}
