using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // static 关键字意味着 这个成员变量是属于类的，而不是类的实例。也就是说，无论创建多少个该类的对象，这个静态成员变量都只有一份拷贝
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
            Destroy(instance.gameObject);  //删除instance,即如果有多个实例的话，第一个将被赋值，其余的将被销毁，这就是C#中的 单例
        }
        else
        {
            instance = this;
        }

        Invoke("AllowSFX", 1f); // 这里也是加延时，避免 一些其他原因导致的脚本顺序出错问题，致使刚开始就播放各种音效

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

    private IEnumerator DecreaseVolume(AudioSource _audio)  // 做个协程让 环境变化时的 环境音效能自然过渡
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
