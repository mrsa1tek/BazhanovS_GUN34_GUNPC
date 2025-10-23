using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEdit : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.Radius);

        Vector3 viewAngleLeft = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.Angle/2);
        Vector3 viewAngleRight = DirectionFromAngle(fov.transform.eulerAngles.y, fov.Angle / 2);

        Handles.color = Color.yellow;

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov.Radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov.Radius);


        if (fov.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.Player.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegress)
    {
        angleInDegress += eulerY;
        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, 
            Mathf.Cos((angleInDegress * Mathf.Deg2Rad)));

    }
}
