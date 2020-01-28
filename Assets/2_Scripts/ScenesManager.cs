using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ScenesManager : MonoBehaviour
{
	public static bool _isGameOver;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			EditorApplication.isPlaying = false;
		}

		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			SceneManager.LoadScene("Dev");
		}

		if(_isGameOver)
		{
			GameOverScene();
		}
	}

	public void GameOverScene()
	{
		SceneManager.LoadScene("GameOver");
	}
}
