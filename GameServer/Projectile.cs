using System.Numerics;

namespace GameServer
{
    class Projectile
    {
        public int id;
        public Vector3 position;
        public Quaternion rotation;

        public Projectile(int _id, Vector3 _spawnPosition, Quaternion _gunRotation)
        {
            id = _id;
            position = _spawnPosition;
            rotation = _gunRotation;
        }
    }
}