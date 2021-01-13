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

        /// <summary>Calculates the player's desired movement direction and moves him.</summary>
        /// <param name="_inputDirection"></param>
        private void Move(Vector3 _inputDirection)
        {
            Vector3 _moveDirection = Vector3.Normalize(_inputDirection);
            position += _moveDirection * moveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.GunRotation(this);
        }
        public void SetPos(Vector3 pos){ position = pos; }
        
        public void SetGunRotation(Quaternion r) { gunRotation = r;  }
    }
}
