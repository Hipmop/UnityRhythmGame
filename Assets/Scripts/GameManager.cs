using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Sheet sheet;
    public Score score;
    float speed = 1.0f;
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = Mathf.Clamp(value, 1.0f, 5.0f);
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        score = new Score();
        // ĵ���� ���� �״ٰ�
        UIController.Instance.Init();
        // �ʿ��� ĵ���� ����� ����
        score.Init();

        Select();
        Play();
    }


    // ����Ʈ���� �������ϱ� ���� �������� �� �� ������ ��
    public void Select()
    {
        // UI�� �ٹ��̹���, ���ǵ� �̸�����

        // ����ƮUI���� Ŭ���Ǹ� ������ �޾ƿͼ� Insert�� AudioClip �Ѱ��༭ ���ε�
        //AudioManager.Instance.Insert();
        //AudioManager.Instance.Play();

        // �ƹ�ư �ٹ��̹��� ���ε� ��
    }

    // ����Ʈ���� �÷����ϱ� ���� �������� �� �� ������ ��(���� ����)
    public void Play()
    {
        StartCoroutine(IEInitPlay());
    }

    IEnumerator IEInitPlay()
    {
        // ȭ�� ���̵� �ƿ�

        // �Ľ�, ���� ��
        yield return Parser.Instance.IEParse("Heart Shaker");
        sheet.Init();
        AudioManager.Instance.Insert(sheet.clip);
        Judgement judgement = FindObjectOfType<Judgement>();
        judgement.Init();
        score.Clear();
        NoteGenerator.Instance.StartGen();
        //NoteGenerator.Instance.Gen(sheet);

        // ȭ�� ���̵� ��

        // ���� ���
        AudioManager.Instance.Play();
        // ��Ʈ �ϰ�

        StartCoroutine(IEEndPlay());
    }

    // ���� ��
    IEnumerator IEEndPlay()
    {
        while (true)
        {
            if (!AudioManager.Instance.IsPlaying())
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }

        // ȭ�� ���̵� �ƿ�

        UIText rscore = UIController.Instance.FindUI("UI_R_Score").uiObject as UIText;
        UIText rgreat = UIController.Instance.FindUI("UI_R_Great").uiObject as UIText;
        UIText rgood = UIController.Instance.FindUI("UI_R_Good").uiObject as UIText;
        UIText rmiss = UIController.Instance.FindUI("UI_R_Miss").uiObject as UIText;

        rscore.SetText(score.data.score.ToString());
        rgreat.SetText(score.data.great.ToString());
        rgood.SetText(score.data.good.ToString());
        rmiss.SetText(score.data.miss.ToString());

        UIImage rBG = UIController.Instance.FindUI("UI_R_BG").uiObject as UIImage;
        rBG.SetSprite(sheet.img);

        // ȭ�� ���̵� ��
        
        
    }
}
