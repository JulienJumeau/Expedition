using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
	public static bool _isGameOver;
	private static Vector3 _chosenCheckPointPosition;
	private static Vector3 _chosenCheckPointRotation;
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
				_chosenCheckPointRotation.y = _checkPoints[0].transform.eulerAngles.y;
				_player.transform.position = _chosenCheckPointPosition;
				_player.transform.Rotate(new Vector3(_player.transform.eulerAngles.x, _chosenCheckPointRotation.y, _player.transform.eulerAngles.z));
			}

			else
			{
				_player.transform.position = _chosenCheckPointPosition;
				_player.transform.Rotate(new Vector3(_player.transform.eulerAngles.x, _chosenCheckPointRotation.y, _player.transform.eulerAngles.z));
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			SceneManager.LoadScene("SceneLoader");
		}

		if (_isGameOver)
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
				PlayerAbilities._oilLevel = 0;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				_chosenCheckPointRotation.y = _checkPoints[chapterIndex - 1].transform.eulerAngles.y;
				SceneManager.LoadScene("SceneLoader");
				break;
			case 2:
				PlayerAbilities._isActionPlaying = false;
				PlayerAbilities._isLanternInInventory = true;
				PlayerAbilities._oilLevel = 1;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				_chosenCheckPointRotation.y = _checkPoints[chapterIndex - 1].transform.eulerAngles.y;
				SceneManager.LoadScene("SceneLoader");
				break;
			case 3:
				PlayerAbilities._isActionPlaying = false;
				PlayerAbilities._isLanternInInventory = true;
				PlayerAbilities._oilLevel = 1;
				_chosenCheckPointPosition = _checkPoints[chapterIndex - 1].transform.position;
				_chosenCheckPointRotation.y = _checkPoints[chapterIndex - 1].transform.eulerAngles.y;
				SceneManager.LoadScene("SceneLoader");
				break;
			default:
				break;
		}
	}
}
