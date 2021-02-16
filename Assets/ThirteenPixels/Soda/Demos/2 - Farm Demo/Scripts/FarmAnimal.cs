
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// This component describes a FarmAnimal, which does two things:
    /// 1. Go somewhere within 3 meters around the world center when it spawns.
    /// 2. When told to, return to the farm it spawned at.
    /// </summary>
    public class FarmAnimal : MonoBehaviour
    {
        [System.NonSerialized]
        public Transform farm;
        [Range(1f, 10f)]
        public float speed = 1f;
        public bool moveToRandomPositionOnStart = true;

        private void Start()
        {
            if (moveToRandomPositionOnStart)
            {
                MoveTo(Random.insideUnitCircle * 3f);
            }
        }

        public void ReturnToFarm()
        {
            StopAllCoroutines();
            StartCoroutine(MoveToFarm());
        }

        public void MoveTo(Vector3 targetPosition)
        {
            StopAllCoroutines();
            StartCoroutine(MoveTo_Coroutine(targetPosition));
        }

        private IEnumerator MoveToFarm()
        {
            yield return MoveTo_Coroutine(farm.position);
            Destroy(gameObject);
        }

        private IEnumerator MoveTo_Coroutine(Vector3 targetPosition)
        {
            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
