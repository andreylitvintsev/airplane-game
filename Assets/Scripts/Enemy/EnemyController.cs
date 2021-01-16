﻿using Common;
using Common.Pool;
using PathCreation.Examples;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(PathFollower)), DisallowMultipleComponent]
    public class EnemyController : MonoBehaviour, IRocketLauncherOwner
    {
        [SerializeField] private ColliderRuntimeSet _attackTargets;
        [SerializeField, Min(0f)] private float _reloadDelayInSeconds = 0f;
        [SerializeField] private GameObjectsPool _rocketsPool;
        
        private PathFollower _pathFollower;

        private RocketLauncher _rocketLauncher = null;

        private void Awake()
        {
            _rocketLauncher = new RocketLauncher(this);
        }

        private void Start()
        {
            if (_attackTargets == null) // TODO: сделать extension
            {
                Debug.LogError("'Attack Targets' must be not null!");
            }
            
            if (_rocketsPool == null)
            {
                Debug.LogError("'Rockets pool' must be not null!");
            }

            _pathFollower = GetComponent<PathFollower>();
        }

        private void Update()
        {
            _rocketLauncher.Update();
            TryFindAndAttack();
        }

        private void TryFindAndAttack()
        {
            var cachedTransform = transform;
            var raycastRay = new Ray(cachedTransform.position, cachedTransform.forward);
            foreach (var attackTarget in _attackTargets)
            {
                if (attackTarget.Raycast(raycastRay, out _, float.PositiveInfinity))
                {
                    _rocketLauncher.TryLaunchRocket();
                }
            }
        }

        public GameObjectsPool RocketsPool => _rocketsPool;

        public Component RocketLauncherOwner => this;

        public float Speed => _pathFollower.speed;

        public float ReloadDelayInSeconds => _reloadDelayInSeconds;
    }
}