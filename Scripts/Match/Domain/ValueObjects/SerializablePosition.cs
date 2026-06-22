using System;
using UnityEngine;

namespace Quoridor
{
    [Serializable]
    public struct SerializablePosition
    {
        [SerializeField] private int x;
        [SerializeField] private int y;

        public int X => x;
        public int Y => y;

        public Position ToPosition()
        {
            return new Position(x, y);
        }
    }
}
