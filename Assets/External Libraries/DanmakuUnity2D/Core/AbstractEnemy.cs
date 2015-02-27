﻿using UnityEngine;
using UnityUtilLib;
using System.Collections;

namespace Danmaku2D {
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public abstract class AbstractEnemy : CachedObject {

		public virtual AbstractAttackPattern CurrentAttackPattern { 
			get {
				return null;
			}
		}

		private AbstractDanmakuField field;
		public AbstractDanmakuField Field {
			get {
				return field;
			}
			set {
				field = value;
			}
		}

		public abstract bool IsDead { get; }

		public virtual void Start() {
			EnemyManager.RegisterEnemy (this);
		}

		public void Hit(float damage) {
			Damage (damage);
			if(IsDead) {
				Die ();
			}
		}

		protected abstract void Damage (float damage);

		private void Die () {
			EnemyManager.UnregisterEnemy (this);
			Destroy (GameObject);
			OnDeath ();
		}

		protected virtual void OnDeath() {
		}

		protected virtual void OnDamage() {
		}

		void OnProjectileCollision(Projectile proj) {
			Hit (proj.Damage);
			proj.Deactivate();
		}
	}
}