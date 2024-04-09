using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GenerativeAI
{
    /// <summary>
    /// Move text field up while keyboard is shown on mobile
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
            Debug.Log($"Root: {rootElement}");

            var textField = rootElement.Q<TextField>(triggerTextField);
            targetElement = rootElement.Q<VisualElement>(targetElementName);
            targetElement ??= rootElement;

            textField.RegisterCallback<FocusInEvent>(_ => IsFocused = true);
            textField.RegisterCallback<FocusOutEvent>(_ => IsFocused = false);

            enabled = false;
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

            // Returns zero-Rect on Android.
            // https://forum.unity.com/threads/keyboard-height.291038/
#if UNITY_ANDROID
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
    
                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
    
                    return Screen.height - Rct.Call<int>("height");
                }
            }
#endif // UNITY_ANDROID

            // Other supported environments
            {
                float w = Screen.width;
                float h = Screen.height;
                var area = TouchScreenKeyboard.area;
                return new Rect(area.x / w, area.y / h, area.width / w, area.height / h);
            }
        }

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
