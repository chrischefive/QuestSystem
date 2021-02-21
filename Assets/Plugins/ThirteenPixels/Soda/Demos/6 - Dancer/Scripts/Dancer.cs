
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Shakes around and calls it a "dance"!
    /// Note how the "Dance" method has multiple parameters, which means that a call is too complex for a GameEvent.
    /// For this reason, we instead use a GlobalGameObjectWithComponentCacheBase to reference a dancer GameObject
    /// including a chached reference to this component.
    /// </summary>
    public class Dancer : MonoBehaviour
    {
        public void Dance(float delay, float duration, float radius)
        {
            StopAllCoroutines();
            StartCoroutine(DanceCoroutine(delay, duration, radius));
        }

        private IEnumerator DanceCoroutine(float delay, float duration, float radius)
        {
            yield return new WaitForSeconds(delay);

            for (var time = 0f; time < duration; time += Time.deltaTime)
            {
                transform.localPosition = Random.insideUnitCircle * radius;
                yield return null;
            }
        }
    }
}
