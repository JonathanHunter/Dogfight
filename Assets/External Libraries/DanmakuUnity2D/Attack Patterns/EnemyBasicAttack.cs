using UnityEngine;
using System.Collections;
using UnityUtilLib;

namespace Danmaku2D.AttackPattern {
	public class EnemyBasicAttack : AbstractAttackPattern {
		
		[SerializeField]
		private FrameCounter fireDelay;

		[SerializeField]
		private float velocity;

		[SerializeField]
		public float angV;

		[SerializeField]
		private float currentDelay;

		[SerializeField]
		private float generalRange;

		[SerializeField]
		private ProjectilePrefab basicPrefab;

		protected override bool IsFinished {
			get {
				return false;
			}
		}
		
		protected override void MainLoop (AttackPatternExecution execution) {
			if (fireDelay.Tick()) {
				float angle = TargetField.AngleTowardPlayer(transform.position) + Random.Range(-generalRange, generalRange);
				Projectile proj = TargetField.SpawnProjectile(basicPrefab, Transform.position,
				                            angle, 
				                            FieldCoordinateSystem.AbsoluteWorld);
				proj.Velocity = velocity;
				proj.AngularVelocity = angV;
			}
		}
	}
}