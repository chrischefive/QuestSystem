
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;

    public class DanceInstructor : MonoBehaviour
    {
        [Range(0f, 2f)]
        public float delay = 0.2f;
        [Range(1f, 8f)]
        public float duration = 2f;
        [Range(0.1f, 2f)]
        public float radius = 0.2f;

        [SerializeField]
        private GlobalDancer dancer = default;

        public void LetDancerDance()
        {
            if (dancer.value)
            {
                dancer.componentCache.Dance(delay, duration, radius);
            }
        }
    }
}
