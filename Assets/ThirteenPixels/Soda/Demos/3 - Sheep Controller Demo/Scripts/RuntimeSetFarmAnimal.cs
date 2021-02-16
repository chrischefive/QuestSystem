
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using ThirteenPixels.Soda;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// Stores the transform components of the GameObjects added to it.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/Demos/FarmAnimal Runtime Set", order = 100)]
    public class RuntimeSetFarmAnimal : RuntimeSetBase<FarmAnimal>
    {
        protected override bool TryCreateElement(GameObject go, out FarmAnimal element)
        {
            element = go.GetComponent<FarmAnimal>();
            return element != null;
        }
    }
}
