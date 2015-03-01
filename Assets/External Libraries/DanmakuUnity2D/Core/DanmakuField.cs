using UnityEngine;
using System.Collections;
using UnityUtilLib;
using System.Collections.Generic;

namespace Danmaku2D {

	public abstract class DanmakuField : PausableGameObject {

		public enum CoordinateSystem { View, Relative, World }
		[SerializeField]
		private Vector2 playerSpawnLocation = new Vector2(0.5f, 0.25f);

		public abstract DanmakuField TargetField { get; }

		private DanmakuGameController gameController;
		public DanmakuGameController GameController {
			get {
				if(gameController == null) {
					gameController = FindObjectOfType<DanmakuGameController>();
				}
				return gameController;
			}
		}

		private AbstractPlayableCharacter player;
		public AbstractPlayableCharacter Player {
			get {
				return player;
			}
		}

		public ProjectileManager BulletPool {
			get {
				return GameController.BulletPool;
			}
		}

		[SerializeField]
		private Camera fieldCamera;
		
		private Transform cameraTransform;

		public Transform CameraTransform {
			get {
				return cameraTransform;
			}
			set {
				cameraTransform = value;
			}
		}

		public void RecomputeWorldPoints() {
			Vector3 UL, BR;
			bottomLeft = fieldCamera.ViewportToWorldPoint (new Vector3(0f, 0f, gamePlaneDistance));
			UL = fieldCamera.ViewportToWorldPoint (Vector3.up);
			BR = fieldCamera.ViewportToWorldPoint (Vector3.right);
			x = (BR - bottomLeft);
			y = (UL - bottomLeft);
			z = Transform.forward * gamePlaneDistance;

			scale = new Vector3 (x.magnitude, y.magnitude, gamePlaneDistance);
			matW2F.SetTRS (bottomLeft, Quaternion.identity, scale);
			matF2W = matW2F.inverse;
		}
	
		private Vector3 x, y, z, scale, bottomLeft;
		private Matrix4x4 matW2F, matF2W;

		public float XSize {
			get { return scale.x; }
		}

		public float YSize {
			get { return scale.y; }
		}

		public Vector3 BottomLeft {
			get { return WorldPoint (new Vector3(0f, 0f, 0f)); }
		}

		public Vector3 BottomRight {
			get { return WorldPoint (new Vector3(1f, 0f, 0f)); }
		}

		public Vector3 TopLeft {
			get { return WorldPoint (new Vector3(0f, 1f, 0f)); }
		}

		public Vector3 TopRight {
			get { return WorldPoint (new Vector3(1f, 1f, 0f)); }
		}

		public Vector3 Center {
			get { return WorldPoint (new Vector3(0.5f, 0.5f, 0f)); }
		}

		public Vector3 Top {
			get { return WorldPoint (new Vector3 (0.5f, 1f, 0f)); }
		}

		public Vector3 Bottom {
			get { return WorldPoint (new Vector3 (0.5f, 0f, 0f));}
		}

		public Vector3 Right {
			get { return WorldPoint (new Vector3 (1f, 0.5f, 0f)); }
		}

		public Vector3 Left {
			get { return WorldPoint (new Vector3 (0f, 0.5f, 0f));}
		}

		public override void Awake () {
			base.Awake ();
			fieldCamera.orthographic = true;
			cameraTransform = fieldCamera.transform;
			RecomputeWorldPoints ();
		}

		[SerializeField]
		private float gamePlaneDistance = 10;

		public Vector3 WorldPoint(Vector3 point, CoordinateSystem coordSys = CoordinateSystem.View) {
			switch (coordSys) {
				case CoordinateSystem.World:
					return point;
				case CoordinateSystem.Relative:
					return bottomLeft + point;
				default:
				case CoordinateSystem.View:
					return bottomLeft + point.x * x + point.y * y + point.z * z;
			}
		}

		public Vector3 RelativePoint(Vector3 point, CoordinateSystem coordSys = CoordinateSystem.View) {
			switch (coordSys) {
				case CoordinateSystem.World:
					return point - bottomLeft;
				case CoordinateSystem.Relative:
					return point;
				default:
				case CoordinateSystem.View:
					return point.x * x + point.y * y + point.z * z;
			}
		}

		public Vector3 ViewPoint(Vector3 point, CoordinateSystem coordSys = CoordinateSystem.World) {
			switch (coordSys) {
				default:
				case CoordinateSystem.World:
					Vector3 offset = point - bottomLeft;
					return new Vector3(
						Vector3.Project (offset, x).magnitude / scale.x,
						Vector3.Project (offset, y).magnitude / scale.y,
						Vector3.Project (offset, z).magnitude / scale.z);
				case CoordinateSystem.Relative:
					return new Vector3(
						Vector3.Project (point, x).magnitude / scale.x,
						Vector3.Project (point, y).magnitude / scale.y,
						Vector3.Project (point, z).magnitude / scale.z);
				case CoordinateSystem.View:
					return point;
			}
		}

		public int LivesRemaining {
			get {
				if(player != null) {
					return player.LivesRemaining;
				} else {
					Debug.Log("Player Field without Player");
					return int.MinValue;
				}
			}
		}
		
		public Vector3 PlayerPosition {
			get {
				if(player != null) {
					return player.Transform.position;
				} else {
					Debug.Log("Player Field without Player");
					return Vector3.zero;
				}
			}
		}

		public float AngleTowardPlayer(Vector2 startLocation, CoordinateSystem coordinateSystem = CoordinateSystem.Relative) {
			return Util.AngleBetween2D (startLocation, PlayerPosition);
		}

		/// <summary>
		/// Spawns the player with the given controller
		/// </summary>
		/// <param name="character">Character prefab, defines character behavior and attack patterns.</param>
		/// <param name="controller">Controller for the player, allows for a user to manually control it or let an AI take over.</param>
		public AbstractPlayableCharacter SpawnPlayer(AbstractPlayableCharacter playerCharacter, CoordinateSystem coordSys = CoordinateSystem.View) {
			Vector3 spawnPos = WorldPoint(Util.To3D(playerSpawnLocation), coordSys);
			player =  (AbstractPlayableCharacter) Instantiate(playerCharacter, spawnPos, Quaternion.identity);
			if(player != null) {
				player.Reset (1);
				player.Transform.parent = Transform;
				player.Field = this;
			}
			return player;
		}
		
		/// <summary>
		/// Spawns a projectile in the field.
		/// 
		/// If absoluteWorldCoord is set to false, location specifies a relative position in the field. 0.0 = left/bottom, 1.0 = right/top. Values greater than 1 or less than 0 spawn
		/// outside of of the camera view.
		/// </summary>
		/// <param name="prefab">Prefab for the spawned projectile, describes the visuals, size, and hitbox characteristics of the prefab.</param>
		/// <param name="location">The location within the field to spawn the projectile.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="absoluteWorldCoord">If set to <c>true</c>, <c>location</c> is in absolute world coordinates relative to the bottom right corner of the game plane.</param>
		public Projectile SpawnProjectile(ProjectilePrefab prefab, Vector2 location, float rotation, CoordinateSystem coordSys = CoordinateSystem.View) {
			Vector3 worldLocation = Vector3.zero;
			worldLocation = WorldPoint (location, coordSys);
			Projectile projectile = ProjectileManager.Get (prefab);
			projectile.Transform.position = worldLocation;
			projectile.Transform.rotation = Quaternion.Euler(0f, 0f, rotation);
			projectile.Activate ();
			return projectile;
		}

		public LinearProjectile FireLinearBullet(ProjectilePrefab bulletType, 
		                                         Vector2 location, 
		                                         float rotation, 
		                                         float velocity,
		                                         DanmakuField.CoordinateSystem coordSys = DanmakuField.CoordinateSystem.View) {
			return ProjectileManager.FireLinearBullet (bulletType, WorldPoint (Util.To3D (location), coordSys), rotation, velocity);
		}
		
		public CurvedProjectile FireCurvedBullet(ProjectilePrefab bulletType,
		                                         Vector2 location,
		                                         float rotation,
		                                         float velocity,
		                                         float angularVelocity,
		                                         DanmakuField.CoordinateSystem coordSys = DanmakuField.CoordinateSystem.View) {
			return ProjectileManager.FireCurvedBullet (bulletType, WorldPoint(Util.To3D(location), coordSys), rotation, velocity, angularVelocity);
		}
		
		public void FireControlledBullet(ProjectilePrefab bulletType, 
		                                 Vector2 location, 
		                                 float rotation, 
		                                 IProjectileController controller,
		                                 CoordinateSystem coordSys = CoordinateSystem.View) {
			ProjectileManager.FireControlledBullet (bulletType, WorldPoint(Util.To3D(location), coordSys), rotation, controller);
		}

		public void SpawnEnemy(Enemy prefab, Vector2 location, CoordinateSystem coordSys = CoordinateSystem.View) {
			Enemy enemy = (Enemy)Instantiate(prefab);
			Transform transform = enemy.Transform;
			transform.position = WorldPoint(Util.To3D(location, 1f), coordSys);
			enemy.Field = this;
		}
	}
}
