using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    static Editor instance;
    public static Editor Instance
    {
        get
        {
            return instance;
        }
    }

    UISilder slider = null;
    UIButton musicController = null;

    public GameObject objects;
    Coroutine coMove;

    int snap = 4;
    public int Snap
    {
        get { return snap; }
        set
        {
            snap = Mathf.Clamp(value, 1, 16);
        }
    }

    public int currentBar = 0;
    public float offsetPosition;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    float speed;
    public void Init()
    {
        slider = UIController.Instance.GetUI("UI_E_ProgressBar").uiObject as UISilder;
        musicController = UIController.Instance.GetUI("UI_E_Play").uiObject as UIButton;

        StartCoroutine(IEBarTimer());

        speed = 16 / GameManager.Instance.sheets[GameManager.Instance.title].BarPerSec;
        offsetPosition = speed * GameManager.Instance.sheets[GameManager.Instance.title].offset * 0.001f;
        objects.transform.position = offsetPosition * Vector3.up;
    }

    void Update()
    {
        float value = Mathf.Clamp(1 / AudioManager.Instance.Length * AudioManager.Instance.progressTime, 0f, 1f);
        if (slider != null)
        {
            slider.slider.value = value;
        }
    }

    public void Play()
    {
        if (AudioManager.Instance.IsPlaying())
        {
            AudioManager.Instance.Pause();
            musicController.SetText(">");
            if (coMove != null)
                StopCoroutine(coMove);
        }
        else
        {
            AudioManager.Instance.Play();
            musicController.SetText("||");
            coMove = StartCoroutine(IEMove());
        }
    }

    public void Stop()
    {
        if (coMove != null)
            StopCoroutine(coMove);

        AudioManager.Instance.Stop();
        musicController.SetText(">");
    }

    public void CaculateCurrnetBar()
    {
        currentBar = (int)(AudioManager.Instance.progressTime * 1000 / GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec);
    }

    IEnumerator IEBarTimer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            CaculateCurrnetBar();
            yield return wait;
        }
    }



    IEnumerator IEMove()
    {
        while (true)
        {
            objects.transform.position += Vector3.down * Time.deltaTime * speed;
            yield return null;
        }
    }

    public void Play(UIObject uiObject)
    {
        Play();
    }

    public void Stop(UIObject uiObject)
    {
        Stop();
    }

    public void Progress()
    {
        if (slider != null)
        {
            float time = AudioManager.Instance.Length * slider.slider.value;
            AudioManager.Instance.progressTime = time;

            // 음악 타임에 맞춰서 오브젝트스 이동
            // 한마디에 16씩 이동
            // time / 한마디 시간

            CaculateCurrnetBar();

            // 한 그리드(한 마디)의 게임오브젝트 y좌표의 높이는 16
            // 현재 음악위치 * 16 = 높이s
            float barPerTime = GameManager.Instance.sheets[GameManager.Instance.title].BarPerSec;
            float pos = time / barPerTime * 16;

            objects.transform.position = new Vector3(0f, -pos + offsetPosition, 0f);

        }
    }
}
