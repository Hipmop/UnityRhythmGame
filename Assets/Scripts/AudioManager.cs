using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
            instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    public void Play()
    {
        audioSource.Play();
    }

    public IEnumerator IEInsertClip(string title) // ��ũ��Ʈ �и��� sheet�� �̵��� �ʿ��غ���
    {
        AudioClip clip;
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"{Application.dataPath}/Sheet/{title}/{title}.mp3", AudioType.MPEG))
        {
            yield return request.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(request);
            clip.name = title;
        }
        audioSource.clip = clip;
    }

    public float GetMilliSec()
    {
        return audioSource.time * 1000;
    }
}
