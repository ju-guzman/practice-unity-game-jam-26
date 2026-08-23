using UnityEngine;

namespace Utils
{
    public static class ScreenToWorld
    {
        public static Vector2 FromMouse(Camera camera, Vector2 screenPosition, float z = 0f)
        {
            var world = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, camera.nearClipPlane));
            world.z = z;
            return world;
        }
    }
}
