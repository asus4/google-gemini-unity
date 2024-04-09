using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GenerativeAI
{
    /// <summary>
    /// Move arbitrary element up while keyboard is shown on mobile
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class TouchScreenKeyboardAnimator : MonoBehaviour
    {
        [SerializeField]
        private string triggerTextField;
        [SerializeField]
        private string targetElementName;
        [SerializeField]
        private bool simulateOnEditor;

        private VisualElement rootElement;
        private VisualElement targetElement;
        private float currentHeight = 0;
        private Coroutine enableCoroutine;

#if UNITY_ANDROID
        private AndroidJavaObject androidView;
        private AndroidJavaObject androidRect;
#endif // UNITY_ANDROID


        private bool _isFocused;
        private bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (_isFocused == value)
                {
                    return;
                }
                _isFocused = value;

                // Delay enable when focused==false while keyboard is running
                if (enableCoroutine != null)
                {
                    StopCoroutine(enableCoroutine);
                    enableCoroutine = null;
                }
                if (value)
                {
                    enabled = value;
                }
                else
                {
                    const float KEYBOARD_ANIMATION_DURATION = 0.5f;
                    enableCoroutine = StartCoroutine(DelayedEnable(value, KEYBOARD_ANIMATION_DURATION));
                }
            }
        }

        private bool NeedAnimate => (Application.isEditor && simulateOnEditor) || TouchScreenKeyboard.isSupported;

        private void Start()
        {
            if (!NeedAnimate)
            {
                // Nothing to do
                enabled = false;
                return;
            }

            if (!TryGetComponent<UIDocument>(out var document))
            {
                Debug.LogError("UIDocument not found");
                return;
            }
            rootElement = document.rootVisualElement;

            var textField = rootElement.Q<TextField>(triggerTextField);
            targetElement = rootElement.Q<VisualElement>(targetElementName);
            targetElement ??= rootElement;

            textField.RegisterCallback<FocusInEvent>(_ => IsFocused = true);
            textField.RegisterCallback<FocusOutEvent>(_ => IsFocused = false);

            enabled = false;
        }

        private void OnDestroy()
        {
            if (enableCoroutine != null)
            {
                StopCoroutine(enableCoroutine);
                enableCoroutine = null;
            }

#if UNITY_ANDROID
            androidView?.Dispose();
            androidRect?.Dispose();
#endif // UNITY_ANDROID
        }

        private void Update()
        {
            float height = GetKeyboardArea().height;
            if (currentHeight != height)
            {
                UpdateKeyboardHeight(height);
                currentHeight = height;
            }
        }

#pragma warning disable CS0162 // Unreachable code detected
        /// <summary>
        /// Get keyboard area in normalized coordinates
        /// </summary>
        /// <returns>*Normalized* Rect</returns>
        private Rect GetKeyboardArea()
        {
            // Simulator on Editor
            if (!TouchScreenKeyboard.isSupported)
            {
                // Return dummy area for simulation
                return IsFocused
                    ? new Rect(0, 0.6f, 1, 0.4f)
                    : Rect.zero;
            }

            float w = Screen.width;
            float h = Screen.height;

#if UNITY_ANDROID
            // Returns zero-Rect on Android.
            // https://forum.unity.com/threads/keyboard-height.291038/
            if (androidView == null)
            {
                using AndroidJavaClass playerClass = new("com.unity3d.player.UnityPlayer");
                using AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                using AndroidJavaObject unityPlayer = activity.Get<AndroidJavaObject>("mUnityPlayer");
                androidView = unityPlayer.Call<AndroidJavaObject>("getView");
                androidRect = new AndroidJavaObject("android.graphics.Rect");
            }
            // TODO: support landscape mode
            androidView.Call("getWindowVisibleDisplayFrame", androidRect);
            float height = (Screen.height - androidRect.Call<int>("height")) / h;
            return new Rect(0, 1f - height, 1f, height);
#endif // UNITY_ANDROID

            // Other supported environments
            {
                var area = TouchScreenKeyboard.area;
                return new Rect(area.x / w, area.y / h, area.width / w, area.height / h);
            }
        }
#pragma warning restore CS0162 // Unreachable code detected


        private void UpdateKeyboardHeight(float height)
        {
            height *= rootElement.layout.height;
            Debug.Log($"Translate Y: {height}");
            targetElement.style.translate = new(new Translate(0, -height));
        }

        private IEnumerator DelayedEnable(bool enabled, float delaySec)
        {
            yield return new WaitForSeconds(delaySec);
            this.enabled = enabled;
        }

    }
}
