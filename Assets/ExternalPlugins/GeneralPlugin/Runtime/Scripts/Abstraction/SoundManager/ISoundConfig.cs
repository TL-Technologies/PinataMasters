using UnityEngine;
using UnityEngine.Audio;


namespace Modules.General.Abstraction
{
    public interface ISoundConfig
    {
        AudioClip Clip { get; set; }
        AudioMixerGroup OutputGroup { get; set; }
        bool Loop { get; set; }
        int Priority { get; set; }
        float Volume { get; set; }
        float Pitch { get; set; }
        float StereoPan { get; set; }
        float SpatialBlend { get; set; }
        float DopplerLevel { get; set; }
        int Spread { get; set; }
        AudioRolloffMode RolloffMode { get; set; }
        float MinDistance { get; set; }
        float MaxDistance { get; set; }
        
        void Initialize(AudioClip currentClip);
    }
}
