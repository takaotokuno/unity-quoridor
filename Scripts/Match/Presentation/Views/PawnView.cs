using UnityEngine;

namespace Quoridor
{
    public sealed class PawnView : ViewBase
    {
        [SerializeField] private Vector3 _offset = Vector3.zero;

        public void Move(TileView to)
        {
            if (to == null) return;

            Vector3 current = transform.position;
            Vector3 target = to.transform.position + _offset;

            transform.position = new Vector3(
                target.x,
                target.y,
                current.z
            );
        }
    }
}