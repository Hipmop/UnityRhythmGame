using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoteObject : MonoBehaviour
{
    public bool life = false;
    /// <summary>
    /// ��Ʈ �ϰ� �ӵ�
    /// interval�� ���� ���ؾ���. ��Ʈ�� �и������� ������ ����� �ϰ� �ְ� ������ �ð�ȭ�ϱ� ����, �⺻����(defaultInterval)�� 0.005 �� �����ϰ� ���� (���Ϸ� ������ ���� ��Ʈ �׷����� ��ĥ ���ɼ� ����)
    /// �׷��Ƿ� ��Ʈ�� �ϰ��ϴ� �ӵ��� 5�� �Ǿ����. ex) 0.01 = 10speed, 0.001 = 1speed
    /// </summary>
    public float speed = 10f;

    /// <summary>
    /// ��Ʈ �ϰ�
    /// </summary>
    public abstract void Move();
    public abstract IEnumerator IEMove();

    /// <summary>
    /// ��Ʈ ��ġ���� (�������)
    /// </summary>
    public abstract void SetPosition(Vector3[] pos);
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
            if (transform.position.y < -1f)
                life = false;

            yield return null;
        }
    }

    public override void SetPosition(Vector3[] pos)
    {
        transform.position = new Vector3(pos[0].x, pos[0].y, pos[0].z);
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
            if (tail.transform.position.y < -1f)
                life = false;

            yield return null;
        }
    }

    public override void SetPosition(Vector3[] pos)
    {
        head.transform.position = new Vector3(pos[0].x, pos[0].y, pos[0].z);
        tail.transform.position = new Vector3(pos[1].x, pos[1].y, pos[1].z);

        lineRenderer.SetPositions(new Vector3[] { head.transform.position, tail.transform.position});
    }
}