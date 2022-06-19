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

    public bool bCtrl;
    float scrollValue;
    Coroutine coCtrl;

    InputManager inputManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    void Update()
    {
        
        //inputManager.mousePos;
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
        while (bCtrl)
        {
            if (scrollValue >= 0)
            {
                // ������
            }
            else
            {
                // �����ٿ�
            }

            yield return null;
        }
    }
}
