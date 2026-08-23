using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ScreenFader : MonoBehaviour
    {
        private VisualElement _document;

        private void Awake()
        {
            var document = GetComponent<UIDocument>();
            _document = document.rootVisualElement.Q<VisualElement>("FadeOverlay");
        }

        public void SetOpacity(float opacity)
        {
            if (_document == null)
            {
                return;
            }

            _document.style.opacity = opacity;
        }
    }
}