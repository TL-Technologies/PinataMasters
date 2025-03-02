using Modules.General.HelperClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace PinataMasters
{

    public enum AudioType
    {
        Sound,
        PrioritySound,
        Music,
        Ambient
    }


    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {

        #region Variables

        private const string SOUND_KEY = "Sound";
        private const string MUSIC_KEY = "Music";

        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SOUNDS_VOLUME = "SoundsVolume";
        private const string PRIORITY_SOUNDS_VOLUME = "PrioritySoundsVolume";
        private const string AMBIENT_VOLUME = "AmbientVolume";

        [Header("Parameters")]
        [SerializeField]
        private float minTimeBetweenSameSounds = 0.04f;

        [Header("Mixer")]
        [SerializeField]
        private AudioMixer mixer = null;

        [Space]
        [SerializeField]
        private AudioMixerGroup soundsGroup = null;
        [SerializeField]
        private AudioMixerGroup prioritySoundsGroup = null;
        [SerializeField]
        private AudioMixerGroup musicGroup = null;
        [SerializeField]
        private AudioMixerGroup ambientGroup = null;

        [Space]
        [SerializeField]
        private AudioMixerSnapshot normalSnapshot = null;
        [SerializeField]
        private AudioMixerSnapshot muffleSnapshot = null;
        [SerializeField]
        private AudioMixerSnapshot demoSnapshot = null;
        [SerializeField]
        private float timeToReachSnapshot = 0f;

        [Header("Music")]
        [SerializeField]
        private AudioClip musicClip = null;

        private AudioSource musicSource;
        private AudioSource soundSource;
        private AudioSource prioritySoundSource;

        private List<AudioSource> ambientSources = new List<AudioSource>();
        private List<AudioClip> blackList = new List<AudioClip>();

        #endregion



        #region Properties

        public bool IsSoundEnable
        {
            get
            {
                return CustomPlayerPrefs.GetBool(SOUND_KEY, true);
            }
            set
            {
                CustomPlayerPrefs.SetBool(SOUND_KEY, value);
                RefreshSound();
            }
        }


        public bool IsMusicEnable
        {
            get
            {
                return CustomPlayerPrefs.GetBool(MUSIC_KEY, true);
            }
            set
            {
                CustomPlayerPrefs.SetBool(MUSIC_KEY, value);
                RefreshMusic();
            }
        }

        #endregion



        #region Public methods

        public void Play(AudioClip audioClip, AudioType type, float volume = 1f, float delay = 0f)
        {
            if (audioClip == null)
            {
                return;
            }

            for (int i = 0; i < blackList.Count; i++)
            {
                if (blackList[i] == audioClip)
                {
                    return;
                }
            }

            blackList.Add(audioClip);
            StartCoroutine(ClipCooldown(audioClip));

            AudioSource outputSource = null;

            switch (type)
            {
                case AudioType.Music:

                    if (IsMusicEnable)
                    {
                        if (musicSource == null)
                        {
                            musicSource = gameObject.AddComponent<AudioSource>();
                            musicSource.outputAudioMixerGroup = musicGroup;
                            musicSource.loop = true;
                        }

                        outputSource = musicSource;
                    }
                    break;


                case AudioType.Sound:

                    if (IsSoundEnable)
                    {
                        if (soundSource == null)
                        {
                            soundSource = gameObject.AddComponent<AudioSource>();
                            soundSource.outputAudioMixerGroup = soundsGroup;
                        }

                        outputSource = soundSource;
                    }
                    break;
                   

                case AudioType.PrioritySound:

                    if (IsSoundEnable)
                    {
                        if (prioritySoundSource == null)
                        {
                            prioritySoundSource = gameObject.AddComponent<AudioSource>();
                            prioritySoundSource.outputAudioMixerGroup = prioritySoundsGroup;
                        }

                        outputSource = prioritySoundSource;
                    }
                    break;


                case AudioType.Ambient:

                    if (IsSoundEnable)
                    {
                        outputSource = gameObject.AddComponent<AudioSource>();

                        ambientSources.Add(outputSource);
                        outputSource.outputAudioMixerGroup = ambientGroup;
                        outputSource.loop = true;
                        outputSource.clip = audioClip;
                    }
                    break;
            }

            if (Mathf.Approximately(delay, 0f))
            {
                PlayClip(type, outputSource, audioClip, volume);
            }
            else
            {
                StartCoroutine(PlayClip(type, outputSource, audioClip, volume, delay));
            }
        }


        public void StopAmbient(AudioClip audioClip)
        {

            for (int i = 0; i < ambientSources.Count; i++)
            {
                if (ambientSources[i].clip == audioClip)
                {
                    Destroy(ambientSources[i]);
                    ambientSources.RemoveAt(i);

                    break;
                }
            }
        }


        public void MuffleAudio()
        {
            muffleSnapshot.TransitionTo(timeToReachSnapshot);
        }


        public void NormalizeAudio()
        {
            normalSnapshot.TransitionTo(timeToReachSnapshot);
        }


        public void SetSnapshotForDemo()
        {
            demoSnapshot.TransitionTo(timeToReachSnapshot);
        }


        public void Mute(bool isMute)
        {
            AudioListener.pause = isMute;
        }


        public void Initialize()
        {
            RefreshSound();
            RefreshMusic();
        }

        #endregion



        #region Private methods

        private void RefreshSound()
        {
            mixer.SetFloat(SOUNDS_VOLUME, IsSoundEnable ? 0f : -80f);
            mixer.SetFloat(AMBIENT_VOLUME, IsSoundEnable ? 0f : -80f);
            mixer.SetFloat(PRIORITY_SOUNDS_VOLUME, IsSoundEnable ? 0f : -80f);
        }


        private void RefreshMusic()
        {
            if (IsMusicEnable)
            {
                Play(musicClip, AudioType.Music);
            }
            else
            {
                if (musicSource != null)
                {
                    musicSource.Pause();
                }
            }
        }


        private IEnumerator PlayClip(AudioType type, AudioSource audioSource, AudioClip audioClip, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);

            PlayClip(type, audioSource, audioClip, volume);
        }


        private void PlayClip(AudioType type, AudioSource audioSource, AudioClip audioClip, float volume)
        {
            if (audioSource != null && audioClip != null)
            {
                switch (type)
                {
                    case AudioType.Ambient:
                        audioSource.clip = audioClip;
                        audioSource.volume = volume;
                        audioSource.Play();
                        break;

                    case AudioType.Music:
                        audioSource.clip = audioClip;
                        audioSource.volume = volume;
                        audioSource.Play();
                        break;

                    case AudioType.Sound:
                        audioSource.PlayOneShot(audioClip, volume);
                        break;

                    case AudioType.PrioritySound:
                        audioSource.PlayOneShot(audioClip, volume);
                        break;
                }
            }
        }


        private IEnumerator ClipCooldown(AudioClip audioClip)
        {
            yield return new WaitForSeconds(minTimeBetweenSameSounds);

            blackList.Remove(audioClip);
        }
   
        #endregion
    }
}
