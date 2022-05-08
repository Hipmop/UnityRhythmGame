using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

    readonly float[] linePos = { -1.5f, -0.5f, 0.5f, 1.5f };
    readonly float defaultInterval = 0.005f; // 1��� ������ (1���� ��ü�� ȭ�鿡 �׷����� ������ ����)

    IObjectPool<NoteShort> poolShort;
    public IObjectPool<NoteShort> PoolShort
    {
        get
        {
            if (poolShort == null)
            {
                poolShort = new ObjectPool<NoteShort>(CreatePooledShort, defaultCapacity:256);
            }
            return poolShort; 
        }
    }
    NoteShort CreatePooledShort()
    {
        GameObject note = Instantiate(notePrefab, parent.transform);
        note.AddComponent<NoteShort>();
        return note.GetComponent<NoteShort>();
    }

    IObjectPool<NoteLong> poolLong;
    public IObjectPool<NoteLong> PoolLong
    {
        get
        {
            if (poolLong == null)
            {
                poolLong = new ObjectPool<NoteLong>(CreatePooledLong, defaultCapacity:64);
            }
            return poolLong;
        }
    }
    NoteLong CreatePooledLong()
    {
        GameObject note = new GameObject("NoteLong");
        note.transform.parent = parent.transform;
        GameObject head = new GameObject("head");
        head.transform.parent = note.transform;
        GameObject tail = new GameObject("tail");
        tail.transform.parent = note.transform;

        head.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = head.GetComponent<LineRenderer>();
        lineRenderer.material = lineRendererMaterial;
        lineRenderer.sortingOrder = 3;
        lineRenderer.widthMultiplier = 0.8f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { head.transform.position, tail.transform.position });

        note.AddComponent<NoteLong>();
        return note.GetComponent<NoteLong>();
    }

    List<NoteObject> noteList =  new List<NoteObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    int currentBar = 3;
    int next = 0;
    int prev = 0;

    public void Gen()
    {
        List<Note> notes = GameManager.Instance.sheet.notes;
        List<Note> reconNotes = new List<Note>();

        for (; next < notes.Count; next++)
        {
            if (notes[next].time > currentBar * 1428) // ���� ����ð���� �ٲ���� 168 BPM / 60 = 2.8, 4���� / 2.8 = 1.428 = 1���� �ð�
            {
                break;
            }
        }
        for (int j = prev; j < next; j++)
        {
            reconNotes.Add(notes[j]);
        }
        prev = next;

        foreach (Note note in reconNotes)
        {
            switch (note.type)
            {
                case (int)NoteType.Short:
                    NoteShort noteShort = PoolShort.Get();
                    // �������� ��Ʈ �ð� - ���� ���� �ð�
                    noteShort.SetPosition(new Vector3(linePos[note.line - 1], (note.time - AudioManager.Instance.GetTime() * 1000) * defaultInterval, 0f));
                    noteShort.Move();
                    // TODO: ��ü ȸ��(Release)�ϴ� �κ� ��������
                    // ȸ�� ���. 1) ��ǥ �� 2) ����� �ð� ��
                    // TODO: �ճ� ���� (�߻�Ŭ�����ʵ� �����ʿ�)
                    break;
                case (int)NoteType.Long:
                    NoteLong noteLong = PoolLong.Get();
                    break;
                default:
                    break;
            }
        }
    }

    public void Init()
    {
        StartCoroutine(IEGenTimer(1.428f));
    }

    IEnumerator IEGenTimer(float interval)
    {
        while (true)
        {
            Gen();
            yield return new WaitForSeconds(interval);
            currentBar++;
        }
    }

    /// <summary>
    /// ��Ʈ�� �� ���� ��� �����ϴ� ���
    /// </summary>
    /// <param name="sheet"></param>
    public void Gen(Sheet sheet)
    {
        foreach (Note note in sheet.notes)
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
                // Head and Tail
                GameObject obj = new GameObject("NoteLong");
                obj.transform.parent = parent.transform;
                GameObject head = Instantiate(notePrefab, new Vector3(linePos[line], note.tail * defaultInterval, 0f), Quaternion.identity, obj.transform);
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
