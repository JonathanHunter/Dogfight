﻿using UnityEngine;
using System.Collections;
using UnityUtilLib;
using Danmaku2D;

namespace Danmaku2D.AttackPatterns {

	public abstract class Burst : AttackPattern {
		[SerializeField]
		private ProjectilePrefab prefab;
		
		[SerializeField]
		private Vector2 spawnLocation;
		
		[SerializeField]
		private Vector2 spawnArea;
		
		[SerializeField]
		private int bulletCount;

		protected abstract IProjectileGroupController BurstController {
			get;
		}
		
		[SerializeField]
		private Counter burstCount;
		
		[SerializeField]
		private FrameCounter burstDelay;
		
		[SerializeField]
		[Range(-180f, 180f)]
		private float burstInitialRotation;
		
		[SerializeField]
		[Range(-360f, 360f)]
		private float burstRotationDelta;
		
		private Vector2 currentBurstSource;
		private ProjectileGroup burstGroup;
		
		public override void Awake () {
			base.Awake ();
			burstGroup = new ProjectileGroup ();
			burstGroup.Controller = BurstController;
		}
		
		protected override bool IsFinished {
			get {
				return burstCount.Ready();
			}
		}
		
		protected override void OnExecutionStart () {
			burstCount.Reset ();
			currentBurstSource = spawnLocation - 0.5f * spawnArea + Util.RandomVect2 (spawnArea);
		}
		
		protected override void MainLoop () {
			if(burstDelay.Tick()) {
				float offset = (burstCount.MaxCount - burstCount.Count) * burstRotationDelta;
				for(int i = 0; i < bulletCount; i++) {
					Projectile temp = SpawnProjectile (prefab, currentBurstSource, offset + 360f / (float) bulletCount * (float)i);
					burstGroup.Add(temp);
				}
				burstCount.Tick();
			}
		}
	}
}
