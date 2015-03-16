using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityUtilLib;
using UnityUtilLib.Pooling;

namespace Danmaku2D {

	public class ProjectileManager : Singleton<ProjectileManager>, IPausable {

		private static ProjectilePool projectilePool;

		private class ProjectilePool : IPool<Projectile> {

			private Queue<int> pool;
			internal Projectile[] all;
			private int totalCount;
			private int inactiveCount;
			private int spawnCount;

			public int TotalCount {
				get {
					return totalCount;
				}
			}

			public int ActiveCount {
				get {
					return totalCount - inactiveCount;
				}
			}

			public ProjectilePool(int initial, int spawn) {
				this.spawnCount = spawn;
				pool = new Queue<int>();
				totalCount = 0;
				inactiveCount = 0;
				Spawn (initial);
			}
			
			protected void Spawn(int count) {
				if(all == null) {
					all = new Projectile[1024];
				}
				int endCount = totalCount + spawnCount;
				if(all.Length <= endCount) {
					int arraySize = all.Length;
					while (arraySize <= endCount) {
						arraySize = Mathf.NextPowerOfTwo(arraySize + 1);
					}
					Projectile[] temp = new Projectile[arraySize];
					System.Array.Copy(all, temp, all.Length);
					all = temp;
				}
				for(int i = totalCount; i < endCount; i++) {
					all[i] = new Projectile();
					all[i].index = i;
					all[i].Pool = this;
					pool.Enqueue(i);
				}
				totalCount = endCount;
				inactiveCount += spawnCount;
			}

			#region IPool implementation
			public Projectile Get () {
				if(inactiveCount <= 0) {
					Spawn (spawnCount);
				}
				inactiveCount--;
				return all [pool.Dequeue ()];
			}
			public void Return (Projectile obj) {
				pool.Enqueue (obj.index);
				inactiveCount++;
			}
			#endregion
			#region IPool implementation
			object IPool.Get () {
				return Get ();
			}
			public void Return (object obj) {
				Return (obj as Projectile);
			}
			#endregion
		}

		[SerializeField]
		private int initialCount = 1000;

		[SerializeField]
		private int spawnOnEmpty = 100;

		public override void Awake () {
			base.Awake ();
			Projectile.SetupCollisions ();
		}

		public void Start () {
			if(projectilePool == null) {
				projectilePool = new ProjectilePool (initialCount, spawnOnEmpty);
			}
		}

		public int TotalCount {
			get {
				return (projectilePool != null) ? projectilePool.TotalCount : 0;
			}
		}

		public int ActiveCount {
			get {
				return (projectilePool != null) ? projectilePool.ActiveCount : 0;
			}
		}

		public void Update() {
			if (!Paused)
				NormalUpdate ();
		}

		public virtual void NormalUpdate () {
			float dt = Util.TargetDeltaTime;
			Projectile[] all = projectilePool.all;
			int totalCount = projectilePool.TotalCount;
			for (int i = 0; i < totalCount; i++) {
				if(all[i].IsActive)
					all[i].Update(dt);
			}
		}

		public static void DeactivateAll() {
			Projectile[] all = projectilePool.all;
			int totalCount = projectilePool.TotalCount;
			for (int i = 0; i < totalCount; i++) {
				if(all[i].IsActive)
					all[i].DeactivateImmediate();
			}
		}

		internal static Projectile Get (ProjectilePrefab projectileType) {
			Projectile proj = projectilePool.Get ();
			proj.MatchPrefab (projectileType);
			return proj;
		}

		#region IPausable implementation

		public bool Paused {
			get;
			set;
		}

		#endregion
	}
}