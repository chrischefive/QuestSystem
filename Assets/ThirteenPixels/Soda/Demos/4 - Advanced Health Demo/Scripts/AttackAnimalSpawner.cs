
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using System.Collections;

    public class AttackAnimalSpawner : MonoBehaviour
    {
        private readonly WaitForSeconds waitForOneSecond = new WaitForSeconds(1f);

        [SerializeField]
        private FarmAnimal animalPrefab = default;
        [Range(0.5f, 5f)]
        public float spawnRange = 1f;

        private void Start()
        {
            StartCoroutine(SpawnAnimals());
        }

        private IEnumerator SpawnAnimals()
        {
            while (enabled)
            {
                var instance = Instantiate(animalPrefab,
                                           transform.position + (Vector3)Random.insideUnitCircle.normalized * spawnRange,
                                           Quaternion.identity);
                instance.MoveTo(Vector3.zero);

                yield return waitForOneSecond;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRange);
        }
#endif
    }
}
