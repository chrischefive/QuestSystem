
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using UnityEngine.Events;
    using ThirteenPixels.Soda;

    /// <summary>
    /// This simple health component supports life points as well as extra lives.
    /// IMPORTANT NOTE: This class is meant for demonstration purposes only - it is not very cleverly designed.
    ///                 A proper Health class example can be found in the "Health Advanced" demo.
    /// </summary>
    public class SimplePlayerHealth : MonoBehaviour
    {
        private const int healthRefillPerLife = 100;

        [SerializeField]
        private GlobalInt health = default;
        [SerializeField]
        private GlobalInt extraLives = default;

        [Space]
        [SerializeField]
        private UnityEvent onTakeDamage = default;
        [SerializeField]
        private UnityEvent onLoseLife = default;
        [SerializeField]
        private UnityEvent onLoseAllLives = default;
        

        public void ApplyDamage(int amount)
        {
            if (amount <= 0) return;
            
            if (health.value > amount)
            {
                health.value -= amount;
                onTakeDamage.Invoke();
            }
            else
            {
                health.value = healthRefillPerLife;
                onTakeDamage.Invoke();

                extraLives.value--;
                onLoseLife.Invoke();

                if (extraLives.value <= 0)
                {
                    onLoseAllLives.Invoke();
                }
            }
        }
    }
}
