﻿using Common;
using Common.Pool;
using PathCreation.Examples;
using UnityEngine;

namespace Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PathFollower))]
    public class EnemyController : MonoBehaviour, IRocketLauncherOwner, IDamagable
    {
        [SerializeField] private ColliderRuntimeSet _attackTargets;
        [SerializeField, Min(0f)] private float _reloadDelayInSeconds = 0f;
        [SerializeField] private GameObjectsPool _rocketsPool;
        [SerializeField] private Animator _animator;
        
        private PathFollower _pathFollower;
        private AnimationEventReceiver _animationEventReceiver;

        private RocketLauncher _rocketLauncher = null;
        private static readonly int IsDestroyed = Animator.StringToHash("IsDestroyed");

        public bool Killed { get; private set; } = false;
        
        private void Awake()
        {
            _rocketLauncher = new RocketLauncher(this);
        }

        private void Start() // TODO: может awake?
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
            _animationEventReceiver = GetComponent<AnimationEventReceiver>();
            // _animationEventReceiver.OnEvent += OnAnimationEvent;
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
        
        public void Damage()
        {
            if (Killed) return;
            Killed = true;
            _animator.SetBool(IsDestroyed, true);
        }
        
        // called from Animation receiver
        public void OnDestroyAnimationEnd()
        {
            gameObject.SetActive(false);
        }


        public GameObjectsPool RocketsPool => _rocketsPool;

        public Component RocketLauncherOwner => this;

        public float Speed => _pathFollower.speed;

        public float ReloadDelayInSeconds => _reloadDelayInSeconds;
    }
}