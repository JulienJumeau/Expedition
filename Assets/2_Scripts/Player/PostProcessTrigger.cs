using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessTrigger : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private float _postprocessDuration;
	private BoxCollider _collider;
	

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		PostProcessManager._isRedPostProssessOn = true;
		StartCoroutine(PostprocessCoroutine());
	}

	private IEnumerator PostprocessCoroutine()
	{
		yield return new WaitForSeconds(_postprocessDuration);
		PostProcessManager._isRedPostProssessOn = false;
	}

	#endregion
}
