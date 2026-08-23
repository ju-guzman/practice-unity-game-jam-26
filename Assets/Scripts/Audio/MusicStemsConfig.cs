using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "MusicStemsConfig", menuName = "Game/Audio/Music Stems Config")]
    public class MusicStemsConfig : ScriptableObject
    {
        [Header("Stems")]
        public List<MusicStemDefinition> stems = new();

        [Header("Volume (0.5 -> 1 range)")]
        public float minVolume = 0.4f;
        public float maxVolume = 1f;

        [Header("Discomfort effect (0 -> 0.5 range)")]
        public float comfortableLowpassCutoff = 22000f;
        public float harshLowpassCutoff = 400f;

        public float comfortableDistortion;
        public float harshDistortion = 0.35f;

        public float comfortablePitch = 1f;
        public float harshPitch = 0.85f;

        [Header("Smoothing")]
        public float smoothing = 3f;

        public float GetActivationThreshold(int index)
        {
            var stem = stems[index];
            if (stem.activationThreshold > 0f)
            {
                return stem.activationThreshold;
            }

            return stems.Count <= 1 ? 0.5f : Mathf.Lerp(0.5f, 1f, (float)index / (stems.Count - 1));
        }
    }
}
