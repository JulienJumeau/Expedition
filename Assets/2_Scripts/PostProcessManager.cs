using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public sealed class PostProcessManager : MonoBehaviour
{
	Vignette _vignetteLayer;
	ChromaticAberration _chromaticAberrationLayer;
	Camera _camera;
	PostProcessVolume _processVolume;

	[SerializeField] private float _vignetteIntensityMin, _vignetteIntensityMax;
	public static bool _isPostProssessOn;

	private void Awake()
	{
		_camera = Camera.main;
		_processVolume = _camera.GetComponent<PostProcessVolume>();
	}

	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
		_isPostProssessOn = false;
	}

	private void Update()
	{
		if (_isPostProssessOn)
		{
			PostProcess();
		}
	}

	public void PostProcess()
	{
		_vignetteLayer.intensity.value = Mathf.Lerp(_vignetteIntensityMin, _vignetteIntensityMax, (Mathf.Sin(3* Time.time) + 1) * 0.5f);
		_chromaticAberrationLayer.intensity.value = (Mathf.Sin(3* Time.time) + 1) * 0.5f;
	}

	//public void GetPostProcessValue()
	//{

	//}
}
