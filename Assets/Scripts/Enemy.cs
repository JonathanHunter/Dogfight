﻿using UnityEngine;
using BaseLib;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(FieldMovementPattern))]
public class Enemy : CachedObject {

	[SerializeField]
	private float deathReflectRadius;

	[SerializeField]
	private float maxHealth;
	private float health;

	private AttackPattern attacks;
	private FieldMovementPattern fmp;

	void Start() {
		health = maxHealth;
		EnemyManager.RegisterEnemy (this);
		fmp = GetComponent<FieldMovementPattern> ();
		attacks = GetComponent<AttackPattern> ();
		if (attacks != null) {
			attacks.Initialize (fmp.field);
			attacks.Fire ();
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Player Shot")) {
			Projectile proj = other.GetComponent<Projectile>();
			if(proj != null) {
				Hit (proj);
			}
		}
	}

	public void Hit(Projectile proj) {
		health -= proj.Damage;
		proj.Active = false;
		if(health <= 0) {
			Die();
		}
	}

	private void Die() {
		float radius = Util.MaxComponent3 (Transform.lossyScale) * deathReflectRadius;
		//TODO: FINISH
		EnemyManager.UnregisterEnemy (this);
		Destroy (GameObject);
	}
}
