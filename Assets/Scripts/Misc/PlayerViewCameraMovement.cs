using UnityEngine;

namespace TM.Misc
{
    public static class PlayerViewCameraMovement
    {
        public static Vector3 CameraPosByAngle(Vector3 target, float angle, float downwardAngle, float distance)
        {
            Vector3 xzPos = new Vector3(target.x + Mathf.Cos(Mathf.Deg2Rad * angle) * distance * Mathf.Cos(Mathf.Deg2Rad * downwardAngle), target.y, target.z + Mathf.Sin(Mathf.Deg2Rad * angle) * distance * Mathf.Cos(Mathf.Deg2Rad * downwardAngle));
            float height = Mathf.Sin(Mathf.Deg2Rad * downwardAngle) * distance;
            return new Vector3(xzPos.x, target.y + height, xzPos.z);
        }
    }
}
