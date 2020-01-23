using UnityEngine;
using UnityEngine.AI;

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	[SerializeField] private bool _isNextDestinationRandom = false;
	private NavMeshAgent _agent;
	private int _nextDestinationIndex;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_nextDestinationIndex = 0;
	}

	private void Start()
	{
		GoToNextPatrolPoint();
	}

	private void Update()
	{
		if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			GoToNextPatrolPoint();
		}
	}

	#endregion

	#region Foe Patrol

	private void GoToNextPatrolPoint()
	{
		if (_patrolPoints.Length == 0)
		{
			Debug.Log("Warning empty array points for partol");
			return;
		}


		_agent.SetDestination(_patrolPoints[_nextDestinationIndex].position);
		
		if (_isNextDestinationRandom)
		{
			_nextDestinationIndex = Random.Range(0, _patrolPoints.Length);
		}

		else
		{
			_nextDestinationIndex++;
		}

		_nextDestinationIndex = (_nextDestinationIndex) % _patrolPoints.Length;
	}

	#endregion
}
