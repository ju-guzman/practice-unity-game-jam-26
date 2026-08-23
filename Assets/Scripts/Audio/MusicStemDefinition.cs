using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class MusicStemDefinition
    {
        public string label = "Stem";
        public AudioClip clip;

        [Tooltip("Value (0.5-1) at which this stem fades in. Leave at 0 to auto-distribute evenly across the stem list.")]
        public float activationThreshold;
    }
}
