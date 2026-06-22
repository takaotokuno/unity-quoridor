namespace Quoridor
{
    public sealed class PlayerRuntimeState
    {
        public bool CanAct { get; private set; }
        public bool CanMove { get; private set; }
        public bool CanPlaceWall { get; private set; }
        public bool CanUseSpecialSkill { get; private set; }
        public bool IsCpu { get; }
        public bool IsAuto { get; private set; }

        public PlayerRuntimeState(
            bool canAct = true,
            bool canMove = true,
            bool canPlaceWall = true,
            bool canUseSpecialSkill = true,
            bool isCpu = false
        )
        {
            CanAct = canAct;
            CanMove = canMove;
            CanPlaceWall = canPlaceWall;
            CanUseSpecialSkill = canUseSpecialSkill;
            IsCpu = isCpu;
            IsAuto = isCpu;
        }

        private PlayerRuntimeState(
            bool canAct,
            bool canMove,
            bool canPlaceWall,
            bool canUseSpecialSkill,
            bool isCpu,
            bool isAuto
        )
        {
            CanAct = canAct;
            CanMove = canMove;
            CanPlaceWall = canPlaceWall;
            CanUseSpecialSkill = canUseSpecialSkill;
            IsCpu = isCpu;
            IsAuto = isAuto;
        }

        public void Reset()
        {
            CanAct = true;
            CanMove = true;
            CanPlaceWall = true;
            CanUseSpecialSkill = true;
            IsAuto = IsCpu;
        }

        public PlayerRuntimeState DeepCopy()
        {
            return new PlayerRuntimeState
            (
                CanAct,
                CanMove,
                CanPlaceWall,
                CanUseSpecialSkill,
                IsCpu,
                IsAuto
            );
        }

        public void SetAuto(bool isAuto)
        {
            IsAuto = isAuto;
        }

        public void ProhibitAction()
        {
            CanAct = false;
        }

        public void ProhibitMove()
        {
            CanMove = false;
        }

        public void ProhibitWallPlacement()
        {
            CanPlaceWall = false;
        }

        public void ProhibitSpecialSkill()
        {
            CanUseSpecialSkill = false;
        }
    }
}
