using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
	private void Awake()
	{
		SceneManager.LoadScene("Main", LoadSceneMode.Additive);
		SceneManager.LoadScene("LDTuto", LoadSceneMode.Additive);
        SceneManager.LoadScene("LDMaze", LoadSceneMode.Additive);
        SceneManager.LoadScene("LDEscape", LoadSceneMode.Additive);
	}
}
