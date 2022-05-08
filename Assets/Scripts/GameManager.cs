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

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        Select();
        Play();
    }

    void Update()
    {
        
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
        // ȭ�� ���̵� �ƿ�

        // �Ľ�, ���� ��
        sheet = Parser.Instance.Parse("Heart Shaker");
        NoteGenerator.Instance.Gen(sheet);

        // ȭ�� ���̵� ��
        
        // ���� ���
        AudioManager.Instance.Play();
        // ��Ʈ �ϰ�
    }
}
