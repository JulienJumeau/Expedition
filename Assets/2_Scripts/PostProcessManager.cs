using System.Collections;
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
	public static bool _isPostProssessAttack, _isPostProssessHoldBreath, _isPostProssessFall;
	private bool _isWounded, _isBreathing, _isFalling;
	private double originVignette, originChromatic;

	private void Awake()
	{
		_camera = Camera.main;
		_processVolume = _camera.GetComponent<PostProcessVolume>();
		_isWounded = _isBreathing = _isFalling = false;
	}

	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
		_processVolume.profile.TryGetSettings(out _colorGrading);
		_colorGrading.gamma.overrideState = true;
		_isPostProssessAttack = _isPostProssessHoldBreath = _isPostProssessFall = false;
	}

	private void Update()
	{
		if (_isPostProssessAttack)
		{
			_isWounded = true;
			PostProcessAttack();
		}

		else if (!_isPostProssessAttack && _isWounded)
		{
			_isWounded = false;
			StartCoroutine(PostProcessOff());
		}

		if (_isPostProssessHoldBreath)
		{
			_isBreathing = true;
			PostProcessHoldBreath(true);
		}

		else if (!_isPostProssessHoldBreath && !_isWounded)
		{
			_isBreathing = false;
			PostProcessHoldBreath(false);
		}

		if (_isPostProssessFall)
		{
			_isFalling = true;
			PostProcessAttack();
		}

		else if (!_isPostProssessFall && _isFalling && !_isBreathing && !_isWounded)
		{
			_isFalling = false;
			StartCoroutine(PostProcessOff());
		}

		if (_colorGrading.gamma.value.w != HudManager._gameGamma)
		{
			_colorGrading.gamma.overrideState = true;
			_colorGrading.gamma.value.w = HudManager._gameGamma;
		}
	}

	private void PostProcessAttack()
	{
		_vignetteLayer.color.value = Color.red;
		_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteIntensityMin, _vignetteIntensityMax, (Mathf.Sin(3 * Time.time) + 1) * 0.5f);
		_chromaticAberrationLayer.intensity.value = (Mathf.Sin(3 * Time.time) + 1) * 0.5f;
	}

	private void PostProcessHoldBreath(bool isHoldingBreath = false)
	{
		if (isHoldingBreath)
		{
			_vignetteLayer.color.value = Color.black;
			_durationGetdBreath = 0;
			_vignetteLayer.intensity.value = Mathf.Lerp(0.1f, 0.5f, _durationHoldBreath);
			_durationHoldBreath += Time.deltaTime / 10;
		}

		else if (_vignetteLayer.intensity.value > 0)
		{
			_durationHoldBreath = 0;
			_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteLayer.intensity.value, 0, _durationGetdBreath);
			_durationGetdBreath += Time.deltaTime / 10;
		}
	}

	private IEnumerator PostProcessOff()
	{
		float elapsedTime = 0, duration = 2;
		//originVignette = Math.Round(_vignetteLayer.intensity.value, 9);
		//originChromatic = Math.Round(_chromaticAberrationLayer.intensity.value, 9);
		//Debug.Log(originVignette);
		while (elapsedTime <= duration)
		{
			elapsedTime += Time.deltaTime;
			_vignetteLayer.color.value = new Color(_vignetteLayer.color.value.r, _vignetteLayer.color.value.g, _vignetteLayer.color.value.b, Mathf.Lerp(1, 0, elapsedTime / duration));
			_chromaticAberrationLayer.intensity.value = Mathf.Lerp((float)originChromatic, 0, elapsedTime / duration);
			//_vignetteLayer.opacity.value = Mathf.Lerp(1, 0, elapsedTime / duration);
			//_vignetteLayer.intensity.value = Mathf.Lerp((float)originVignette, 0, elapsedTime / duration);
			yield return null;
		}

		_vignetteLayer.intensity.value = 0;
		_chromaticAberrationLayer.intensity.value = 0;
		_vignetteLayer.opacity.value = 1;
	}
}
