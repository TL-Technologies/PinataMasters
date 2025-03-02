using System;
using UnityEngine;


namespace Modules.General.Abstraction
{
    public interface IAudioSourceWrapper
    {
        bool IsValid { get; }
        bool IsDone { get; }
        Action<IAudioSourceWrapper> FinishPlayCallback { get; }
        AudioSource Source { get; }
        float Time { get; }
        bool IsMuted { get; set; }
        float Volume { get; set; }
        bool Loop { get; set; }
    }
}
