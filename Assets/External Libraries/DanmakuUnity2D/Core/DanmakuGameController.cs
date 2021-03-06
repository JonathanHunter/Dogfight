using UnityEngine;
using System.Collections.Generic;
using UnityUtilLib;
using UnityUtilLib.Pooling;

/// <summary>
/// A development kit for quick development of 2D Danmaku games
/// </summary>
namespace Danmaku2D {

	/// <summary>
	/// A GameController implementation for 2D Danmaku games.
	/// </summary>
//	[RequireComponent(typeof(EnemyManager))]
	public class DanmakuGameController : GameController {

		[SerializeField]
		private int maximumLives;

		[SerializeField]
		private int projectileInitialCount = 1000;
		
		[SerializeField]
		private int projectileSpawnOnEmpty = 100;

		/// <summary>
		/// The maximum number of lives a player can reach.
		/// </summary>
		/// <value>The maximum lives.</value>
		public static int MaximumLives {
			get {
				return (Instance as DanmakuGameController).maximumLives;
			}
		}

//		public abstract void SpawnEnemy (Enemy prefab, Vector2 relativeLocations);

		public override void Update() {
			base.Update ();
			if (!IsGamePaused) {
				Projectile.UpdateAll();
			}
		}

		public override void Awake () {
			base.Awake ();
			Projectile.Setup (projectileInitialCount, projectileSpawnOnEmpty);
		}
	}
}