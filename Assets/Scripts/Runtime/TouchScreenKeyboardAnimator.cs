using System.Collections;
using UnityEngine;
using TMPro;

namespace Gemini
{
    /// <summary>
    /// Move arbitrary element up while keyboard is shown on mobile
    /// </summary>
    public sealed class TouchScreenKeyboardAnimator : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField targetInputField;
        [SerializeField]
        private RectTransform target;
        [SerializeField]
        private bool simulateOnEditor;
        [SerializeField]
        private AnimationCurve easing;

        private RectTransform rootTransform;
        private float currentHeight = 0;
        private Coroutine enableCoroutine;
        private Coroutine tweenCoroutine;

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
                Debug.Log($"IsFocused: {value}");

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

            rootTransform = target.transform.GetComponentInParent<Canvas>().rootCanvas.transform as RectTransform;

            targetInputField.shouldHideMobileInput = true;
            targetInputField.onSelect.AddListener(_ => IsFocused = true);
            targetInputField.onDeselect.AddListener(_ => IsFocused = false);

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
            // TouchScreenKeyboard.area returns zero-Rect on Android.
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

#pragma warning disable CS0162 // Unreachable code detected
            // Other supported environments
            {
                var area = TouchScreenKeyboard.area;
                return new Rect(area.x / w, area.y / h, area.width / w, area.height / h);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }


        private void UpdateKeyboardHeight(float relativeHeight)
        {
            float height = relativeHeight * rootTransform.sizeDelta.y;
            Debug.Log($"Translate Y: {height}");

            if (tweenCoroutine != null)
            {
                StopCoroutine(tweenCoroutine);
                tweenCoroutine = null;
            }
            float from = target.transform.localPosition.y;
            float to = height;
            float duration = Application.platform switch
            {
                RuntimePlatform.Android => 0.2f,
                RuntimePlatform.IPhonePlayer => 0.25f,
                _ => 0.2f,
            };
            tweenCoroutine = StartCoroutine(AnimatePosition(from, to, duration));
        }

        private IEnumerator DelayedEnable(bool enabled, float delaySec)
        {
            yield return new WaitForSeconds(delaySec);
            this.enabled = enabled;
        }

        private IEnumerator AnimatePosition(float from, float to, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = easing.Evaluate(elapsed / duration);
                target.transform.localPosition = new Vector3(0, Mathf.Lerp(from, to, t), 0);
                yield return new WaitForEndOfFrame();
            }
            target.transform.localPosition = new Vector3(0, to, 0);
        }
    }
}
