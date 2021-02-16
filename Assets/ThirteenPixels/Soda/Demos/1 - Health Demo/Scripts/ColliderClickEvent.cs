
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// A very simple component that invokes a UnityEvent if the collider of its GameObject was clicked on.
    /// </summary>
    public class ColliderClickEvent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onClick = default;
        
        private void OnMouseDown()
        {
            onClick.Invoke();
        }
    }
}