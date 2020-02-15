using System.Collections;
using UnityEngine;

public class PostProcessTrigger : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private float _postprocessDuration;
	[SerializeField] private bool _isPlayerInjured;
	private BoxCollider _collider;
	

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		PostProcessManager._isPostProssessFall = true;
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
