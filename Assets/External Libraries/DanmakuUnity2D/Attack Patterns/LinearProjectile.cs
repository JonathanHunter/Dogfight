using System;
using System.Collections.Generic;
using UnityEngine;

namespace Danmaku2D {
	public class LinearProjectile : ProjectileController {
		public float Velocity {
			get;
			set;
		}

		public LinearProjectile (float velocity) : base() {
			Velocity = velocity;
		}
		
		#region IProjectileController implementation
		
		public override Vector2 UpdateProjectile (Projectile projectile, float dt) {
			base.UpdateProjectile (projectile, dt);
			if (Velocity != 0)
				return projectile.Transform.up * Velocity * dt;
			else
				return Vector2.zero;
		}
		
		#endregion
	}
}
