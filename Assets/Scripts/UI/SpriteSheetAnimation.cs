using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [UxmlElement]
    public partial class SpriteSheetAnimation : VisualElement
    {
        // UxmlAttributes

        [UxmlAttribute("sprites")]
        public List<Sprite> Sprites
        {
            get => _sprites;
            set
            {
                _sprites = value ?? new List<Sprite>();
                CurrentFrame = 0;
                ApplyFrame();
                RefreshTimer();
            }
        }

        [UxmlAttribute("cycle-duration")]
        public float CycleDuration
        {
            get => _cycleDuration;
            set
            {
                _cycleDuration = Mathf.Max(0.001f, value);
                RefreshTimer();
            }
        }

        [UxmlAttribute("loop")]
        public bool Loop
        {
            get => _loop;
            set => _loop = value;
        }

        // Arranca la animación automáticamente al adjuntarse al panel.
        [UxmlAttribute("playing")]
        public bool Playing
        {
            get => _playing;
            set
            {
                _playing = value;
                RefreshTimer();
            }
        }

        // 0 = comportamiento original (cambio instantáneo entre frames).
        // >0 = duración en segundos del crossfade entre un frame y el siguiente durante la reproducción automática.
        [UxmlAttribute("smooth-transition-duration")]
        public float SmoothTransitionDuration
        {
            get => _smoothTransitionDuration;
            set => _smoothTransitionDuration = Mathf.Max(0f, value);
        }

        // Public state

        private int CurrentFrame { get; set; }

        public int FrameCount => _sprites.Count;

        // Events

        public event Action<int> OnFrameChanged;
        public event Action OnAnimationCompleted;

        // Private state

        private List<Sprite> _sprites = new();
        private float _cycleDuration = 1f;
        private bool _loop = true;
        private bool _playing = true;
        private float _smoothTransitionDuration;
        private IVisualElementScheduledItem _scheduledItem;

        private readonly VisualElement _transitionOverlay;
        private Sprite _appliedSprite;
        private IVisualElementScheduledItem _transitionScheduledItem;
        private long _transitionElapsedMs;
        private long _transitionDurationMs;
        private const long TransitionTickMs = 16;

        // Constructor

        public SpriteSheetAnimation()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            _transitionOverlay = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0,
                    right = 0,
                    bottom = 0,
                    display = DisplayStyle.None
                }
            };
            Add(_transitionOverlay);
        }

        // Public API

        public void Play()
        {
            CurrentFrame = 0;
            _playing = true;
            ApplyFrame();
            RefreshTimer();
        }

        public void Stop()
        {
            _playing = false;
            CurrentFrame = 0;
            StopTimer();
            ApplyFrame();
        }

        public void Pause()
        {
            _playing = false;
            StopTimer();
        }

        public void Resume()
        {
            _playing = true;
            RefreshTimer();
        }

        public void SetFrame(int index)
        {
            if (_sprites.Count == 0) return;
            CurrentFrame = Mathf.Clamp(index, 0, _sprites.Count - 1);
            ApplyFrame();
        }

        public void SetCycleDuration(float duration)
        {
            _cycleDuration = Mathf.Max(0.001f, duration);
            RefreshTimer();
        }

        public void SetSprites(List<Sprite> newSprites)
        {
            _sprites = newSprites ?? new List<Sprite>();
            CurrentFrame = Mathf.Clamp(CurrentFrame, 0, Mathf.Max(0, _sprites.Count - 1));
            ApplyFrame();
            RefreshTimer();
        }

        // Lifecycle

        private void OnAttachToPanel(AttachToPanelEvent _) => RefreshTimer();
        private void OnDetachFromPanel(DetachFromPanelEvent _) => StopTimer();

        // Timer

        private void RefreshTimer()
        {
            StopTimer();
            if (!_playing || panel == null || _sprites.Count == 0) return;

            long intervalMs = Mathf.Max(1, Mathf.RoundToInt(_cycleDuration * 1000f / _sprites.Count));
            // StartingIn is required here: Execute(...).Every(ms) alone runs the callback once
            // immediately, which would advance past frame 0 right away instead of holding it.
            _scheduledItem = schedule.Execute(AdvanceFrame).StartingIn(intervalMs).Every(intervalMs);
        }

        private void StopTimer()
        {
            _scheduledItem?.Pause();
            _scheduledItem = null;
        }

        // Animation

        private void AdvanceFrame()
        {
            if (_sprites.Count == 0) return;

            CurrentFrame++;

            if (CurrentFrame >= _sprites.Count)
            {
                if (_loop)
                {
                    CurrentFrame = 0;
                }
                else
                {
                    CurrentFrame = _sprites.Count - 1;
                    _playing = false;
                    StopTimer();
                    ApplyFrame();
                    OnAnimationCompleted?.Invoke();
                    return;
                }
            }

            if (_smoothTransitionDuration > 0f)
            {
                BeginTransition(_sprites[CurrentFrame]);
            }
            else
            {
                ApplyFrame();
            }

            OnFrameChanged?.Invoke(CurrentFrame);
        }

        private void ApplyFrame()
        {
            StopTransitionTimer();
            _transitionOverlay.style.display = DisplayStyle.None;

            if (_sprites.Count == 0 || _sprites[CurrentFrame] == null)
            {
                style.backgroundImage = StyleKeyword.None;
                _appliedSprite = null;
                return;
            }

            style.backgroundImage = new StyleBackground(_sprites[CurrentFrame]);
            _appliedSprite = _sprites[CurrentFrame];
        }

        // Smooth transition (crossfade)

        private void BeginTransition(Sprite newSprite)
        {
            StopTransitionTimer();

            if (_appliedSprite == null || newSprite == null)
            {
                style.backgroundImage = newSprite ? new StyleBackground(newSprite) : StyleKeyword.None;
                _appliedSprite = newSprite;
                return;
            }

            // Show the outgoing frame on the overlay, on top of the new frame already applied below,
            // then fade the overlay out - no native way to blend two background images otherwise.
            _transitionOverlay.style.backgroundImage = new StyleBackground(_appliedSprite);
            _transitionOverlay.style.opacity = 1f;
            _transitionOverlay.style.display = DisplayStyle.Flex;

            style.backgroundImage = new StyleBackground(newSprite);
            _appliedSprite = newSprite;

            _transitionElapsedMs = 0;
            _transitionDurationMs = Mathf.Max(1, Mathf.RoundToInt(_smoothTransitionDuration * 1000f));
            _transitionScheduledItem = schedule.Execute(TickTransition).Every(TransitionTickMs);
        }

        private void TickTransition()
        {
            _transitionElapsedMs += TransitionTickMs;
            var t = Mathf.Clamp01((float)_transitionElapsedMs / _transitionDurationMs);
            _transitionOverlay.style.opacity = 1f - t;

            if (t >= 1f)
            {
                StopTransitionTimer();
                _transitionOverlay.style.display = DisplayStyle.None;
            }
        }

        private void StopTransitionTimer()
        {
            _transitionScheduledItem?.Pause();
            _transitionScheduledItem = null;
        }
    }
}
