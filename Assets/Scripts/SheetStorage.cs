using System.IO;
using UnityEngine;

public class SheetStorage : MonoBehaviour
{
    /*
     * ����
        1) ��Ʈ ������Ʈ �о y��ǥ ������� �ð� ���
        BarPerSec / 16 * ��Ʈy��ǥ = ����� �ð�

        �ճ�Ʈ�� ���
        Head y��ǥ = NoteLong�� y��ǥ
        Tail y��ǥ = NoteLong.y + tail.y�� ������ǥ
     */


    void Start()
    {

    }

    public void Save()
    {
        Sheet sheet = GameManager.Instance.sheets[GameManager.Instance.title];

        string notes = string.Empty;
        float baseTime = sheet.BarPerSec / 16;
        foreach (NoteObject note in NoteGenerator.Instance.toReleaseList)
        {
            if (!note.gameObject.activeSelf) // ��Ȱ��ȭ�Ǿ��ִٸ� ������ ��Ʈ�̹Ƿ� ����
                continue;

            float line = note.transform.position.x;
            int findLine = 0;
            if (line < -1f && line > -2f)
            {
                findLine = 0;
            }
            else if (line < 0f && line > -1f)
            {
                findLine = 1;
            }
            else if (line < 1f && line > 0f)
            {
                findLine = 2;
            }
            else if (line < 2f && line > 1f)
            {
                findLine = 3;
            }

            if (note is NoteShort)
            {
                NoteShort noteShort = note as NoteShort;
                int noteTime = (int)(noteShort.transform.position.y * baseTime * 1000);

                notes += $"{noteTime}, {(int)NoteType.Short}, {findLine + 1}\n";
            }
            else if (note is NoteLong)
            {
                NoteLong noteLong = note as NoteLong;
                int headTime = (int)(noteLong.transform.position.y * baseTime * 1000);
                int tailTime = (int)((noteLong.transform.position.y + noteLong.tail.transform.position.y - noteLong.transform.position.y) * baseTime * 1000);
                notes += $"{headTime}, {(int)NoteType.Long}, {findLine + 1}, {tailTime}\n";
            }
        }

        string writer = $"[Description]\n" +
            $"Title: {sheet.title}\n" +
            $"Artist: {sheet.artist}\n\n" +
            $"[Audio]\n" +
            $"BPM: {sheet.bpm}\n" +
            $"Offset: {sheet.offset}\n" +
            $"Signature: {sheet.signature[0]}/{sheet.signature[1]}\n\n" +
            $"[Note]\n" +
            $"{notes}";

        writer.TrimEnd('\r', '\n');

        string pathSheet = $"{Application.dataPath}/Sheet/{sheet.title}/{sheet.title}.sheet";
        if (File.Exists(pathSheet))
        {
            try
            {
                File.Delete(pathSheet);
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }

        if (!File.Exists(pathSheet))
        {
            using (FileStream fs = File.Create(pathSheet))
            {

            }
        }
        else
        {
            Debug.LogError($"{sheet.title}.sheet�� �̹� �����մϴ� !");
            return;
        }

        using (StreamWriter sw = new StreamWriter(pathSheet))
        {
            sw.Write(writer);
        }
    }
}
