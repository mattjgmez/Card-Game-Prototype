using UnityEngine;
using System.Collections;

public class SceneInitializer : MonoBehaviour
{
    public IEnumerator InitializeGame()
    {
        Debug.Log("SceneInitialize.InitializeGame: Coroutine started.");

        yield return null;

        Debug.Log("SceneInitializer.InitializeGame: Attempting to Initialize Game.");
        TurnManager.Instance.InitializeGame();
    }

    public void InitializeGameStart()
    {
        Debug.Log("SceneInitialize.InitializeGameStart: Coroutine called.");
        StartCoroutine(InitializeGame());
    }
}
