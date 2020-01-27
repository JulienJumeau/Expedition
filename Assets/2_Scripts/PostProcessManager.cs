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

	// Start is called before the first frame update
	private void Start()
	{
		_processVolume.profile.TryGetSettings(out _chromaticAberrationLayer);
		_processVolume.profile.TryGetSettings(out _vignetteLayer);
	}

	// Update is called once per frame
	private void Update()
	{
		//_vignetteLayer.intensity.value = (Mathf.Sin(Time.time) + 1) * 0.5f;
		Debug.Log((Mathf.Sin(Time.time) + 1) * 0.5f);
	}

	//public 
}
