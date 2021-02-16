
namespace ThirteenPixels.SodaDemos
{
    using UnityEngine;
    
    public class ApplyDamageOnCollision : MonoBehaviour
    {
        [Range(1, 100)]
        public int damage = 10;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var hitHealth = collision.gameObject.GetComponent<Health>();
            if (hitHealth)
            {
                hitHealth.DecreaseHealth(damage);
            }
        }
    }
}
