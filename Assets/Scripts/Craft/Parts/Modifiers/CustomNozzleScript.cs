namespace Assets.Scripts.Craft.Parts.Modifiers
{
	using Assets.Scripts.Craft.Parts.Modifiers.Propulsion;
	using Assets.Scripts.Extensions;
	using ModApi.Common.Extensions;
	using ModApi.Craft.Parts;
	using ModApi.Craft.Parts.Input;
	using System.Linq.Expressions;
	using UnityEngine;

	public class CustomNozzleScript : PartModifierScript<CustomNozzleData>
    {
		/// <summary>
		/// The stock nozzle
		/// </summary>
		private Transform _stockNozzle;

		/// <summary>
		/// This flag indicates that <see cref="_stockNozzle"/> was found and exists
		/// I use flags because checking boolean value true or false is faster that checking object is null
		/// </summary>
		private bool _stockNozzleExist = false;

		/// <summary>
		/// The stock object where force of jet engine apply
		/// </summary>
		private Transform _stockForceTransform;

		/// <summary>
		/// This flag indicates that <see cref="_stockForceTransform"/> was found and exists
		/// </summary>
		private bool _stockForceTransformExist = false;

		/// <summary>
		/// Custom nozzle, AL-31F or AL-41F
		/// </summary>
		private GameObject _customNozzle;

		/// <summary>
		/// The rotating part of AL-41F
		/// </summary>
		private Transform _customNozzleRotatingPart;

		/// <summary>
		/// This flag indicates that <see cref="_customNozzleRotatingPart"/> was found and exists
		/// </summary>
		private bool _customNozzleRotatingPartExist = false;

		/// <summary>
		/// The exhaust particle system of stock nozzle
		/// </summary>
		private Transform _stockExhaustParticleSystem;

		/// <summary>
		/// This flag indicates that <see cref="_stockExhaustParticleSystem"/> was found and exists
		/// </summary>
		private bool _stockExhaustParticleSystemExist = false;

		/// <summary>
		/// The hot air (distortion) particle system of stock nozzle
		/// </summary>
		private Transform _stockDistortionEffect;

		/// <summary>
		/// This flag indicates that <see cref="_stockDistortionEffect"/> was found and exists
		/// </summary>
		private bool _stockDistortionEffectExist = false;

		/// <summary>
		/// The smoke particle system of stock nozzle
		/// </summary>
		private Transform _stockSmokeParticleSystem;

		/// <summary>
		/// This flag indicates that <see cref="_stockSmokeParticleSystem"/> was found and exists
		/// </summary>
		private bool _stockSmokeParticleSystemExist = false;

		/// <summary>
		///  Animator that controls throttle animation of custom nozzle
		/// </summary>
		private Animator animator;

		/// <summary>
		/// This flag indicates that <see cref="animator"/> was found and exists
		/// </summary>
		private bool _animatorExist = false;

		/// <summary>
		/// Throttle input controller
		/// </summary>
		private IInputController _throttleInputController;

		/// <summary>
		/// This flag indicates that <see cref="_throttleInputController"/> was found and exists
		/// </summary>
		private bool _throttleInputControllerExist = false;

		/// <summary>
		/// Pitch input controller for thrust vectoring system of AL-41F
		/// </summary>
		private IInputController _pitchInputController;

		/// <summary>
		/// This flag indicates that <see cref="_pitchInputController"/> was found and exists
		/// </summary>
		private bool _pitchInputControllerExist = false;

		/// <summary>
		/// Yaw input controller for thrust vectoring system of AL-41F
		/// </summary>
		private IInputController _yawInputController;

		/// <summary>
		/// This flag indicates that <see cref="_yawInputController"/> was found and exists
		/// </summary>
		private bool _yawInputControllerExist = false;

		/// <summary>
		/// Throttle tracking value
		/// </summary>
		private float _throttleTrackingValue = 0f;

		/// <summary>
		/// Pitch tracking value
		/// </summary>
		private float _pitchTrackingValue = 0f;

		/// <summary>
		/// Yaw tracking value
		/// </summary>
		private float _yawTrackingValue = 0f;

		//CONSTANTS
		private readonly float _internalDiameter = 1.020f;
		private readonly float _externalDiameter = 1.180f;
		private readonly float _stockDiameter = 0.994506f;

		//MODIFIERS
		private string _nozzleType;
		private string _sizing;
		private float _throttleAnimationSpeed;
		private float _throttleMinAnimation;
		private float _throttleMaxAnimation;
		private float _exhaustMinScale;
		private float _exhaustMaxScale;
		private float _smokeScale;
		private float _distortionMinScale;
		private float _distortionMaxScale;
		private float _pitchAngle;
		private float _yawAngle;
		private float _rotatingSpeed;
		private bool _disableStockNozzle;
		private float _nozzleScale;
		private float _nozzleOffset;

		//JET ENGINE DATA MODIFIERS
		private float _jetEngineScale = 1f;
		private float _jetEngineBypassRatio = 1f;
		private float _jetEngineCompressionRatio = 1f;
		private bool _jetEngineAfterburner = true;
		private float _jetEngineShroudLength = 1f;

		//FOR DESIGNER UPDATE
		private float _currentModifiersSumValue = 0f;
		private float _lastModifiersSumValue = 0f;
		private string _currentStringModifiersSumValue = string.Empty;
		private string _lastStringModifiersSumValue = string.Empty;
		private float _currentJetEngineDataModifiersSumValue = 0f;
		private float _lastJetEngineDataModifiersSumValue = 0f;

		private void Start()
		{
			DestroyNozzle();
			GetModifiers();
			GetJenEngineDataModifiers();
			GetInputControllers();
			FindStockObjects();
			DisableStockNozzleMeshes();
			InstaniateNozzle();
			TuneNozzle();
			GetAnimator();
			StockObjectsParentOverride();
		}

		private void Update()
		{
			Animate();
			RotateNozzle();
			ExhaustOverride();
			DistortionOverride();

			DesignerUpdating();
		}

		private void DestroyNozzle()
		{
			if (_customNozzle != null)
			{
				Destroy(_customNozzle);
			}
			_stockNozzleExist = false;
			_stockExhaustParticleSystemExist = false;
			_stockForceTransformExist = false;
			_stockDistortionEffectExist = false;
			_stockSmokeParticleSystemExist = false;
			_animatorExist = false;
			_throttleInputControllerExist = false;
			_pitchInputControllerExist = false;
			_yawInputControllerExist = false;
			_throttleTrackingValue = 0f;
			_pitchTrackingValue = 0f;
			_yawTrackingValue = 0f;
			_customNozzleRotatingPartExist = false;
		}

		private void GetModifiers()
		{
			switch(this.Data.nozzleType)
			{
				case "AL-31F":
					_nozzleType = "AL-31F";
					break;
				case "AL-41F":
					_nozzleType = "AL-41F";
					break;
				default:
					_nozzleType = "AL-31F";
					break;
			}

			_sizing = this.Data.sizing;
			_throttleAnimationSpeed = Mathf.Clamp(this.Data.throttleAnimationSpeed, 0f, 1000f);
			_throttleMinAnimation = Mathf.Clamp(this.Data.throttleMinAnimation, 0f, 1f);
			_throttleMaxAnimation = Mathf.Clamp(this.Data.throttleMaxAnimation, 0f, 1f);
			_exhaustMinScale = Mathf.Clamp(this.Data.exhaustMinScale, 0f, 1000000f);
			_exhaustMaxScale = Mathf.Clamp(this.Data.exhaustMaxScale, 0f, 1000000f);
			_smokeScale = Mathf.Clamp(this.Data.smokeScale, 0f, 1000000f);
			_distortionMinScale = Mathf.Clamp(this.Data.distortionMinScale, 0f, 1000000);
			_distortionMaxScale = Mathf.Clamp(this.Data.distortionMaxScale, 0f, 1000000);
			_pitchAngle = this.Data.pitchAngle;
			_yawAngle = this.Data.yawAngle;
			_rotatingSpeed = Mathf.Clamp(this.Data.rotatingSpeed, 0f, 1000000f);
			_disableStockNozzle = this.Data.disableStockNozzle;
			_nozzleScale = Mathf.Clamp(this.Data.nozzleScale, 0.001f, 1000000f);
			_nozzleOffset = this.Data.nozzleOffset;
		}

		private void GetJenEngineDataModifiers()
		{
			var jetEngineScript = this.transform.GetComponent<JetEngineScript>();

			if (jetEngineScript != null)
			{
				_jetEngineScale = jetEngineScript.Data.Scale;
				_jetEngineBypassRatio = jetEngineScript.Data.BypassRatio;
				_jetEngineCompressionRatio = jetEngineScript.Data.CompressionRatio;
				_jetEngineAfterburner = jetEngineScript.Data.HasAfterburner;
				_jetEngineShroudLength = jetEngineScript.Data.ShroudLength;
			}
			else
			{
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: {nameof(JetEngineScript)} not found. Automatic nozzle update after changing jet engine modifiers not available.");
			}
		}

		private void GetInputControllers()
		{
			_throttleInputController = GetInputController($"{this.Data.throttleInputId}");
			if (_throttleInputController != null)
			{
				_throttleInputControllerExist = true;
			}
			else
			{
				_throttleInputControllerExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: {nameof(_throttleInputController)} with input id {this.Data.throttleInputId} not found. Not possible to animate nozzle.");
			}

			_pitchInputController = GetInputController($"{this.Data.pitchInputId}");
			if (_pitchInputController != null)
			{
				_pitchInputControllerExist = true;
			}
			else
			{
				_pitchInputControllerExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: {nameof(_pitchInputController)} with input id {this.Data.pitchInputId} not found. Not possible to rotate AL-41F nozzle.");
			}

			_yawInputController = GetInputController($"{this.Data.yawInputId}");
			if (_yawInputController != null)
			{
				_yawInputControllerExist = true;
			}
			else
			{
				_yawInputControllerExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: {nameof(_yawInputController)} with input id {this.Data.yawInputId} not found. Not possible to rotate AL-41F nozzle.");
			}
		}

		private void FindStockObjects()
		{
			_stockNozzle = this.transform.FindChildByName(this.Data.stockNozzleName);
			if (_stockNozzle != null)
			{
				_stockNozzleExist = true;
			}
			else
			{
				_stockNozzleExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: stock {this.Data.stockNozzleName} not found. Automatic nozzle positioning and scaling not available. Not possible to disable stock nozzle mesh.");
			}

			_stockExhaustParticleSystem = this.transform.FindChildByName(this.Data.stockExhaustParticleSystemName);
			if (_stockExhaustParticleSystem != null)
			{
				_stockExhaustParticleSystemExist = true;
			}
			else
			{
				_stockExhaustParticleSystemExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: stock {this.Data.stockExhaustParticleSystemName} not found. Exhaust override not available. Not possible to move and rotate exhaust for AL-41F.");
			}

			_stockForceTransform = this.transform.FindChildByContainsName(this.Data.stockForceTransformName);
			if (_stockForceTransform != null)
			{
				_stockForceTransformExist = true;
			}
			else
			{
				_stockForceTransformExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: stock {this.Data.stockForceTransformName} not found. Not possible to rotate force direction for AL-41F.");
			}

			_stockDistortionEffect = this.transform.FindChildByContainsName(this.Data.stockDistortionEffectName);
			if (_stockDistortionEffect != null)
			{
				_stockDistortionEffectExist = true;
			}
			else
			{
				_stockDistortionEffectExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: stock {this.Data.stockDistortionEffectName} not found. Not possible to move and rotate distortion effect for AL-41F. Distortion override not available.");
			}

			_stockSmokeParticleSystem = this.transform.FindChildByName(this.Data.stockSmokeParticleSystemName);
			if (_stockSmokeParticleSystem != null)
			{
				_stockSmokeParticleSystemExist = true;
			}
			else
			{
				_stockSmokeParticleSystemExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: stock {this.Data.stockSmokeParticleSystemName} not found. Not possible to move and rotate smoke effect for AL-41F. Smoke override not available.");
			}
		}

		private void DisableStockNozzleMeshes()
		{
			if (_stockNozzleExist)
			{
				if (_disableStockNozzle)
				{
					if (_stockNozzle.GetComponent<MeshRenderer>() != null)
					{
						_stockNozzle.GetComponent<MeshRenderer>().enabled = false;
					}

					foreach (MeshRenderer meshRenderer in _stockNozzle.GetComponentsInChildren<MeshRenderer>())
					{
						meshRenderer.enabled = false;
					}
				}
				else
				{
					if (_stockNozzle.GetComponent<MeshRenderer>() != null)
					{
						_stockNozzle.GetComponent<MeshRenderer>().enabled = true;
					}

					foreach (MeshRenderer meshRenderer in _stockNozzle.GetComponentsInChildren<MeshRenderer>())
					{
						meshRenderer.enabled = true;
					}
				}
			}
		}

		private void InstaniateNozzle()
		{
			_customNozzle = Instantiate(Mod.Instance.Mod.AssetBundle.LoadAsset<GameObject>(_nozzleType));
			_customNozzle.SetActive(true);

			if (_stockNozzleExist)
			{
				_customNozzle.transform.parent = _stockNozzle.transform;
			}
			else
			{
				_customNozzle.transform.parent = this.transform;
			}

			_customNozzle.transform.localPosition = Vector3.zero;
			_customNozzle.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);

			if (_nozzleType == "AL-41F")
			{
				_customNozzleRotatingPart = _customNozzle.transform.GetChild(0);
				_customNozzleRotatingPartExist = true;
			}
		}

		private void TuneNozzle()
		{
			_customNozzle.transform.localPosition = new Vector3(0f, _nozzleOffset, 0f);

			float diameterScaleFactor = 1f;

			switch (this.Data.sizing)
			{
				case "External":
					diameterScaleFactor = _stockDiameter / _externalDiameter;
					break;
				case "Internal":
					diameterScaleFactor = _stockDiameter / _internalDiameter;
					break;
				default:
					diameterScaleFactor = _stockDiameter / _internalDiameter;
					break;
			}

			float scale = (_nozzleScale / 100f) * diameterScaleFactor * 2f;

			_customNozzle.transform.localScale = new Vector3(scale, scale, scale);
		}

		private void GetAnimator()
		{
			animator = _customNozzle.GetComponent<Animator>();

			if (animator != null)
			{
				_animatorExist = true;
			}
			else
			{
				_animatorExist = false;
				Game.Instance.DevConsole.LogWarning($"{nameof(CustomNozzleScript)}: {nameof(animator)} not found.");
			}
		}

		private void StockObjectsParentOverride()
		{
			if (Game.InFlightScene)
			{
				if (_stockExhaustParticleSystemExist)
				{
					_stockExhaustParticleSystem.transform.parent = _customNozzle.transform;
					if (_nozzleType == "AL-41F")
					{
						_stockExhaustParticleSystem.transform.parent = _customNozzleRotatingPart.transform;
					}
					_stockExhaustParticleSystem.transform.localPosition = Vector3.zero;
				}

				if (_stockForceTransformExist)
				{
					_stockForceTransform.transform.parent = _customNozzle.transform;
					if (_nozzleType == "AL-41F")
					{
						_stockForceTransform.transform.parent = _customNozzleRotatingPart.transform;
					}
					_stockForceTransform.transform.localPosition = Vector3.zero;
				}

				if (_stockDistortionEffectExist)
				{
					_stockDistortionEffect.transform.parent = _customNozzle.transform;
					if (_nozzleType == "AL-41F")
					{
						_stockDistortionEffect.transform.parent = _customNozzleRotatingPart.transform;
					}
					_stockDistortionEffect.transform.localPosition = Vector3.zero;
				}

				if (_stockSmokeParticleSystemExist)
				{
					_stockSmokeParticleSystem.transform.parent = _customNozzle.transform;
					if (_nozzleType == "AL-41F")
					{
						_stockSmokeParticleSystem.transform.parent = _customNozzleRotatingPart.transform;
					}
					_stockSmokeParticleSystem.transform.localPosition = Vector3.zero;
					_stockSmokeParticleSystem.transform.localScale = new Vector3(_smokeScale, _smokeScale, _smokeScale);
				}
			}
		}

		private void Animate()
		{
			if (_animatorExist && _throttleInputControllerExist)
			{
				float inputControllerValue = 0f;

				if (Game.InDesignerScene)
				{
					inputControllerValue = 1f;
				}
					
				if (Game.InFlightScene)
				{
					inputControllerValue = _throttleInputController.Value;
				}
					
				float changeSpeed = _throttleAnimationSpeed * Time.deltaTime;

				_throttleTrackingValue = _throttleTrackingValue < inputControllerValue
					? _throttleTrackingValue += changeSpeed
					: _throttleTrackingValue -= changeSpeed;

				if (Mathf.Abs(_throttleTrackingValue - inputControllerValue) <= 2f * changeSpeed)
				{
					_throttleTrackingValue = inputControllerValue;
				}
				_throttleTrackingValue = Mathf.Clamp(_throttleTrackingValue, 0f, 1f);

				float interpolatedAnimationTime = Mathf.Lerp(_throttleMinAnimation, _throttleMaxAnimation, _throttleTrackingValue);

				animator.SetFloat("Throttle", interpolatedAnimationTime);
			}
		}

		private void RotateNozzle()
		{
			if (_customNozzleRotatingPartExist && _pitchInputControllerExist && _yawInputControllerExist)
			{
				float pitchInput = 0f;
				float yawInput = 0f;

				if (Game.InFlightScene)
				{
					pitchInput = -_pitchInputController.Value * _pitchAngle;
					yawInput = -_yawInputController.Value * _yawAngle;
				}

				float changeSpeed = _rotatingSpeed * Time.deltaTime;
				float doubleFrameIncrement = 2f * changeSpeed;

				_pitchTrackingValue = _pitchTrackingValue < pitchInput
					? _pitchTrackingValue += changeSpeed
					: _pitchTrackingValue -= changeSpeed;
				if (Mathf.Abs(_pitchTrackingValue - pitchInput) <= doubleFrameIncrement)
				{
					_pitchTrackingValue = pitchInput;
				}

				_yawTrackingValue = _yawTrackingValue < yawInput
					? _yawTrackingValue += changeSpeed
					: _yawTrackingValue -= changeSpeed;
				if (Mathf.Abs(_yawTrackingValue - yawInput) <= doubleFrameIncrement)
				{
					_yawTrackingValue = yawInput;
				}

				_customNozzleRotatingPart.transform.localEulerAngles = new Vector3(_pitchTrackingValue, _yawTrackingValue, 0f);
			}
		}

		private void ExhaustOverride()
		{
			float interpolatedExhaustScale = Mathf.Lerp(_exhaustMinScale, _exhaustMaxScale, _throttleTrackingValue);

			if (Game.InDesignerScene)
			{
				if (_stockExhaustParticleSystem != null)
				{
					_stockExhaustParticleSystem.transform.position = _customNozzle.transform.position;
					if (_customNozzleRotatingPart != null)
					{
						_stockExhaustParticleSystem.transform.position = _customNozzleRotatingPart.transform.position;
					}

					float scaleFactor = _nozzleScale / 100f;
					_stockExhaustParticleSystem.transform.localScale = new Vector3(interpolatedExhaustScale * scaleFactor, interpolatedExhaustScale * scaleFactor, interpolatedExhaustScale * scaleFactor);
				}
			}

			if (Game.InFlightScene)
			{
				if (_stockExhaustParticleSystemExist)
				{
					float scaleFactor = 1f / _customNozzle.transform.localScale.y;
					_stockExhaustParticleSystem.transform.localScale = new Vector3(interpolatedExhaustScale * scaleFactor, interpolatedExhaustScale * scaleFactor, interpolatedExhaustScale * scaleFactor);
				}
			}
		}

		private void DistortionOverride()
		{
			float interpolatedDistortionScale = Mathf.Lerp(_distortionMinScale, _distortionMaxScale, _throttleTrackingValue);

			if (Game.InFlightScene)
			{
				if (_stockDistortionEffectExist)
				{
					float scaleFactor = 1f / _customNozzle.transform.localScale.y;
					_stockDistortionEffect.transform.localScale = new Vector3(interpolatedDistortionScale * scaleFactor, interpolatedDistortionScale * scaleFactor, interpolatedDistortionScale * scaleFactor);
				}
			}
		}

		private void DesignerUpdating()
		{
			if (Game.InDesignerScene)
			{
				GetModifiers();
				GetJenEngineDataModifiers();

				int disableStockNozzle = _disableStockNozzle ? 1 : 0;

				_lastModifiersSumValue = _currentModifiersSumValue;
				_currentModifiersSumValue = _throttleAnimationSpeed +
											_throttleMinAnimation +
											_throttleMaxAnimation +
											_exhaustMinScale +
											_exhaustMaxScale +
											disableStockNozzle +
											_nozzleScale +
											_nozzleOffset;

				_lastStringModifiersSumValue = _currentStringModifiersSumValue;
				_currentStringModifiersSumValue = _nozzleType +_sizing;

				int jetEngineAfterburner = _jetEngineAfterburner ? 1 : 0;

				_lastJetEngineDataModifiersSumValue = _currentJetEngineDataModifiersSumValue;
				_currentJetEngineDataModifiersSumValue = _jetEngineScale +
														 _jetEngineBypassRatio +
														 _jetEngineCompressionRatio +
														 jetEngineAfterburner +
														 _jetEngineShroudLength;

				if (Mathf.Abs(_currentModifiersSumValue - _lastModifiersSumValue) > Mathf.Epsilon ||
					_currentStringModifiersSumValue != _lastStringModifiersSumValue ||
					Mathf.Abs(_currentJetEngineDataModifiersSumValue - _lastJetEngineDataModifiersSumValue) > Mathf.Epsilon)
				{
					Start();
				}
			}
		}
	}
}