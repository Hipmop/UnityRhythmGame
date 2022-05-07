using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoteObject : MonoBehaviour
{
    /// <summary>
    /// ��Ʈ �ϰ� �ӵ�
    /// </summary>
    public float speed;

    /// <summary>
    /// ��Ʈ �ϰ�
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// ��Ʈ ��ġ���� (�������)
    /// </summary>
    public abstract void SetPosition();
}

public class NoteShort : NoteObject
{
    GameObject note;

    public override void Move()
    {
        Debug.Log("��");
    }

    public override void SetPosition()
    {
        note.transform.position = Vector3.zero;
    }
}

public class NoteLong : NoteObject
{
    LineRenderer lineRenderer;
    GameObject head;
    GameObject tail;

    void Start()
    {
        head = transform.GetChild(0).gameObject;
        tail = transform.GetChild(1).gameObject;
        lineRenderer = head.GetComponent<LineRenderer>();
    }

    public override void Move()
    {
        Debug.Log("��");
    }

    public override void SetPosition()
    {
        head.transform.position = Vector3.zero;
        tail.transform.position = Vector3.zero;

        lineRenderer.SetPositions(new Vector3[] { head.transform.position, tail.transform.position});
    }
}