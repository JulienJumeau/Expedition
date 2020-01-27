using UnityEngine;
using UnityEngine.AI;

public enum FoeState
{
	idle,
	Patrol,
	Chase,
	Attack
}

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	[SerializeField] private bool _isNextDestinationRandom = false;
	[SerializeField] private float _foePatrolSpeed = 0, _foeChaseSpeed = 0;
	[SerializeField] private float _detectionRadius = 0, _detectionRadiusWHoldingBreath = 0;
	[SerializeField] private PlayerAbilities _player;
	private NavMeshAgent _agent;
	private Transform _targetPlayer;
	public static FoeState _foeState;
	private int _nextDestinationIndex;
	private float _distanceTargetAgent, _CurrentDetectionRadius;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_targetPlayer = _player.transform;
		_nextDestinationIndex = 0;
		_distanceTargetAgent = 0;
		_foeState = FoeState.Patrol;
		_CurrentDetectionRadius = _detectionRadius;
	}

	private void Start()
	{
		//GoToNextPatrolPoint();
	}

	private void Update()
	{
		FoePattern();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRadius);
	}

	#endregion

	private void FoePattern()
	{
		//gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _isChasingPlayer ? Color.red : Color.green;

		if (_foeState == FoeState.Patrol && !_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			GoToNextPatrolPoint();
		}

		_distanceTargetAgent = Vector3.Distance(_targetPlayer.position, this.transform.position);


		if (_distanceTargetAgent <= _detectionRadius && _player._isHiding == false)
		{
			_foeState = FoeState.Chase;
			SetFoeAgentProperties(_targetPlayer.position, _foeChaseSpeed, 4, false);

			if (IsFoeNearTarget())
			{
				_foeState = FoeState.Attack;
			}
		}

		else if (_foeState == FoeState.Chase)
		{
			_agent.SetDestination(this.transform.position);
			_foeState = FoeState.Patrol;
		}

		if (_foeState == FoeState.Attack)
		{
			FaceTarget();

			if (!IsFoeNearTarget() || _player._isHiding == true)
			{
				_foeState = FoeState.Chase;
			}
		}
		if (_player._isHoldingBreath && _foeState == FoeState.Patrol)
		{
			_detectionRadius = _detectionRadiusWHoldingBreath;
		}
		else _detectionRadius = _CurrentDetectionRadius;
	}

	private void GoToNextPatrolPoint()
	{
		if (_patrolPoints.Length == 0)
		{
			Debug.Log("Warning empty array points for partol");
			return;
		}

		SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foePatrolSpeed, 0, true);

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
		Vector3 direction = (_targetPlayer.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	private void SetFoeAgentProperties(Vector3 targetPosition, float speed, float stoppingDistance, bool autoBraking)
	{
		_agent.SetDestination(targetPosition);
		_agent.speed = speed;
		_agent.stoppingDistance = stoppingDistance;
		_agent.autoBraking = autoBraking;
	}

	private bool IsFoeNearTarget() => _distanceTargetAgent <= _agent.stoppingDistance;
}
