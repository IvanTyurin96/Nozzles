#if UNITY_EDITOR
namespace Assets.Scripts.Craft.Parts.Modifiers.EditorScripts
{
    using ModApi.Craft.Parts.Editor;
    using ModApi.Craft.Parts.Modifiers;
	using UnityEngine;

	/// <summary>
	/// An editor only class used to associated part modifiers with game objects when defining parts.
	/// </summary>
    public sealed class CustomNozzleEditorScript : PartModifierEditorScript<CustomNozzleData>
    {
	}
}
#endif