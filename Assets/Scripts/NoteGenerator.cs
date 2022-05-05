using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    static NoteGenerator instance;
    public static NoteGenerator Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject parent;
    public GameObject notePrefab;

    readonly float[] linePos = { -1.5f, -0.5f, 0.5f, 1.5f };
    readonly float defaultInterval = 0.01f;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }


    // TODO: ���� ������ ����ð� �������� 3���� ����ŭ(1���𿡴� 32*4 ��ŭ�� ������Ʈ�� �� �� �ִ�.)
    // ��Ʈ�� ������ƮǮ ��� ������ ��ġ(*Unity ���� ������ƮǮ�� ���)
    // TODO�� TODO: ���� ���̵����� 1���� ���� ��Ʈ�� ���� �ٸ�. �׷��� 32*4��ŭ�� ������Ʈ�� �׻� �������� �ʿ� ���� ������
    // ���� ������ ���� ������Ʈ�� �����ϰų�, ��� �ʿ��� ������Ʈ�� ���� ���
    // �� �����غ���, ��ü ���� ��ճ��� �������ִ� �͵� ���� ����� �ƴұ�

    public void Gen(Sheet sheet)
    {
        foreach(Note note in sheet.notes)
        {
            Instantiate(notePrefab, new Vector3(linePos[note.line - 1], note.time * defaultInterval, 0f), Quaternion.identity, parent.transform);
        }
    }

    public void Drop()
    {

    }
}
