using Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class MusicTensionController : MonoBehaviour
    {
        private const float LowRangeMax = 0.5f;

        public static MusicTensionController Instance { get; private set; }

        [SerializeField] private MusicStemsConfig config;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private string lowpassCutoffParam = "MusicLowpassCutoff";
        [SerializeField] private string distortionParam = "MusicDistortion";

        private static float Value => GameManager.Instance ? GameManager.Instance.Value : 1f;

        private AudioSource[] _stemSources;
        private float[] _stemTargetVolumes;
        private float _targetLowpassCutoff;
        private float _targetDistortion;
        private float _targetPitch;

        private void Awake()
        {
            Instance = this;
            CreateStemSources();
        }

        private void Start()
        {
            if (_stemSources.Length == 0)
            {
                return;
            }

            var startTime = AudioSettings.dspTime + 0.1;
            foreach (var source in _stemSources)
            {
                source.PlayScheduled(startTime);
            }
        }

        private void CreateStemSources()
        {
            var stemCount = config ? config.stems.Count : 0;
            _stemSources = new AudioSource[stemCount];
            _stemTargetVolumes = new float[stemCount];

            for (var i = 0; i < stemCount; i++)
            {
                var stem = config.stems[i];
                var stemObject = new GameObject($"Stem_{stem.label}");
                stemObject.transform.SetParent(transform);

                var source = stemObject.AddComponent<AudioSource>();
                source.clip = stem.clip;
                source.loop = true;
                source.playOnAwake = false;
                source.volume = 0f;
                source.outputAudioMixerGroup = musicMixerGroup;

                _stemSources[i] = source;
            }
        }

        private void Update()
        {
            if (!config || _stemSources.Length == 0)
            {
                return;
            }

            UpdateTargets();
            ApplySmoothing();
        }

        private void UpdateTargets()
        {
            if (Value >= LowRangeMax)
            {
                var t = (Value - LowRangeMax) / (1f - LowRangeMax);
                var masterVolume = Mathf.Lerp(config.minVolume, config.maxVolume, t);

                var activeCount = 0;
                for (var i = 0; i < _stemSources.Length; i++)
                {
                    if (Value >= config.GetActivationThreshold(i))
                    {
                        activeCount++;
                    }
                }

                var perStemVolume = masterVolume / Mathf.Sqrt(Mathf.Max(activeCount, 1));

                for (var i = 0; i < _stemSources.Length; i++)
                {
                    var isActive = Value >= config.GetActivationThreshold(i);
                    _stemTargetVolumes[i] = isActive ? perStemVolume : 0f;
                }

                _targetLowpassCutoff = config.comfortableLowpassCutoff;
                _targetDistortion = config.comfortableDistortion;
                _targetPitch = config.comfortablePitch;
            }
            else
            {
                var d = (LowRangeMax - Value) / LowRangeMax;

                for (var i = 0; i < _stemSources.Length; i++)
                {
                    _stemTargetVolumes[i] = i == 0 ? Mathf.Lerp(config.minVolume, 0f, d) : 0f;
                }

                _targetLowpassCutoff = Mathf.Lerp(config.comfortableLowpassCutoff, config.harshLowpassCutoff, d);
                _targetDistortion = Mathf.Lerp(config.comfortableDistortion, config.harshDistortion, d);
                _targetPitch = Mathf.Lerp(config.comfortablePitch, config.harshPitch, d);
            }
        }

        private void ApplySmoothing()
        {
            var step = config.smoothing * Time.deltaTime;

            var newPitch = Mathf.MoveTowards(_stemSources[0].pitch, _targetPitch, step);

            for (var i = 0; i < _stemSources.Length; i++)
            {
                var source = _stemSources[i];
                source.volume = Mathf.MoveTowards(source.volume, _stemTargetVolumes[i], step);
                source.pitch = newPitch;
            }

            if (!musicMixerGroup) return;
            var mixer = musicMixerGroup.audioMixer;
            mixer.GetFloat(lowpassCutoffParam, out var currentCutoff);
            mixer.GetFloat(distortionParam, out var currentDistortion);

            mixer.SetFloat(lowpassCutoffParam, Mathf.MoveTowards(currentCutoff, _targetLowpassCutoff, step * 1000f));
            mixer.SetFloat(distortionParam, Mathf.MoveTowards(currentDistortion, _targetDistortion, step));
        }
    }
}
