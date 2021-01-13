using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion gunRotation;
        public CharacterStats state;

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;
            state = CharacterStats.Idle;
        }

        /// <summary>Processes player input and moves the player.</summary>
        public void Update()
        {

        }
    }
}
