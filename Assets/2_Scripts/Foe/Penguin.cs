using UnityEngine;
using UnityEngine.AI;

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	[SerializeField] private bool _isNextDestinationRandom = false;
	[SerializeField] private float _detectionRadius = 0, _detectionRadiusWHoldingBreath = 0;
	[SerializeField] private PlayerAbilities _player;
	private NavMeshAgent _agent;
	private int _nextDestinationIndex;
	private Transform _target;
	private float _distanceTargetAgent;
	private bool _isChasingPlayer;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_nextDestinationIndex = 0;
		_distanceTargetAgent = 0;
		_target = _player.transform;
	}

	private void Start()
	{
		GoToNextPatrolPoint();
	}

	private void Update()
	{
		gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _isChasingPlayer ? Color.red : Color.green;

		if (!_agent.pathPending && _agent.remainingDistance < 0.5f && !_isChasingPlayer)
		{
			GoToNextPatrolPoint();
		}

		_distanceTargetAgent = Vector3.Distance(_target.position, transform.position);

		if (_distanceTargetAgent <= _detectionRadius && _player._isHiding == false)
		{
			_agent.SetDestination(_target.position);
			_isChasingPlayer = true;
			Debug.Log("Chasing started!");

			// When Enemy arrives at "StoppingDistance"
			if (_distanceTargetAgent <= _agent.stoppingDistance)
			{
				//Attack the target
				FaceTarget();
			}
		}
		if ((_player._isHiding || _distanceTargetAgent > _detectionRadius) && _isChasingPlayer)
		{
			_agent.SetDestination(transform.position);
			_isChasingPlayer = false;
			Debug.Log("Chasing stopped!");
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

	private void FaceTarget()
	{
		Vector3 direction = (_target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	#endregion
}
