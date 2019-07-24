using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    public AudioClip musicClip;

    [Range(0, 1)] public float musicVolume = 0.4f;
    [Range(0, 1)] public float fxVolume = 1f;

    // Random pitch adjustment range.
    [SerializeField] float lowPitchRange = .90f;
    [SerializeField] float highPitchRange = 1.10f;

    private void Start()
    {
        MusicSource.volume = musicVolume;
        EffectsSource.volume = fxVolume;
        PlayMusic(musicClip);
    }

    //// Play a single clip through the sound effects source.
    //public void Play(AudioClip clip)
    //{
    //    EffectsSource.clip = clip;
    //    EffectsSource.Play();
    //}

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    //// Play a random clip from an array, and randomize the pitch slightly.
    //public void RandomSoundEffect(params AudioClip[] clips)
    //{
    //    int randomIndex = Random.Range(0, clips.Length);
    //    float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

    //    EffectsSource.pitch = randomPitch;
    //    EffectsSource.clip = clips[randomIndex];
    //    EffectsSource.Play(); 
    //}

    public void PlayClip(AudioClip clip, float volume = 1f, bool useRandomPitch = true)
    {
        if (clip != null)
        {
            GameObject go = new GameObject("SoundFX " + clip.name);
            go.transform.position = transform.position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;

            if (useRandomPitch)
            {
                source.pitch = Random.Range(lowPitchRange, highPitchRange);
            }
            source.volume = volume;
            source.Play();
            Destroy(go, clip.length);
        }
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            GameObject go = new GameObject("SoundFX " + clip.name);
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;

            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            source.pitch = randomPitch;
            source.volume = volume;
            source.Play();
            Destroy(go, clip.length);

            return source;
        }

        return null;
    }

    public AudioSource PlayRandom(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips != null)
        {
            if (clips.Length != 0)
            {
                int randomIndex = Random.Range(0, clips.Length);

                if (clips[randomIndex] != null)
                {
                    return PlayClipAtPoint(clips[randomIndex], position, volume);
                }
            }
        }
        return null;
    }

}
