using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    public List<AudioClip> BGMClip;
    public List<AudioClip> SFXClip;

    public int MaxAudioSource = 5;

    AudioSource BGSource;
    AudioSource[] SFXSource;

    private void OnEnable()
    {
        //BGM AudioSource Setup
        BGSource = gameObject.AddComponent<AudioSource>();
        BGSource.volume = 1;
        BGSource.playOnAwake = false;
        BGSource.loop = true;

        SFXSource = new AudioSource[MaxAudioSource];

        for (int i = 0; i < SFXSource.Length; ++i)
        {
            SFXSource[i] = gameObject.AddComponent<AudioSource>();
            SFXSource[i].volume = 1;
            SFXSource[i].playOnAwake = false;
            SFXSource[i].loop = false;
        }

        PlayBGM(BGMClip[0].name);
    }

    public void PlayBGM(string name, bool isLoop= true, float volume = 1.0f)
    {
        for (int i = 0; i < BGMClip.Count; ++i)
        {
            if (BGMClip[i].name == name)
            {
                AudioSource Source = BGSource;
                Source.clip = BGMClip[i];
                Source.loop = isLoop;
                Source.volume = volume;
                Source.Play();

                return;
            }
        }
    }

    public void StopBGM()
    {
        if(BGSource)
        {
            if(BGSource.isPlaying)
            {
                BGSource.Stop();
            }
        }
    }


    public void PlaySFX(string name, float volume =1.0f, bool isLoop = false)
    {
        for (int i = 0; i < SFXClip.Count; ++i)
        {
            if (SFXClip[i].name == name)
            {
                AudioSource Source = GetEmptyAudioSource();
                Source.clip = SFXClip[i];
                Source.loop = isLoop;
                Source.volume = volume;
                Source.Play();

                return;
            }
        }
    }

    public void StopSFX(string name)
    {
        for (int i = 0; i < SFXSource.Length; ++i)
        {
            if (SFXSource[i].isPlaying)
            {
                if (SFXSource[i].clip.name == name)
                {
                    SFXSource[i].Stop();
                }
            }
        }
    }




    private AudioSource GetEmptyAudioSource()
    {
        float LargeProgress = 0;
        int LargeIndex = 0;

        for (int i = 0; i < SFXSource.Length; ++i)
        {
            if (!SFXSource[i].isPlaying)
            {
                return SFXSource[i];
            }

            float Progress = SFXSource[i].time / SFXSource[i].clip.length;

            if (Progress > LargeProgress && !SFXSource[i].loop)
            {
                LargeProgress = Progress;
                LargeIndex = i;
            }
        }

        return SFXSource[LargeIndex];
    }
}
