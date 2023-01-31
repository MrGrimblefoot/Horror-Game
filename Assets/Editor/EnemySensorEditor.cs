using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySensor))]
public class EnemySensorEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemySensor sensor = (EnemySensor)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up, Vector3.forward, 360, sensor.radius);

        Vector3 viewAngle1 = DirectionFromAngle(sensor.transform.eulerAngles.y, -sensor.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(sensor.transform.eulerAngles.y, sensor.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngle1 * sensor.radius);
        Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngle2 * sensor.radius);

        if (sensor.canSeePlayer)
        {
            Handles.color = Color.red;
            Handles.DrawLine(sensor.transform.position, sensor.playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
