using UnityEngine;
using UnityEngine.AI;

public class PenguinRunner : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _destinatinationPoints = null;
	private NavMeshAgent _agent;
	private Animator _animator;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponentInChildren<Animator>();
		this.transform.position = _destinatinationPoints[0].position;
	}

	private void Start()
	{
		_animator.SetBool("P_IsRunning", true);
		_agent.SetDestination(_destinatinationPoints[1].position);
	}

	private void Update()
	{
		if (_agent.remainingDistance < 0.1f)
		{
			Destroy(this.transform.parent.gameObject);
		}
	}

	#endregion
}
