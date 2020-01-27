using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public sealed class PostProcessManager : MonoBehaviour
{
	Vignette _vignetteLayer;
	ChromaticAberration _chromaticAberrationLayer;
	Camera _camera;
	PostProcessVolume _processVolume;

	private void Awake()
	{
		_camera = Camera.main;
		_processVolume = _camera.GetComponent<PostProcessVolume>();
	}

	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
	}

	private void Update()
	{
		OnOffPostProcess();
	}

	public void OnOffPostProcess()
	{
		//_vignetteLayer.intensity.value = Mathf.PingPong();
		_chromaticAberrationLayer.intensity.value = (Mathf.Sin(Time.time) + 1);
	}

	public void GetPostProcessValue()
	{

	}

	//public 
}
