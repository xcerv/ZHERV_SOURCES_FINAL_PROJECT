using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class sound_controller_generic : MonoBehaviour
{
    public AudioSource source_loop;
    public AudioSource source_once;

    // Start is called before the first frame update
    void Start()
    {
        /*
        if(source_loop != null)
        {
            source_loop.volume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
        }

        if(source_once != null)
        {
            source_once.volume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
        }
        */
    }

    public void PlaySound (AudioClip clip){
        source_once.loop = false;
        source_once.PlayOneShot(clip);
    }

    // Play random from list
    public void PlaySoundList (List<AudioClip> clips){
        source_once.loop = false;
        source_once.PlayOneShot(clips[Random.Range(0, clips.Count)]);
    }

    public void PlaySoundLoop (AudioClip clip, bool fadeIn){
        source_loop.loop = true;
        source_loop.clip = clip;

        if(!fadeIn){
            source_loop.Play();
        }
        else{
            StartCoroutine(FadeIn(source_loop, 5));
        }
    }

    public void StopLoopSound(bool fadeOut)
    {
        if(!fadeOut){
            source_loop.Stop();
        }
        else{
            StartCoroutine(FadeOut(source_loop, 3));
        }
    }

    IEnumerator FadeOut(AudioSource src, float fadeTime) 
    {
        float timeElapsed = 0;
        float volumeMax = src.volume;

        while (src.volume > 0) 
        {
            src.volume = Mathf.SmoothStep(volumeMax, 0, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        src.Stop();
    }

    IEnumerator FadeIn(AudioSource src, float fadeTime) 
    {
        float volumeMax = src.volume;
        yield return new WaitForSeconds(1);

        float timeElapsed = 0;
        src.volume = 0;
        src.Play();

        while (src.volume < volumeMax) 
        {
            src.volume = Mathf.SmoothStep(0, volumeMax, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        src.volume = volumeMax;
    }
}
