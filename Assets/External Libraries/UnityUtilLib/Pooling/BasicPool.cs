using System;

/// <summary>
/// A set of useful classes to implement any from of <a href="http://en.wikipedia.org/wiki/Object_pool_pattern>object pooling</a> that is commonly used in video games
/// </summary>
namespace UnityUtilLib.Pooling {

	/// <summary>
	/// A basic implementation of Pool that constructs new objects using the <c>new</c> operator.
	/// The given type must have a default parameterless constructor to use with this pool.
	/// </summary>
	public class BasicPool<T> : Pool<T> where T : IPooledObject, new() {

		public BasicPool(int initial, int spawn) : base(initial, spawn) {
		}

		#region implemented abstract members of Pool
		protected override T CreateNew () {
			return new T ();
		}
		#endregion
	}
}

