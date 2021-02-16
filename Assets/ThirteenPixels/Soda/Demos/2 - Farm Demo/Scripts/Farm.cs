
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;

    /// <summary>
    /// This component describes a place to spawn FarmAnimals, which will remember it in order to eventually return to it.
    /// </summary>
    public class Farm : MonoBehaviour
    {
        public FarmAnimal animalPrefab;

        public void SpawnAnimals(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var animal = Instantiate(animalPrefab, transform.position, Quaternion.identity);
                animal.farm = transform;
            }
        }

        public void SpawnRandomAmountOfAnimals()
        {
            SpawnAnimals(Random.Range(10, 20 + 1));
        }
    }
}
