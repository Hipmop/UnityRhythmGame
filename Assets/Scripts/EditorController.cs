using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour
{
    static EditorController instance;
    public static EditorController Instance
    {
        get
        {
            return instance;
        }
    }

    public bool isCtrl;
    float scrollValue;
    Coroutine coCtrl;

    InputManager inputManager;
    AudioManager audioManager;
    Editor editor;

    public Action<int> GridSnapListener;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        audioManager = AudioManager.Instance;
        editor = Editor.Instance;
    }

    void Update()
    {
        // �׸��忡 ���̽��� ��ġ �˾Ƴ�����
        // ���� ������ ����, ������ ��ġ �˾Ƴ�����
        Debug.Log(inputManager.mousePos);
    }

    // �����̽� - ���/�Ͻ�����( Space - Play/Puase )
    public void Space()
    {
        Editor.Instance.Play();
    }

    // ���콺��Ŭ�� - ��Ʈ ��ġ ( Mouse leftBtn - Dispose note )
    // ���콺��Ŭ�� - ��Ʈ ���� ( Mouse rightBtn - Cancel note )
    public void MouseBtn(string btnName)
    {
        if (btnName == "leftButton")
        {
            
        }
        else if (btnName == "rightButton")
        {

        }
    }

    // ���콺�� - ���� �� �׸��� ��ġ �̵� ( Mouse wheel - Move music and grids pos )
    public void Scroll(float value)
    {
        scrollValue = value;
    }

    // ��Ʈ�� + ���콺�� - �׸��� ���� ���� ( Ctrl + Mouse wheel - Change snap of grids )
    public void Ctrl()
    {
        if (coCtrl != null)
        {
            StopCoroutine(coCtrl);
        }
        coCtrl = StartCoroutine(IEWaitMouseWheel());
    }

    IEnumerator IEWaitMouseWheel()
    {
        while (isCtrl)
        {
            if (scrollValue > 0)
            {
                // ������
                Editor.Instance.Snap /= 2;
            }
            else if (scrollValue < 0)
            {
                // �����ٿ�
                Editor.Instance.Snap *= 2;
            }
            scrollValue = 0;

            GridSnapListener.Invoke(Editor.Instance.Snap);

            yield return null;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 100, 100), scrollValue.ToString());
        GUI.Label(new Rect(100, 200, 100, 100), Editor.Instance.Snap.ToString());
    }
}
