﻿using UnityEngine;
using System.Collections;
using Danmaku2D;
using UnityUtilLib;

public class RussianWinter : TimedAttackPattern {

	[SerializeField]
	private ProjectilePrefab snowPrefab;

	[SerializeField]
	private FrameCounter delay;

	[SerializeField]
	private int spawnCount;

	[SerializeField]
	private Vector2 areaCenter;

	[SerializeField]
	private Vector2 areaSize;

	[SerializeField]
	private float angleMin;

	[SerializeField]
	private float angleMax;

	[SerializeField]
	private LinearProjectile linearController;

	private ProjectileGroup group;

	protected override void OnExecutionStart () {
		base.OnExecutionStart ();
		group = new ProjectileGroup ();
		group.Controller = linearController;
	}

	protected override void MainLoop () {
		base.MainLoop ();
		if(delay.Tick ()) {
			for(int i = 0; i < spawnCount; i++) {
				group.Add (SpawnProjectile(snowPrefab, areaCenter - areaSize / 2f + Util.RandomVect2(areaSize), Random.Range(angleMin, angleMax)));
			}
		}
	}
}
