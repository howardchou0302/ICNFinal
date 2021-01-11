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
        private bool[] inputs;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;
            state = CharacterStats.Idle;

            inputs = new bool[4];
        }

        /// <summary>Processes player input and moves the player.</summary>
        public void Update()
        {
            Vector3 _inputDirection = Vector3.Zero;
            if (inputs[0])
            {
                _inputDirection.Y += 1;
            }
            if (inputs[1])
            {
                _inputDirection.Y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.X += 1;
            }
            if (inputs[3])
            {
                _inputDirection.X -= 1;
            }

            Move(_inputDirection);
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

        /// <summary>Updates the player input with newly received input.</summary>
        /// <param name="_inputs">The new key inputs.</param>
        /// <param name="_rotation">The new rotation.</param>
        public void SetInput(bool[] _inputs) { inputs = _inputs; }
        
        public void SetGunRotation(Quaternion r) { gunRotation = r;  }
    }
}
