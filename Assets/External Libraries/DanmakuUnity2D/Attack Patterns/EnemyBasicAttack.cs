using UnityEngine;
using System.Collections;
using UnityUtilLib;

/// <summary>
/// A set of scripts for commonly created Attack Patterns
/// </summary>
namespace Danmaku2D.AttackPatterns {

	/// <summary>
	/// A basic enemy attack pattern. It fires a set of bullets in a fixed pattern at a fixed interval.
	/// </summary>
	[AddComponentMenu("Danmaku 2D/Attack Patterns/Enemy Basic Attack")]
	public class EnemyBasicAttack : AttackPattern {
		
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
		
		protected override void MainLoop () {
			if (fireDelay.Tick()) {
				float angle = TargetField.AngleTowardPlayer(transform.position) + Random.Range(-generalRange, generalRange);
				FireCurvedBullet(basicPrefab, transform.position, angle, velocity, angV, DanmakuField.CoordinateSystem.World);
			}
		}
	}
}