using UnityEditor;
using UnityEngine;
using UnityUtilLib;
using UnityUtilLib.Editor;

namespace UnityUtilLib.Editor {

	/// <summary>
	/// A custom editor for editing FixedScreenAreaCamera instances.
	/// Adds a button for easy initialization
	/// </summary>
	[CustomEditor(typeof(FixedScreenAreaCamera))]
	internal class FixedScreenAreaCameraEditor : UnityEditor.Editor {

		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();
			if(GUILayout.Button("Reinitialize")) {
				Camera camera = (target as FixedScreenAreaCamera).gameObject.GetComponent<Camera>();
				if(camera == null) {
					camera = (target as FixedScreenAreaCamera).gameObject.AddComponent<Camera>();
				}
				SerializedProperty nativeAspectRatioProp = serializedObject.FindProperty("nativeAspectRatio");
				SerializedProperty nativeBoundsProp = serializedObject.FindProperty("nativeBounds");
				Vector2 screenSize = EditorUtil.GetGameViewAspectRatio();
				if(screenSize == Vector2.zero) {
					Debug.LogWarning("Warning: Game View in Free Aspect. Native Aspect Ratio is improperly initialized. Try again with a fixed aspect ratio.");
				}
				nativeAspectRatioProp.floatValue = screenSize.x / screenSize.y;
				nativeBoundsProp.rectValue = camera.rect;
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}