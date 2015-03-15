using UnityEngine;

/// <summary>
/// A development kit for quick development of 2D Danmaku games
/// </summary>
namespace Danmaku2D {

	/// <summary>
	/// An interface for defining any controller of single Projectile.
	/// If looking to reuse behavior among large numbers of projectiles, use a ProjectileGroup and IProjectileGroupController instead.
	/// Generally speaking it's best not to directly implement this interface manually, use only when sublcassing ProjectileController or ProjectileControlBehavior does not work.
	/// </summary>
	public interface IProjectileController {

		/// <summary>
		/// Gets or sets the projectile controlled by this controller.
		/// </summary>
		/// <value>The projectile controlled by this controller</value>
		Projectile Projectile { get; set; }

		/// <summary>
		/// Updates the Projectile controlled by the controller instance.
		/// </summary>
		/// <returns>the displacement from the Projectile's original position after udpating</returns>
		/// <param name="dt">the change in time since the last update</param>
		Vector2 UpdateProjectile (float dt);

	}
}