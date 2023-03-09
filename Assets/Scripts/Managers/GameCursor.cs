using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCursor
{
    private static Plane _gamePlane = new Plane(Vector3.up, 0);

    public static Vector3 WorldPosition
    {
        get
        {
            float distance;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (_gamePlane.Raycast(ray, out distance))
                return ray.GetPoint(distance);

            // Ray did not hit plane
            return Vector3.zero;
        }
    }
}
