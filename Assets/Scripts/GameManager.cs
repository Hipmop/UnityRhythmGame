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

    public List<GameObject> canvases = new List<GameObject>();
    enum Canvas
    {
        Title,
        Select,
        SFX,
        GameBGA,
        Game,
        Result,
    }
    CanvasGroup sfxFade;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        score = new Score();

        foreach (GameObject go in canvases)
        {
            go.SetActive(true);
        }
        sfxFade = canvases[(int)Canvas.SFX].GetComponent<CanvasGroup>();
        sfxFade.alpha = 1f;

        UIController.Instance.Init();
        score.Init();

        StartCoroutine(IETitle());
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

    IEnumerator IETitle()
    {
        // UIObject���� �ڱ��ڽ��� ĳ���Ҷ����� ������ �ְ� ��Ȱ��ȭ
        yield return new WaitForSeconds(2f);
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(false);
        canvases[(int)Canvas.Select].SetActive(false);

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 1f));

        // Ÿ��Ʋ ��Ʈ�� ���
        canvases[(int)Canvas.Title].GetComponent<Animation>().Play();
        yield return new WaitForSeconds(5f);

        // ȭ�� ���̵� �ƿ�
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));
        canvases[(int)Canvas.Title].SetActive(false);

        // ����ȭ��(�̱���), �ϴ� �÷��̷� �ٷ�
        Select();
        Play();
    }

    IEnumerator IEInitPlay()
    {
        // ȭ�� ���̵� �ƿ�
        //yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));

        // Sheet �Ľ�
        yield return Parser.Instance.IEParse("Heart Shaker");
        sheet.Init();

        // Audio ����
        AudioManager.Instance.Insert(sheet.clip);

        // Game UI �ѱ�
        canvases[(int)Canvas.Game].SetActive(true);

        // BGA �ѱ�
        canvases[(int)Canvas.GameBGA].SetActive(true);

        // ���� �ʱ�ȭ
        FindObjectOfType<Judgement>().Init();

        // ���� �ʱ�ȭ
        score.Clear();

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));

        // Audio ���
        AudioManager.Instance.Play();

        // Note ����
        NoteGenerator.Instance.StartGen();

        // End �˸���
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
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(true);

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
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));

    }
}
