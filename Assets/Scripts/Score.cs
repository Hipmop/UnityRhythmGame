using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Score
{
    public int great;
    public int good;
    public int miss;
    public int fastMiss; // ���� �Է��ؼ� �̽�
    public int longMiss; // �ճ�Ʈ �ϼ� ����, miss ī��Ʈ�� ���� ����

    public int combo;
    public int score;

    public int Calculate()
    {
        score = (great * 500) + (good * 200);
        return score;
    }
}