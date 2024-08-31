namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using ModApi.Craft.Parts;
    using ModApi.Craft.Parts.Attributes;
    using UnityEngine;

    [Serializable]
    [DesignerPartModifier("CustomNozzle")]
    [PartModifierTypeId("Nozzles.CustomNozzle")]
    public class CustomNozzleData : PartModifierData<CustomNozzleScript>
    {
		[DesignerPropertySpinner("AL-31F", "AL-41F", Label = "Nozzle type", Header = "Nozzle: general")]
		public string nozzleType = "AL-31F";

		[DesignerPropertySpinner("External", "Internal", Label = "Sizing")]
		public string sizing = "External";

		[DesignerPropertySlider(0f, 1f, 21, Label = "Throttle animation speed", Header = "Nozzle: animation")]
		public float throttleAnimationSpeed = 0.1f;

		[DesignerPropertySlider(0f, 1f, 11, Label = "Throttle min animation")]
		public float throttleMinAnimation = 0f;

		[DesignerPropertySlider(0f, 1f, 11, Label = "Throttle max animation")]
		public float throttleMaxAnimation = 1f;

		[DesignerPropertySlider(0f, 3f, 31, Label = "Exhaust min scale", Header = "Nozzle: exhaust override")]
		public float exhaustMinScale = 0.9f;

		[DesignerPropertySlider(0f, 3f, 31, Label = "Exhaust max scale")]
		public float exhaustMaxScale = 1.8f;

		[DesignerPropertySlider(0f, 3f, 31, Label = "Smoke scale", Header = "Nozzle: smoke override")]
		public float smokeScale = 1.5f;

		[DesignerPropertySlider(0f, 3f, 31, Label = "Distortion min scale", Header = "Nozzle: distortion override")]
		public float distortionMinScale = 1.0f;

		[DesignerPropertySlider(0f, 3f, 31, Label = "Distortion max scale")]
		public float distortionMaxScale = 1.0f;

		[DesignerPropertySpinner(IsHidden = true)]
		public string throttleInputId = "Throttle";

		[DesignerPropertySpinner(IsHidden = true)]
		public string pitchInputId = "Pitch";

		[DesignerPropertySpinner(IsHidden = true)]
		public string yawInputId = "Yaw";

		[DesignerPropertySlider(0f, 30f, 31, Label = "Pitch angle", Header = "Nozzle: TVS")]
		public float pitchAngle = 10f;

		[DesignerPropertySlider(0f, 30f, 31, Label = "Yaw angle")]
		public float yawAngle = 10f;

		[DesignerPropertySlider(0f, 60f, 61, Label = "Rotating speed")]
		public float rotatingSpeed = 60f;

		[DesignerPropertyToggleButton(Label = "Disable stock nozzle", Header = "Nozzle: other")]
		public bool disableStockNozzle = true;

		[DesignerPropertySlider(50f, 150f, 101, Label = "Nozzle scale", Header = "Nozzle: manual transforming")]
		public float nozzleScale = 100f;

		[DesignerPropertySlider(-2f, 2f, 41, Label = "Nozzle offset")]
		public float nozzleOffset = 0f;

		[DesignerPropertySpinner(IsHidden = true)]
		public string stockNozzleName = "Nozzle";

		[DesignerPropertySpinner(IsHidden = true)]
		public string stockExhaustParticleSystemName = "ExhaustParticleSystem";

		[DesignerPropertySpinner(IsHidden = true)]
		public string stockForceTransformName = "ForceTransform";

		[DesignerPropertySpinner(IsHidden = true)]
		public string stockDistortionEffectName = "DistortionEffect";

		[DesignerPropertySpinner(IsHidden = true)]
		public string stockSmokeParticleSystemName = "SmokeParticleSystem";
	}
}