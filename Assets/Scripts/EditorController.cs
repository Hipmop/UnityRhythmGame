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

    Camera cam;
    Vector3 worldPos;

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
        cam = Camera.main;

        inputManager = FindObjectOfType<InputManager>();
        audioManager = AudioManager.Instance;
        editor = Editor.Instance;
    }

    void Update()
    {
        // �׸��忡 ���̽��� ��ġ �˾Ƴ�����
        // ���� ������ ����, ������ ��ġ �˾Ƴ�����
        //Debug.Log(inputManager.mousePos);
        Vector3 mousePos = inputManager.mousePos;
        mousePos.z = -cam.transform.position.z;
        worldPos = cam.ScreenToWorldPoint(mousePos);
        //int layerMask = (1 << LayerMask.NameToLayer("Grid")) + (1 << LayerMask.NameToLayer("Note"));

        Debug.DrawRay(worldPos, cam.transform.forward * 2, Color.red, 0.2f);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, cam.transform.forward, 2f);
        if (hit)
        {
            int line = int.Parse(hit.transform.name.Split('_')[1]);
            int index = hit.transform.parent.GetComponent<GridObject>().index;
            if (worldPos.x < -1f && worldPos.x > -2f)
            {
                Debug.Log($"0�� ���� : {index}�� �׸��� : {line} ��Ʈ");
            }
            else if (worldPos.x < 0.5f && worldPos.x > -1f)
            {
                Debug.Log($"1�� ���� : {index}�� �׸��� : {line} ��Ʈ");
            }
            else if (worldPos.x < 1f && worldPos.x > 0.5f)
            {
                Debug.Log($"2�� ���� : {index}�� �׸��� : {line} ��Ʈ");
            }
            else if (worldPos.x < 1.5f && worldPos.x > 1f)
            {
                Debug.Log($"3�� ���� : {index}�� �׸��� : {line} ��Ʈ");
            }
        }
    }

    /// <summary>
    /// �����̽� - ���/�Ͻ�����( Space - Play/Puase )
    /// </summary>
    public void Space()
    {
        Editor.Instance.Play();
    }

    /// <summary>
    /// ��Ŭ�� - ��Ʈ ��ġ ( Mouse leftBtn - Dispose note )
    /// ��Ŭ�� - ��Ʈ ���� ( Mouse rightBtn - Cancel note )
    /// </summary>
    /// <param name="btnName"></param>
    public void MouseBtn(string btnName)
    {
        if (btnName == "leftButton")
        {
            
        }
        else if (btnName == "rightButton")
        {

        }
    }

    /// <summary>
    /// ���콺�� - ���� �� �׸��� ��ġ �̵� ( Mouse wheel - Move music and grids pos )
    /// </summary>
    /// <param name="value"></param>
    public void Scroll(float value)
    {
        scrollValue = value;
    }

    /// <summary>
    /// ��Ʈ�� + ���콺�� - �׸��� ���� ���� ( Ctrl + Mouse wheel - Change snap of grids )
    /// </summary>
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
                GridSnapListener.Invoke(Editor.Instance.Snap);
            }
            else if (scrollValue < 0)
            {
                // �����ٿ�
                Editor.Instance.Snap *= 2;
                GridSnapListener.Invoke(Editor.Instance.Snap);
            }
            scrollValue = 0;

            yield return null;
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 36;
        GUI.Label(new Rect(100, 100, 100, 100), "Mouse Pos : " + inputManager.mousePos.ToString(), style);
        GUI.Label(new Rect(100, 200, 100, 100), "ScreenToWorld : " + worldPos.ToString(), style);
        GUI.Label(new Rect(100, 300, 100, 100), "CurrentBar : " + Editor.Instance.currentBar.ToString(), style);
    }
}
