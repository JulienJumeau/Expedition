using UnityEngine;
using UnityEngine.AI;

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	private NavMeshAgent _agent;
	private int _nextDesitnationIndex;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_nextDesitnationIndex = 0;
	}

	private void Start()
	{
		GoToNextPoint();
	}

	private void Update()
	{
		if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			GoToNextPoint();
		}
	}

	#endregion

	#region Foe Patrol

	private void GoToNextPoint()
	{
		if (_patrolPoints.Length == 0)
		{
			Debug.Log("Warning empty array points for partol");
			return;
		}

		_agent.SetDestination(_patrolPoints[_nextDesitnationIndex].position);
		_nextDesitnationIndex = (_nextDesitnationIndex + 1) % _patrolPoints.Length;
	}

	#endregion
}
