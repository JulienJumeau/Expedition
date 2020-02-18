using System.Collections;
using UnityEngine;

public class PostProcessTrigger : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private float _postprocessDuration;
	[SerializeField] private bool _isPlayerInjured;
	private BoxCollider _collider;
	private Animator _animator;
	private Camera _camera;
	 
	#endregion

	#region Unity Methods

	private void Awake()
	{
		_camera = Camera.main;
		_collider = GetComponent<BoxCollider>();
		_animator = _camera.GetComponent<Animator>();
	}

	private void OnTriggerEnter(Collider _)
	{
		PostProcessManager._isPostProssessFall = true;
		_animator.SetTrigger("IsFalling");
		PlayerAbilities._mustStandUp = true;
		StartCoroutine(PostprocessCoroutine());
		_collider.enabled = false;

		if (_isPlayerInjured)
		{
			PlayerAbilities._isPlayerInjured = true;
		}
	}

	private IEnumerator PostprocessCoroutine()
	{
		yield return new WaitForSeconds(_postprocessDuration);
		PostProcessManager._isPostProssessFall = false;
	}

	#endregion
}
