using System;
using UnityEngine;


namespace Modules.General.Abstraction
{
    public interface ISoundManager
    {
        bool IsSoundEnabled { get; set; }
        bool IsMusicEnabled { get; set; }
        bool IsMutedGlobally { set; }
        
        void AddSound(string key, ISoundConfig config, bool replaceIfExists = false, int limiter = -1);
        void AddSound(string key, AudioClip clip, bool replaceIfExists = false, int limiter = -1);
        bool ContainsSound(string key);

        IAudioSourceWrapper InstantiateSound(string key, Vector3 position, Action<IAudioSourceWrapper> finishPlayCallback = null);
        IAudioSourceWrapper Play(string key, Action<IAudioSourceWrapper> finishPlayCallback = null);
        IAudioSourceWrapper AddSoundAndPlay(string key, AudioClip soundClip, Action<IAudioSourceWrapper> finishPlayCallback = null);
        IAudioSourceWrapper Play(string key, Vector3 position, Action<IAudioSourceWrapper> finishPlayCallback = null);
        void PlaySoundWithFade(IAudioSourceWrapper wrapper, float targetVolume, float duration, Action<IAudioSourceWrapper> finishPlayCallback = null);
        void StopSoundWithFade(IAudioSourceWrapper wrapper, float duration, Action callbackSoundFinishFadeOut);
        void Play(IAudioSourceWrapper wrapper);
        void Pause(IAudioSourceWrapper wrapper, string reason = null);
        void PauseAll(string reason = null);
        void Unpause(IAudioSourceWrapper wrapper, string reason = null);
        void UnpauseAll(string reason = null);
        void Mute(IAudioSourceWrapper wrapper, string reason = null);
        void MuteAll(string reason = null);
        void Unmute(IAudioSourceWrapper wrapper, string reason = null);
        void UnmuteAll(string reason = null);
        bool Stop(IAudioSourceWrapper wrapper);
        void StopAll();
        void SetMuted(IAudioSourceWrapper wrapper, bool isMuted, string reason = null);
        void SetMutedAll(bool isMuted, string reason = null);
        void ToggleSoundAvailability(bool isEnabled);
        void SetBlocked(bool isBlocked, string reason);
    }
}
