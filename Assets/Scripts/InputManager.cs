using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameObject[] keyEffects = new GameObject[4];
    Judgement judgement = null;
    Sync sync = null;

    void Start()
    {
        foreach (var effect in keyEffects)
        {
            effect.gameObject.SetActive(false);
        }
        judgement = FindObjectOfType<Judgement>();
        sync = FindObjectOfType<Sync>();
    }

    public void OnNoteLine0(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(0);
            keyEffects[0].gameObject.SetActive(true);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(0);
            keyEffects[0].gameObject.SetActive(false);
        }
    }
    public void OnNoteLine1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(1);
            keyEffects[1].gameObject.SetActive(true);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(1);
            keyEffects[1].gameObject.SetActive(false);
        }
    }
    public void OnNoteLine2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(2);
            keyEffects[2].gameObject.SetActive(true);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(2);
            keyEffects[2].gameObject.SetActive(false);
        }
    }
    public void OnNoteLine3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(3);
            keyEffects[3].gameObject.SetActive(true);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(3);
            keyEffects[3].gameObject.SetActive(false);
        }
    }
    public void OnSpeedDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.Speed -= 0.1f;
            NoteGenerator.Instance.Interpolate();

            UIText speedUI = UIController.Instance.find.Invoke("UI_G_Speed").uiObject as UIText;
            speedUI.SetText("Speed " + GameManager.Instance.Speed.ToString("0.0"));
        }
    }
    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.Speed += 0.1f;
            NoteGenerator.Instance.Interpolate();

            UIText speedUI = UIController.Instance.find.Invoke("UI_G_Speed").uiObject as UIText;
            speedUI.SetText("Speed " + GameManager.Instance.Speed.ToString("0.0"));
        }
    }
    public void OnJudgeDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.isPlaying)
                sync.Down();
        }
    }
    public void OnJudgeUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.isPlaying)
                sync.Up();
        }
    }

    public void OnItemMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ItemController.Instance.Move(context.ReadValue<float>());
        }
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!GameManager.Instance.isPlaying)
                GameManager.Instance.Play();
        }
    }
    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.isPlaying)
                GameManager.Instance.Stop();
        }
    }

    // 테스트용 코드
    public void OnTest(InputAction.CallbackContext context)
    {
        // Audio Time을 끝으로 옮겨 결과창을 바로 볼 수 있게 위함
        AudioManager.Instance.audioSource.time = AudioManager.Instance.Length;
    }
}
