using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class MusicStemDefinition
    {
        public string label = "Stem";
        public AudioClip clip;
        public float activationThreshold;
    }
}
