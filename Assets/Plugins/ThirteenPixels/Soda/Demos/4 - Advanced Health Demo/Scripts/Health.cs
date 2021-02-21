
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    using UnityEngine.Events;
    using ThirteenPixels.Soda;

    [AddComponentMenu("Soda/Demos/Health")]
    public class Health : MonoBehaviour
    {
        [System.Serializable]
        public class IntEvent : UnityEvent<int> { }

        [System.Serializable]
        public struct Events
        {
#pragma warning disable 649
            [SerializeField]
            private IntEvent _onChangeHealth;
            public IntEvent onChangeHealth
            {
                get { return _onChangeHealth; }
            }

            [SerializeField]
            private UnityEvent _onDeath;
            public UnityEvent onDeath
            {
                get { return _onDeath; }
            }
#pragma warning restore 649
        }

        private bool alive = true;
        [Tooltip("The amount of health this entity currently has.")]
        [SerializeField]
        private ScopedInt health = default;
        [Tooltip("The amount of health this entity starts with.\n0 = whatever value \"health\" has on start.")]
        [SerializeField]
        private ScopedInt startHealth = default;
        [Tooltip("The maximum amount of health this entity can have.")]
        [SerializeField]
        private ScopedInt maxHealth = default;
        [SerializeField]
        private bool destroyOnDeath = false;

        public Events events;


        private void Reset()
        {
            health = new ScopedInt(100);
            startHealth = new ScopedInt(100);
            maxHealth = new ScopedInt(100);
        }

        private void Start()
        {
            if (startHealth.value != 0)
            {
                health.value = startHealth.value;
            }
        }

        public void DecreaseHealth(int amount)
        {
            if (!alive || amount <= 0) return;

            health.value -= amount;
            if (health.value <= 0)
            {
                health.value = 0;
                events.onChangeHealth.Invoke(health.value);
                Die();
            }
            else
            {
                events.onChangeHealth.Invoke(health.value);
            }
        }

        public void IncreaseHealth(int amount, bool canRevive = false)
        {
            var cannotRevive = !alive && !canRevive;
            var negativeAmount = amount <= 0;
            var alreadyMax = health == maxHealth;
            if (cannotRevive || negativeAmount || alreadyMax) return;

            health.value += amount;
            if (health.value >= maxHealth.value)
            {
                health.value = maxHealth.value;
            }
            events.onChangeHealth.Invoke(health.value);
        }

        public void Die()
        {
            alive = false;
            events.onDeath.Invoke();
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
