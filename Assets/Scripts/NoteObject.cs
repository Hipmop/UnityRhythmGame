using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoteObject : MonoBehaviour
{
    /// <summary>
    /// ��Ʈ �ϰ� �ӵ�
    /// interval�� ���� ���ؾ���. ��Ʈ�� �и������� ������ ����� �ϰ� �ְ� ������ �ð�ȭ�ϱ� ����, �⺻����(defaultInterval)�� 0.005 �� �����ϰ� ���� (���Ϸ� ������ ���� ��Ʈ �׷����� ��ĥ ���ɼ� ����)
    /// �׷��Ƿ� ��Ʈ�� �ϰ��ϴ� �ӵ��� 5�� �Ǿ����. ex) 0.01 = 10speed, 0.001 = 1speed
    /// </summary>
    public float speed = 5f;

    /// <summary>
    /// ��Ʈ �ϰ�
    /// </summary>
    public abstract void Move();
    public abstract IEnumerator IEMove();

    /// <summary>
    /// ��Ʈ ��ġ���� (�������)
    /// </summary>
    public abstract void SetPosition();
}

public class NoteShort : NoteObject
{
    public override void Move()
    {
        StartCoroutine(IEMove());
    }

    public override IEnumerator IEMove()
    {
        while (true)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            yield return null;
        }
    }

    public override void SetPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y * speed, transform.position.z);
    }
}

public class NoteLong : NoteObject
{
    LineRenderer lineRenderer;
    GameObject head;
    GameObject tail;

    void Awake()
    {
        head = transform.GetChild(0).gameObject;
        tail = transform.GetChild(1).gameObject;
        lineRenderer = head.GetComponent<LineRenderer>();
    }

    public override void Move()
    {
        StartCoroutine(IEMove());
    }

    public override IEnumerator IEMove()
    {
        while (true)
        {
            head.transform.position += Vector3.down * speed * Time.deltaTime;
            tail.transform.position += Vector3.down * speed * Time.deltaTime;
            lineRenderer.SetPositions(new Vector3[]{head.transform.position, tail.transform.position});
            yield return null;
        }
    }

    public override void SetPosition()
    {
        head.transform.position = new Vector3(head.transform.position.x, head.transform.position.y * speed, head.transform.position.z);
        tail.transform.position = new Vector3(tail.transform.position.x, tail.transform.position.y * speed, tail.transform.position.z);

        lineRenderer.SetPositions(new Vector3[] { head.transform.position, tail.transform.position});
    }
}