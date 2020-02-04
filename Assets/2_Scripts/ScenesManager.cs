using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
	public static bool _isGameOver;

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
				SceneManager.LoadScene("SceneLoader");
				break;
			case 2:
				SceneManager.LoadScene("SceneLoader");
				break;
			case 3:
				SceneManager.LoadScene("SceneLoader");
				break;
			default:
				break;
		}
	}
}
