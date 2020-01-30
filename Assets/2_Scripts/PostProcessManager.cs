using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public sealed class PostProcessManager : MonoBehaviour
{
	private Vignette _vignetteLayer;
	private ChromaticAberration _chromaticAberrationLayer;
	private Camera _camera;
	private PostProcessVolume _processVolume;
	private float _durationHoldBreath = 0, _durationGetdBreath = 0;

	[SerializeField] private float _vignetteIntensityMin, _vignetteIntensityMax;
	public static bool _isPostProssessOn, _isPostProssessHoldBreath;

	private void Awake()
	{
		_camera = Camera.main;
		_processVolume = _camera.GetComponent<PostProcessVolume>();
	}

	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
		_isPostProssessOn = _isPostProssessHoldBreath = false;
	}

	private void Update()
	{
		if (_isPostProssessOn)
		{
			PostProcessAttack();
			Debug.Log(_isPostProssessOn);
		}

		if (!_isPostProssessOn)
		{
			PostProcessOff();
			Debug.Log(_isPostProssessOn);
		}

		//if (_isPostProssessHoldBreath)
		//{
		//	PostProcessHoldBreath(true);
		//}
		//else
		//{
		//	PostProcessHoldBreath();
		//}
	}

	public void PostProcessAttack()
	{
		_vignetteLayer.color.value = Color.red;
		_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteIntensityMin, _vignetteIntensityMax, (Mathf.Sin(3 * Time.time) + 1) * 0.5f);
		_chromaticAberrationLayer.intensity.value = (Mathf.Sin(3 * Time.time) + 1) * 0.5f;
	}

	public void PostProcessHoldBreath(bool isHoldingBreath = false)
	{
		_vignetteLayer.color.value = Color.black;
		
		if (isHoldingBreath)
		{
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
