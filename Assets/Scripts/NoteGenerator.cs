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

    readonly float[] linePos = { -1.5f, -0.5f, 0.5f, 1.5f };
    readonly float defaultInterval = 0.005f; // 1��� ������ (1���� ��ü�� ȭ�鿡 �׷����� ������ ����)
    public float Interval { get; private set; }

    IObjectPool<NoteShort> poolShort;
    public IObjectPool<NoteShort> PoolShort
    {
        get
        {
            if (poolShort == null)
            {
                poolShort = new ObjectPool<NoteShort>(CreatePooledShort, defaultCapacity: 256);
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
                poolLong = new ObjectPool<NoteLong>(CreatePooledLong, defaultCapacity: 64);
            }
            return poolLong;
        }
    }
    NoteLong CreatePooledLong()
    {
        GameObject note = new GameObject("NoteLong");
        note.transform.parent = parent.transform;
        
        GameObject head = Instantiate(notePrefab);
        head.name = "head";
        head.transform.parent = note.transform;

        GameObject tail = Instantiate(notePrefab);
        tail.transform.parent = note.transform;
        tail.name = "tail";

        GameObject line = new GameObject("line");
        line.transform.parent = note.transform;

        line.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = lineRendererMaterial;
        lineRenderer.sortingOrder = 3;
        lineRenderer.widthMultiplier = 0.8f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;

        note.AddComponent<NoteLong>();
        return note.GetComponent<NoteLong>();
    }

    int currentBar = 3; // ���� �÷��� �� 3���� ���� ����
    int next = 0;
    int prev = 0;
    List<NoteObject> toReleaseList = new List<NoteObject>();

    Coroutine coGenTimer;
    Coroutine coReleaseTimer;
    Coroutine coInterpolate;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Ǯ�� ��� ���� (���� �÷��� �� ���)
    public void StartGen()
    {
        Interval = defaultInterval * GameManager.Instance.Speed;
        coGenTimer = StartCoroutine(IEGenTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f)); // ������ 1���� �ð����� ������ ��Ʈ ������Ʈ Ž��
        coReleaseTimer = StartCoroutine(IEReleaseTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f * 0.5f)); // 1���� �ð��� ���� �ֱ�� ������ ��Ʈ ������Ʈ Ž��
        coInterpolate = StartCoroutine(IEInterpolate(0.25f, 3.5f));
    }

    // �� ���� �� ���� (�������Ҷ� ���)
    public void GenAll()
    {
        Gen2();
    }

    public void StopGen()
    {
        if (coGenTimer != null)
        {
            StopCoroutine(coGenTimer);
            coGenTimer = null;
        }
        if (coReleaseTimer != null)
        {
            StopCoroutine(coReleaseTimer);
            coReleaseTimer = null;
        }
        foreach (NoteObject note in toReleaseList)
        {
            note.gameObject.SetActive(false);
        }

        toReleaseList.Clear();
        currentBar = 3;
        next = 0;
        prev = 0;
    }

    void Gen()
    {
        List<Note> notes = GameManager.Instance.sheets[GameManager.Instance.title].notes;
        List<Note> reconNotes = new List<Note>();

        for (; next < notes.Count; next++)
        {
            if (notes[next].time > currentBar * GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec)
            {
                break;
            }
        }
        for (int j = prev; j < next; j++)
        {
            reconNotes.Add(notes[j]);
        }
        prev = next;

        float currentTime = AudioManager.Instance.GetMilliSec();
        float noteSpeed = Interval * 1000;
        foreach (Note note in reconNotes)
        {
            NoteObject noteObject = null;

            switch (note.type)
            {
                case (int)NoteType.Short:
                    noteObject = PoolShort.Get();
                    noteObject.SetPosition(new Vector3[] { new Vector3(linePos[note.line - 1], (note.time - currentTime) * Interval, 0f) });
                    break;
                case (int)NoteType.Long:
                    noteObject = PoolLong.Get();
                    noteObject.SetPosition(new Vector3[] // �������� ��Ʈ �ð� - ���� ���� �ð�
                    {
                        new Vector3(linePos[note.line - 1], (note.time - currentTime) * Interval, 0f),
                        new Vector3(linePos[note.line - 1], (note.tail - currentTime) * Interval, 0f)
                    });
                    break;
                default:
                    break;
            }
            noteObject.speed = noteSpeed;
            noteObject.note = note;
            noteObject.life = true;
            noteObject.gameObject.SetActive(true);
            noteObject.Move();
            toReleaseList.Add(noteObject);
        }
    }

    /// <summary>
    /// Editor Gen�޼ҵ�, ��Ʈ�� �̵��� ��Ʈ �ڽ��� ó������ ����.
    /// </summary>
    void Gen2()
    {
        Sheet sheet = GameManager.Instance.sheets[GameManager.Instance.title];

        List<Note> notes = sheet.notes;

        // (��Ʈ�ð� - ������) / 1��Ʈ(32��Ʈ�ð���) = ��Ʈ�� ��ġ
        // ��Ʈ�� ��ġ * 0.25(�׸����� 32��Ʈ���� ����) = �������� ��Ʈ�� ��ġ

        foreach (Note note in notes)
        {
            NoteObject noteObject = null;

            switch (note.type)
            {
                case (int)NoteType.Short:
                    noteObject = PoolShort.Get();
                    if (note.time - sheet.offset <= 0)
                        noteObject.SetPosition(new Vector3[] { new Vector3(linePos[note.line - 1], 0f, 0f) });
                    else
                        noteObject.SetPosition(new Vector3[] { new Vector3(linePos[note.line - 1], (note.time - sheet.offset) / sheet.BeatPerMilliSec * 0.25f, 0f) });
                    break;
                case (int)NoteType.Long:
                    noteObject = PoolLong.Get();
                    if (note.time - sheet.offset <= 0)
                    {
                        noteObject.SetPosition(new Vector3[] // �������� ��Ʈ �ð� - ���� ���� �ð�
{
                        new Vector3(linePos[note.line - 1], 0f, 0f),
                        new Vector3(linePos[note.line - 1], (note.tail - sheet.offset) / sheet.BeatPerMilliSec * 0.25f, 0f)
});
                    }
                    else
                    {
                        noteObject.SetPosition(new Vector3[] // �������� ��Ʈ �ð� - ���� ���� �ð�
                        {
                        new Vector3(linePos[note.line - 1], (note.time - sheet.offset) / sheet.BeatPerMilliSec * 0.25f, 0f),
                        new Vector3(linePos[note.line - 1], (note.tail - sheet.offset) / sheet.BeatPerMilliSec * 0.25f, 0f)
                        });
                    }
                    break;
                default:
                    break;
            }
            noteObject.note = note;
            noteObject.life = true;
            noteObject.gameObject.SetActive(true);
            toReleaseList.Add(noteObject); // �����ó����� Releaseȣ���ؼ� �������ֱ�
        }
    }

    void Release()
    {
        List<NoteObject> reconNotes = new List<NoteObject>();
        foreach (NoteObject note in toReleaseList)
        {
            if (!note.life)
            {
                if (note is NoteShort)
                    PoolShort.Release(note as NoteShort);
                else
                    PoolLong.Release(note as NoteLong);

                note.gameObject.SetActive(false);
            }
            else
            {
                reconNotes.Add(note);
            }
        }
        toReleaseList.Clear();
        toReleaseList.AddRange(reconNotes);
    }

    public void Interpolate()
    {
        if (coInterpolate != null)
            StopCoroutine(coInterpolate);

        coInterpolate = StartCoroutine(IEInterpolate());
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

    IEnumerator IEReleaseTimer(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            Release();
        }
    }

    IEnumerator IEInterpolate(float rate = 1f, float duration = 1f)
    {
        float time = 0;
        Interval = defaultInterval * GameManager.Instance.Speed;
        float noteSpeed = Interval * 1000;
        while (time < duration)
        {
            float milli = AudioManager.Instance.GetMilliSec();

            foreach (NoteObject note in toReleaseList)
            {
                note.speed = noteSpeed;
                note.Interpolate(milli, Interval);
            }
            time += rate;
            yield return new WaitForSeconds(rate);
        }
    }
}
