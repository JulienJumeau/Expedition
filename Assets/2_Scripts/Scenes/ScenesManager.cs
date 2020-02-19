using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour
{
	[SerializeField] private GameObject _hudFadeOutGO;
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

	public void Quit()
	{
		Application.Quit();
		//UnityEditor.EditorApplication.isPlaying = false;
	}

	public void OnClickButton(int chapterIndex)
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		switch (chapterIndex)
		{
			case 0:
				StartCoroutine(LoadChapter(false, false, chapterIndex, true));
				break;
			case 1:
				StartCoroutine(LoadChapter(false, false, chapterIndex - 1));
				break;
			case 2:
			case 3:
				StartCoroutine(LoadChapter(true, true, chapterIndex - 1));
				break;
			default:
				break;
		}
	}

	private IEnumerator LoadChapter(bool isLanternInInventory, bool isOilFull, int chapterIndex, bool isMainMenu = false)
	{
		if (isMainMenu)
		{
			StartCoroutine(SoundManager.FadeoutMusic(FindObjectOfType<AudioSource>(), 2));
		}

		StartCoroutine(HudManager.Fade(_hudFadeOutGO, false, 2));
		yield return new WaitForSeconds(2);
		PlayerAbilities._isActionPlaying = false;
		PlayerAbilities._isLanternInInventory = isLanternInInventory;
		PlayerAbilities._isTheBeginning = !isLanternInInventory;
		PlayerAbilities._oilLevel = isOilFull ? 1 : 0;
		PlayerAbilities._isPlayerInjured = false;

		if (isMainMenu)
		{
			_chosenCheckPointPosition = Vector3.zero;
			SceneManager.LoadScene("Intro");
		}

		else
		{
			_chosenCheckPointPosition = _checkPoints[chapterIndex].transform.position;
			_chosenCheckPointRotation.y = _checkPoints[chapterIndex].transform.eulerAngles.y;
			SceneManager.LoadScene("SceneLoader");
		}
	}
}
