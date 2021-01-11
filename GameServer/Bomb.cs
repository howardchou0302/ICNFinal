using System.Numerics;

namespace GameServer
{
    class Bomb
    {
        public int id;
        public Vector3 position;

        public Bomb(int _id, Vector3 _spawnPosition)
        {
            id = _id;
            position = _spawnPosition;
        }
    }
}