using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// ���� ���� ����. InputManager.OnEnter() ����
    /// </summary>
    public bool isPlaying = true;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();

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
        StartCoroutine(IEInit());
    }

    public void Title()
    {
        StartCoroutine(IETitle());
    }

    public void Select()
    {
        StartCoroutine(IESelect());
    }

    public void Play()
    {
        StartCoroutine(IEInitPlay());
    }

    public void Stop()
    {
        // Game UI ����
        canvases[(int)Canvas.Game].SetActive(false);

        // playing timer ����
        if (coPlaying != null)
        {
            StopCoroutine(coPlaying);
            coPlaying = null;
        }

        // ��Ʈ Gen ����
        NoteGenerator.Instance.StopGen();

        // ���� ����
        AudioManager.Instance.Stop();

        Select();
    }

    IEnumerator IEInit()
    {
        SheetLoader.Instance.Init();

        foreach (GameObject go in canvases)
        {
            go.SetActive(true);
        }
        sfxFade = canvases[(int)Canvas.SFX].GetComponent<CanvasGroup>();
        sfxFade.alpha = 1f;

        UIController.Instance.Init();
        Score.Instance.Init();

        // UIObject���� �ڱ��ڽ��� ĳ���Ҷ����� ������ �ְ� ��Ȱ��ȭ(�ӽ��ڵ�)
        yield return new WaitForSeconds(2f);
        canvases[(int)Canvas.Game].SetActive(false);
        canvases[(int)Canvas.GameBGA].SetActive(false);
        canvases[(int)Canvas.Result].SetActive(false);
        canvases[(int)Canvas.Select].SetActive(false);

        // ����ȭ�� ������ ����
        yield return new WaitUntil(() => SheetLoader.Instance.bLoadFinish == true);
        ItemGenerator.Instance.Init();

        // Ÿ��Ʋ ȭ�� ����
        //Title();
        Select();
    }

    IEnumerator IETitle()
    {
        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 1f));

        // Ÿ��Ʋ ��Ʈ�� ���
        canvases[(int)Canvas.Title].GetComponent<Animation>().Play();
        yield return new WaitForSeconds(5f);

        // ����ȭ�� ����
        Select();
    }

    IEnumerator IESelect()
    {        
        // ȭ�� ���̵� �ƿ�
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));

        // Title UI ����
        canvases[(int)Canvas.Title].SetActive(false);

        // Result UI ����
        canvases[(int)Canvas.Result].SetActive(false);

        // Select UI �ѱ�
        canvases[(int)Canvas.Select].SetActive(true);

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));

        // �� ������ ������ �� �ְ� ����
        isPlaying = false;
    }

    IEnumerator IEInitPlay()
    {        
        // �� ������ ������ �� ���� ����
        isPlaying = true;

        // ȭ�� ���̵� �ƿ�
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, true, 2f));

        //  Select UI ����
        canvases[(int)Canvas.Select].SetActive(false);

        // Sheet �ʱ�ȭ
        title = sheets.ElementAt(ItemController.Instance.page).Key;
        sheets[title].Init();

        // Audio ����
        AudioManager.Instance.Insert(sheets[title].clip);

        // Game UI �ѱ�
        canvases[(int)Canvas.Game].SetActive(true);

        // BGA �ѱ�
        canvases[(int)Canvas.GameBGA].SetActive(true);

        // ���� �ʱ�ȭ
        FindObjectOfType<Judgement>().Init();

        // ���� �ʱ�ȭ
        Score.Instance.Clear();

        // ���� ����Ʈ �ʱ�ȭ
        JudgeEffect.Instance.Init();

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));

        // Audio ���
        AudioManager.Instance.Play();

        // Note ����
        NoteGenerator.Instance.StartGen();

        // End �˸���
        coPlaying = StartCoroutine(IEEndPlay());
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

        rscore.SetText(Score.Instance.data.score.ToString());
        rgreat.SetText(Score.Instance.data.great.ToString());
        rgood.SetText(Score.Instance.data.good.ToString());
        rmiss.SetText(Score.Instance.data.miss.ToString());

        UIImage rBG = UIController.Instance.FindUI("UI_R_BG").uiObject as UIImage;
        rBG.SetSprite(sheets[title].img);

        // ȭ�� ���̵� ��
        yield return StartCoroutine(AniPreset.Instance.IEAniFade(sfxFade, false, 2f));

        // 5�� ���
        yield return new WaitForSeconds(5f);

        // ���� ȭ�� �ҷ�
        Select();
    }
}
