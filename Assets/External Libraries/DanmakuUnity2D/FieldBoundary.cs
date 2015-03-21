using System;
using UnityEngine;
using UnityUtilLib;

namespace Danmaku2D {

	[ExecuteInEditMode]
	public class FieldBoundary : CachedObject {

		private enum Edge { Top = 0, Bottom = 1, Left = 2, Right = 3}
		
		private static Vector2[] fixedPoints = new Vector2[] {
			new Vector2 (0, 1f),
			new Vector2 (0, -1f),
			new Vector2 (-1f, 0f),
			new Vector2 (1f, 0f)
		};
		
		[SerializeField]
		private DanmakuField field;
		
		[SerializeField]
		private Edge location;
		
		[SerializeField]
		private float bufferRatio = 0.1f;

		[SerializeField]
		private float hangoverRatio = 0f;
		
		[SerializeField]
		private float spaceRatio = 0;
		
		private BoxCollider2D boundary;
		private Bounds oldBounds;
		private Bounds newBounds;
		
		public override void Awake () {
			base.Awake ();
			boundary = GetComponent<BoxCollider2D> ();
			if (field == null) {
				Debug.Log("No field provided, searching in ancestor GameObjects...");
				field = GetComponentInParent<DanmakuField>();
			}
			if (field == null) {
				Debug.LogError ("Field Boundary without a DanmakuField");
			} else {
				UpdatePosition ();
			}
		}
		
		void Update () {
			if (field != null && field.MovementBounds != oldBounds) {
				UpdatePosition ();
			}
		}

		void OnDrawGizmos() {
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube (boundary.bounds.center, boundary.bounds.size);
		}
		
		private void UpdatePosition() {
			oldBounds = field.MovementBounds;

			float size = Util.MaxComponent2 (oldBounds.size);
			Vector2 newPosition = (Vector2)oldBounds.center + Util.HadamardProduct2(fixedPoints [(int)location], oldBounds.extents);
			float buffer = bufferRatio * size;
			float space = spaceRatio * size;
			float hangover = hangoverRatio * size;
			
			Vector2 area = boundary.size;
			switch(location) {
			case Edge.Top:
			case Edge.Bottom:
				area.y = buffer;
				area.x = oldBounds.size.x + hangover;
				break;
			case Edge.Left:
			case Edge.Right:
				area.x = buffer;
				area.y = oldBounds.size.y + hangover;
				break;
			}
			boundary.size = area;

			oldBounds = boundary.bounds;
			switch(location) {
			case Edge.Top:
				newPosition.y += oldBounds.extents.y + space;
				break;
			case Edge.Bottom:
				newPosition.y -= oldBounds.extents.y + space;
				break;
			case Edge.Left:
				newPosition.x -= oldBounds.extents.x + space;
				break;
			case Edge.Right:
				newPosition.x += oldBounds.extents.x + space;
				break;
			}

			transform.position = newPosition;
		}
	}
}