using System.Numerics;

namespace GameServer
{
    class Bomb
    {
        public int id;
        public Vector2 position;

        public Bomb(int _id, Vector2 _spawnPosition)
        {
            id = _id;
            position = _spawnPosition;
        }
    }
}