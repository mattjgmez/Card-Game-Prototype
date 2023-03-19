using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoSingleton<HandManager>
{
    [SerializeField] private List<GameObject> objectsToCenter = new List<GameObject>(); // The list of objects to center
    [SerializeField] private Transform _handPosition;// The point to center the objects around
    [SerializeField] private float spacing_X, spacing_Y; // The spacing between each object

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            CenterObjects();
    }

    /// <summary>
    /// Centers the objects around the specified point, with the specified spacing.
    /// </summary>
    private void CenterObjects()
    {
        if (objectsToCenter == null || objectsToCenter.Count == 0)
        {
            Debug.LogWarning("No objects provided to center.");
            return;
        }

        // Calculate the total width of all objects with their spacing
        float totalWidth = (objectsToCenter.Count - 1) * spacing_X;

        // Calculate the starting position of the first object
        Vector3 startPosition = _handPosition.position - new Vector3(totalWidth / 2, 0, 0);

        // Set the position for each object
        for (int i = 0; i < objectsToCenter.Count; i++)
        {
            Vector3 objectPosition = startPosition + new Vector3(i * spacing_X, i * spacing_Y, 0);
            objectsToCenter[i].transform.position = objectPosition;
        }
    }
}
