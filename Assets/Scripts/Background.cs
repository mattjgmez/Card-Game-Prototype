using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.Advance += ScrollBackground;
    }

    private void OnDisable()
    {
        GameManager.Instance.Advance -= ScrollBackground;
    }

    private void ScrollBackground(GameState state)
    {
        StartCoroutine(ScrollBackgroundCoroutine());
    }

    private IEnumerator ScrollBackgroundCoroutine()
    {
        bool isPlayer1Turn = GameManager.Instance.CurrentTurn == GameState.Player1Turn;

        int direction = isPlayer1Turn ? -1 : 1;
        Vector3 targetPosition = transform.position + new Vector3(direction, 0);

        while (isPlayer1Turn ? transform.position.x > targetPosition.x : transform.position.x < targetPosition.x)
        {
            transform.Translate(2 * (isPlayer1Turn ? -transform.right : transform.right) * Time.deltaTime);
            yield return null;
        }
    }
}
