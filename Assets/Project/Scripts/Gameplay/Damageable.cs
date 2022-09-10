using System;
using UnityEngine;

namespace Project.Gameplay
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 1;

        public float Health { get; private set; }
        public float MaxHealth => _maxHealth;
    
        public event Action<float> TookDamage;
        public event Action Died;
    
        private void Awake()
        {
            Health = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (Health <= 0) return;
        
            Health -= damage;
            TookDamage?.Invoke(damage);
            if (Health <= 0)
            {
                Died?.Invoke();
            }
        }

        public void Kill()
        {
            Health = 0;
            Died?.Invoke();
        }

        public void RestoreHealth()
        {
            Health = MaxHealth;
        }
    }
}
