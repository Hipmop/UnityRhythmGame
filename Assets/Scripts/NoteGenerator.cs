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
    public Material lineRendererMaterial;

    enum NoteType
    {
        Short = 0,
        Long = 1,
    }

    int[] previousLong = { -1, -1, -1, -1 };

    readonly float[] linePos = { -1.5f, -0.5f, 0.5f, 1.5f };
    readonly float defaultInterval = 0.005f; // 1��� ������ (1���� ��ü�� ȭ�鿡 �׷����� ������ ����)

    List<NoteObject> noteList =  new List<NoteObject>();

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
            int line = note.line - 1;
            if (note.type == (int)NoteType.Short)
            {
                GameObject obj = Instantiate(notePrefab, new Vector3(linePos[line], note.time * defaultInterval, 0f), Quaternion.identity, parent.transform);
                obj.AddComponent<NoteShort>();
                noteList.Add(obj.GetComponent<NoteObject>());
            }
            else
            {
                int p = previousLong[line];
                if (p != note.time && p != -1) // ���� Ÿ�Ӱ� ���� Ÿ�Ӱ� �������� �ʴٸ� ��������
                {
                    // Head and Tail
                    GameObject obj = new GameObject("NoteLong");
                    obj.transform.parent = parent.transform;
                    GameObject head = Instantiate(notePrefab, new Vector3(linePos[line], p * defaultInterval, 0f), Quaternion.identity, obj.transform);
                    GameObject tail = Instantiate(notePrefab, new Vector3(linePos[line], note.time * defaultInterval, 0f), Quaternion.identity, obj.transform);

                    head.AddComponent<LineRenderer>();
                    LineRenderer lineRenderer = head.GetComponent<LineRenderer>();
                    lineRenderer.material = lineRendererMaterial;
                    lineRenderer.sortingOrder = 3;
                    lineRenderer.widthMultiplier = 0.8f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPositions(new Vector3[] { head.transform.position, tail.transform.position });

                    obj.AddComponent<NoteLong>(); // ȣ����� ����
                    noteList.Add(obj.GetComponent<NoteObject>());

                    previousLong[line] = -1; // �ճ��� ������
                }
                else
                    previousLong[line] = Mathf.Clamp(note.time, 1, note.time); // �ճ� ������ ���
            }
            
        }

        Drop();
    }

    public void Drop()
    {
        foreach (NoteObject noteObject in noteList)
            noteObject.Move();
    }
}
