using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ScoreData
{
    public int great;
    public int good;
    public int miss;
    public int fastMiss; // ���� �Է��ؼ� �̽�
    public int longMiss; // �ճ�Ʈ �ϼ� ����, miss ī��Ʈ�� ���� ����

    public string[] judgeText;
    public JudgeType judge;
    public int combo;
    public int score 
    { 
        get
        {
            return (great * 500) + (good * 200);
        }
        set
        {
            score = value;
        }
    }
}

public class Score
{
    public ScoreData data;

    UIText uiJudgement;
    UIText uiCombo;
    UIText uiScore;

    public Score()
    {
        data.judgeText = Enum.GetNames(typeof(JudgeType));
    }

    public void Init()
    {
        uiJudgement = UIController.Instance.FindUI("UI_Judgement").uiObject as UIText;
        uiCombo = UIController.Instance.FindUI("UI_Combo").uiObject as UIText;
        uiScore = UIController.Instance.FindUI("UI_Score").uiObject as UIText;
    }

    public void SetScore()
    {
        uiJudgement.SetText(data.judgeText[(int)data.judge]);
        uiCombo.SetText($"{data.combo}");
        uiScore.SetText($"{data.score}");

        UIController.Instance.find.Invoke(uiJudgement.Name);
        UIController.Instance.find.Invoke(uiCombo.Name);
        //UIController.Instance.find.Invoke(uiScore.Name);
    }

    public void Ani(UIObject uiObject)
    {
        Debug.Log($"{uiObject.Name} : ui Ani");
    }
}