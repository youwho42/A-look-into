using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundSet
{
    
    public AudioClip[] clips;

    [Range(0.0f, 1.0f)]
    public float volume = 0.7f;

    [Range(0.1f, 3.0f)]
    public float pitch = 1.0f;

    [Range(0.0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0.0f, 0.5f)]
    public float randomPitch = 0.1f;

    public AudioSource source;
    public bool overlap;

    public void SetSource(AudioSource _source, int randomClip)
    {
        source = _source;
        source.clip = clips[randomClip];
        source.playOnAwake = false;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2, randomVolume / 2));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2, randomPitch / 2));
        if (!source.isPlaying && !overlap)
            source.Play();
        if (overlap)
            source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}








public class AnimalSounds : MonoBehaviour
{

    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;
    public AudioMixerGroup mixerGroup;
    float cawTimer;
    public Vector2 minMaxTimesToCry;
    int timesToCaw;

    public bool isCrying;
    public Vector2 minMaxBetweenCries;
    public bool continuous;
    public bool mute;
    public float minSpeachPause;
    public float maxSpeachPause;

    public AnimationCurve curve;
    public Animator animator;
    int caw_hash = Animator.StringToHash("Caw");
    float mainVolume;

    int lastCryIndex;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        SetTimesToCry();
        SetFirstCry();
        if (continuous)
        {
            source.loop = true;
            SetContinuous();
        }
        else
            source.loop = false;

    }
    
   
    private void Update()
    {
        ChangeVolume();
        if (continuous)
        {
            SetContinuous();
            return;
        }
           
        cawTimer -= Time.deltaTime;
        if(cawTimer <= 0 && !isCrying && !mute)
        {
            SetTimesToCry();
            SetCrySounds();
            isCrying = true;
        }
        
    }
    void ChangeVolume()
    {
        int m = 1;
        if (mute)
            m = 0;
        source.volume = mainVolume * m;
    }
    void SetContinuous()
    {
        int r = 0;
        if (soundSets.Length > 1)
            r = Random.Range(0, soundSets.Length);

        int t = Random.Range(0, soundSets[r].clips.Length);

        PlayContinuousSound(r,t);
    }

    public void SetCrySounds()
    {
        int r = 0;
        if (soundSets.Length > 1)
            r = Random.Range(0, soundSets.Length);

        StartCoroutine(PlaySoundsCo(r, timesToCaw));
    }

    IEnumerator PlaySoundsCo(int AudioSet, int timesToCry)
    {
        int t = 0;
        while (t < timesToCry)
        {
            if (PlaySound(AudioSet))
                t++;
                
            yield return new WaitForSeconds(Random.Range(minSpeachPause, maxSpeachPause));
        }
        isCrying = false;
        SetNextCry();
        
    }

    void SetFirstCry()
    {
        cawTimer = Random.Range(minMaxBetweenCries.y/2, minMaxBetweenCries.y);
    }

    void SetNextCry()
    {
        cawTimer = (curve.Evaluate(Random.Range(0.0f, 1.0f)) * (minMaxBetweenCries.y - minMaxBetweenCries.x)) + minMaxBetweenCries.x;
    }
    void SetTimesToCry()
    {
        var t = (curve.Evaluate(Random.Range(0.0f, 1.0f))) * (minMaxTimesToCry.y - minMaxTimesToCry.x) + minMaxTimesToCry.x;
        
        timesToCaw = (int)t;
    }

    bool PlaySound(int soundSet)
    {
        if (!source.isPlaying)
        {
            int t = 0;
            do
            {
                t = Random.Range(0, soundSets[soundSet].clips.Length);
            }
            while (t == lastCryIndex);
            lastCryIndex = t;
            
            soundSets[soundSet].SetSource(source, t);
            soundSets[soundSet].source.outputAudioMixerGroup = mixerGroup;
            mainVolume = soundSets[soundSet].volume;
            soundSets[soundSet].Play();
            animator.SetTrigger(caw_hash);
            
            
            return true;
        }
        return false;
    }

    void PlayContinuousSound(int soundSet, int index)
    {
        if (!source.isPlaying)
        {
            
            soundSets[soundSet].SetSource(source, index);
            mainVolume = soundSets[soundSet].volume;
            soundSets[soundSet].Play();
            
        }
        
    }
}


