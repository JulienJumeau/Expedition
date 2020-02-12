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
		EventSubcription();
	}

	private void Update()
	{
		if (_agent.enabled && _agent.remainingDistance < 0.1f)
		{
			FindObjectOfType<InputManager>().OnPause -= Penguin_OnPause;
			Destroy(this.transform.parent.gameObject);
		}
	}

	#endregion

	private void EventSubcription()
	{
		FindObjectOfType<InputManager>().OnPause += Penguin_OnPause;
	}

	private void Penguin_OnPause(object sender, InputManager.PauseEventArgs e)
	{
		if (e.isPaused)
		{
			_animator.speed = 0;
			_agent.enabled = !e.isPaused;
		}

		else
		{
			_animator.speed = 1;
			_agent.enabled = !e.isPaused;
			_agent.SetDestination(_destinatinationPoints[1].position);
		}
	}
}
