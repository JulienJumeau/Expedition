using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
	public static bool _isGameOver;
	private static Vector3 _chosenCheckPointPosition;
	private GameObject[] _checkPoints;
	private GameObject _player;

	private void Start()
	{
		_checkPoints = GameObject.FindGameObjectsWithTag("Checkpoints");
		_player = GameObject.FindGameObjectWithTag("Player");

		if (_checkPoints.Length != 0)
		{
			if (_chosenCheckPointPosition == Vector3.zero)
			{
				_chosenCheckPointPosition = _checkPoints[0].transform.position;
				_player.transform.position = _chosenCheckPointPosition;
			}

			else
			{
				_player.transform.position = _chosenCheckPointPosition;
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			SceneManager.LoadScene("SceneLoader");
		}

		if(_isGameOver)
		{
			_isGameOver = false;
			GameOverScene();
		}
	}

	public void GameOverScene()
	{
		SceneManager.LoadScene("GameOver");
	}

	public void OnClickButton(int chapterIndex)
	{
		switch (chapterIndex)
		{
			case 1:
				PlayerAbilities._isActionPlaying = false;
				PlayerAbilities._isLanternInInventory = false;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				SceneManager.LoadScene("SceneLoader");
				break;
			case 2:
				PlayerAbilities._isActionPlaying = false;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				SceneManager.LoadScene("SceneLoader");
				break;
			case 3:
				PlayerAbilities._isActionPlaying = false;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				SceneManager.LoadScene("SceneLoader");
				break;
			default:
				break;
		}
	}
}
