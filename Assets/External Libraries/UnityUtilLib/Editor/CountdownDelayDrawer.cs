using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor scripts for various components of UnityUtilLib
/// </summary>
namespace UnityUtilLib.Editor {

	/// <summary>
	/// Custom <a href="http://docs.unity3d.com/ScriptReference/PropertyDrawer.html">PropertyDrawer</a> for CountdownDelay
	/// </summary>
	[CustomPropertyDrawer(typeof(CountdownDelay))]
	public class CountdownDelayDrawer : PropertyDrawer {

		/// <summary>
		/// Creates the custom GUI an instance of CountdownDelay
		/// Abstracts away all of the hidden variables and only exposes a float field for easy editing
		/// </summary>
		/// <param name="position"> the rectangle on the screen to use for the property GUI.</param>
		/// <param name="property">The <a href="http://docs.unity3d.com/ScriptReference/SerializedProperty.html">SerializedProperty</a> to make the custom GUI for.</param>
		/// <param name="label">The label of this property</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			SerializedProperty maxDelayProp = property.FindPropertyRelative ("maxDelay");
			SerializedProperty currentDelayProp = property.FindPropertyRelative ("currentDelay");
			EditorGUI.PropertyField (position, maxDelayProp, label);
			currentDelayProp.floatValue = maxDelayProp.floatValue;
			EditorGUI.EndProperty();
		}
	}
}