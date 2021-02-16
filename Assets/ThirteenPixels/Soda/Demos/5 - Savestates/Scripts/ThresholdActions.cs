
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using UnityEngine.Events;
    using Soda;

    /// <summary>
    /// Invokes events when the given value goes above (greater than or equal) or below (less than) the given threshold.
    /// 
    /// This class demonstrates one way of monitoring two values at the same time.
    /// </summary>
    public class ThresholdActions : MonoBehaviour
    {
        [SerializeField]
        private ScopedInt value = default;
        [SerializeField]
        private ScopedInt threshold = default;
        [Space]
        [Tooltip("Invoked when the value is greater or equal to the threshold value.")]
        [SerializeField]
        private UnityEvent onOverThreshold = default;
        [Tooltip("Invoked when the value is lower than the threshold value.")]
        [SerializeField]
        private UnityEvent onUnderThreshold = default;

        public bool valueIsOverThreshold
        {
            get { return value.value >= threshold.value; }
        }
        private bool valueWasOverThreshold = false;

        private void OnEnable()
        {
            value.onChangeValue.AddResponse(UpdateValueOrThreshold);
            threshold.onChangeValue.AddResponse(UpdateValueOrThreshold);
            
            UpdateValueOrThreshold(0);
        }

        private void OnDisable()
        {
            value.onChangeValue.RemoveResponse(UpdateValueOrThreshold);
            threshold.onChangeValue.RemoveResponse(UpdateValueOrThreshold);
        }

        private void UpdateValueOrThreshold(int value)
        {
            // In this method, we ignore the passed parameter as it comes from either the value or the threshold.
            // Instead of using it, we read the values directly from the InReferences.

            var overThreshold = valueIsOverThreshold;
            if (overThreshold && !valueWasOverThreshold)
            {
                onOverThreshold.Invoke();
            }
            else if (!overThreshold && valueWasOverThreshold)
            {
                onUnderThreshold.Invoke();
            }
            valueWasOverThreshold = overThreshold;
        }
    }
}