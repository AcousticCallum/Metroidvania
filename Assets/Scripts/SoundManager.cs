using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioMixer audioMixer;

    [SerializeField] private float lowpassCutoffFreq;

    [Space]

    public Sound[] sounds;
    public Sound[] music;
    public float blacklistClearCooldown;

    private List<string> blacklist = new List<string>();
    private float pitchMultiplier = 1.0f;

    private void Awake()
    {
        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.loop = s.loop;
            s.source.ignoreListenerPause = s.ignoreListenerPause;
            s.source.volume = s.volume;
        }

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.loop = s.loop;
            s.source.ignoreListenerPause = s.ignoreListenerPause;
            s.source.volume = s.volume;
        }

        StartCoroutine(BlacklistClearLoop());
    }

    private void Start()
    {
        //PlaySound("MainMusic");
    }

    private IEnumerator BlacklistClearLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(blacklistClearCooldown);

            blacklist.Clear();
        }
    }

    public void SetPitchMultiplier(float multiplier = 1.0f)
    {
        pitchMultiplier = multiplier;
    }

    public void PlaySound(string soundName, bool overrideBlacklist = false)
    {
        if (!overrideBlacklist && blacklist.Contains(soundName))
        {
            return;
        }

        blacklist.Add(soundName);

        foreach (Sound s in sounds)
        {
            if (s.name.Equals(soundName))
            {
            	s.source.pitch = (s.basePitch * pitchMultiplier) * (1.0f + Random.value * s.pitchVariation);
                s.source.volume = s.volume;// * PlayerData.instance.sfxVolume;
                s.source.Play();
                return;
            }
        }

        foreach (Sound s in music)
        {
            if (s.name.Equals(soundName))
            {
                s.source.volume = s.volume;// * PlayerData.instance.musicVolume;
                s.source.Play();
                return;
            }
        }

        Debug.Log("Couldn't find sound named '" + soundName + "'.");
    }

    public void SetAudioVolume()
    {
        /*if (UIManager.instance.masterVolumeSlider.value == UIManager.instance.masterVolumeSlider.minValue)
        {
            audioMixer.SetFloat("Master", -80f); // Volume off
        }
        else
        {
            audioMixer.SetFloat("Master", PlayerData.instance.masterVolume);
        }

        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * PlayerData.instance.sfxVolume;
        }

        foreach (Sound s in music)
        {
            s.source.volume = s.volume * PlayerData.instance.musicVolume;
        }*/
    }

    public void SetLowpass(bool enable = true)
    {
        if (!enable)
        {
            audioMixer.SetFloat("Lowpass", 22000);
            return;
        }

        audioMixer.SetFloat("Lowpass", lowpassCutoffFreq);
    }
}
