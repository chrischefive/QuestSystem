
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using ThirteenPixels.Soda;

    [CreateAssetMenu(menuName = "Soda/Demos/Global Dancer")]
    public class GlobalDancer : GlobalGameObjectWithComponentCacheBase<Dancer>
    {
        protected override bool TryCreateComponentCache(GameObject gameObject, out Dancer componentCache)
        {
            componentCache = gameObject.GetComponent<Dancer>();
            return componentCache != null;
        }
    }
}
