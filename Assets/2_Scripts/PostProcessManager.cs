using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public sealed class PostProcessManager : MonoBehaviour
{
	private Vignette _vignetteLayer;
	private ChromaticAberration _chromaticAberrationLayer;
	private ColorGrading _colorGrading;
	private Camera _camera;
	private PostProcessVolume _processVolume;
	private float _durationHoldBreath = 0, _durationGetdBreath = 0;

	[SerializeField] private float _vignetteIntensityMin, _vignetteIntensityMax;
	public static bool _isPostProssessOn, _isPostProssessHoldBreath, _isRedPostProssessOn;
	private bool _isAttacking, _isBreathing;

	private void Awake()
	{
		_camera = Camera.main;
		_processVolume = _camera.GetComponent<PostProcessVolume>();
		_isAttacking = _isBreathing = false;
	}

	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
		_processVolume.profile.TryGetSettings(out _colorGrading);
		_colorGrading.gamma.overrideState = true;
		_isPostProssessOn = _isPostProssessHoldBreath = false;
	}

	private void Update()
	{
		if (_isPostProssessOn)
		{
			_isAttacking = true;
			PostProcessAttack();
		}
		else if (!_isPostProssessOn && _isAttacking)
		{
			_isAttacking = false;
			PostProcessOff();
		}

		if (_isPostProssessHoldBreath)
		{
			_isBreathing = true;
			PostProcessHoldBreath(true);
		}
		else if (!_isPostProssessHoldBreath && !_isAttacking)
		{
			PostProcessHoldBreath();
		}

		if (_colorGrading.gamma.value.w != HudManager._gameGamma)
		{
			_colorGrading.gamma.overrideState = true;
			_colorGrading.gamma.value.w = HudManager._gameGamma;
		}

		if (_isRedPostProssessOn)
		{
			PostProcessAttack();
		}
		else if (!_isRedPostProssessOn)
		{
			PostProcessOff();
		}

	}

	public void PostProcessAttack()
	{
		_vignetteLayer.color.value = Color.red;
		_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteIntensityMin, _vignetteIntensityMax, (Mathf.Sin(3 * Time.time) + 1) * 0.5f);
		_chromaticAberrationLayer.intensity.value = (Mathf.Sin(3 * Time.time) + 1) * 0.5f;
	}

	public void PostProcessHoldBreath(bool isHoldingBreath = false)
	{
		if (isHoldingBreath)
		{
			_vignetteLayer.color.value = Color.black;
			_durationGetdBreath = 0;
			_vignetteLayer.intensity.value = Mathf.Lerp(0, 0.5f, _durationHoldBreath);
			_durationHoldBreath += Time.deltaTime / 10;
		}

		else if (_vignetteLayer.intensity.value > 0)
		{
			_durationHoldBreath = 0;
			_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteLayer.intensity.value, 0, _durationGetdBreath);
			_durationGetdBreath += Time.deltaTime / 10;
		}
	}

	public void PostProcessOff()
	{
		_vignetteLayer.intensity.value = 0;
		_chromaticAberrationLayer.intensity.value = 0;
	}
}
