using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // static �ؼ�����ζ�� �����Ա������������ģ����������ʵ����Ҳ����˵�����۴������ٸ�����Ķ��������̬��Ա������ֻ��һ�ݿ���
    public static AudioManager instance;

    [SerializeField] private float sfxMinimumDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBgm;
    private int bgmIndex;

    private bool canPlaySFX;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);  //ɾ��instance,������ж��ʵ���Ļ�����һ��������ֵ������Ľ������٣������C#�е� ����
        }
        else
        {
            instance = this;
        }

        Invoke("AllowSFX", 1f); // ����Ҳ�Ǽ���ʱ������ һЩ����ԭ���µĽű�˳��������⣬��ʹ�տ�ʼ�Ͳ��Ÿ�����Ч

    }

    private void Update()
    {
        if (!playBgm)
        {
            StopAllBGM();
        }
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                PlayRandomBGM();
            }
        }
    }
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        //if(sfx[_sfxIndex].isPlaying)
        //{
        //return;
        //}

        if (canPlaySFX == false)
        {
            return;
        }

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
        {
            return;
        }

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    public void StopSFXWtihTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    private IEnumerator DecreaseVolume(AudioSource _audio)  // ����Э���� �����仯ʱ�� ������Ч����Ȼ����
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }


    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX()
    {
        canPlaySFX = true;
    }

}
