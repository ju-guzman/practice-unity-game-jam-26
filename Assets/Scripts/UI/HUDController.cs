using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    /// <summary>
    /// Non-diegetic UI (HUD/dialogue/inventory) rendered via UI Toolkit.
    /// Deliberately has no reference to Light2D/Volume: lighting stays in GameJam.Lighting,
    /// communication happens only through GameJam.Core.GameEvents.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class HUDController : MonoBehaviour
    {
        private UIDocument _document;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }
    }
}
