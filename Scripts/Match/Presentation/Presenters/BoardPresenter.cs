using UnityEngine;

namespace Quoridor
{
    public sealed class BoardPresenter 
        : PresenterBase, 
          IMatchObserver<MatchReadiedEvent>,
          IMatchObserver<PawnMovedEvent>,
          IMatchObserver<WallPlacedEvent>,
          IMatchObserver<WallRemovedEvent>,
          IMatchObserver<InputReceivedEvent>, 
          IEventSubscriber
    {
        private readonly BoardView _board;
        private readonly TileView[,] _tiles;
        private readonly WallView[,] _walls;
        private readonly WallJointView[,] _wallJoints;
        private readonly PawnView[] _pawns;
        private readonly BoardCellViewModel[,] _viewModels;
        private IMatchEventBus _eventBus;
        private readonly InteractionStateStore _interactionStateStore;
        private readonly InputStateStore _inputStateStore;

        public BoardPresenter(
            BoardView board,
            TileView[,] tiles,
            WallView[,] walls,
            WallJointView[,] wallJoints,
            PawnView[] pawns,
            BoardCellViewModel[,] viewModels,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore
        ) : base()
        {
            _board = board;
            _tiles = tiles;
            _walls = walls;
            _wallJoints = wallJoints;
            _pawns = pawns;
            _viewModels = viewModels;
            _interactionStateStore = interactionStateStore;
            _inputStateStore = inputStateStore;
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchReadiedEvent>(this);
            _eventBus.Subscribe<PawnMovedEvent>(this);
            _eventBus.Subscribe<WallPlacedEvent>(this);
            _eventBus.Subscribe<WallRemovedEvent>(this);
            _eventBus.Subscribe<InputReceivedEvent>(this);
        }

        public override void Dispose()
        {
            _eventBus.Unsubscribe<MatchReadiedEvent>(this);
            _eventBus.Unsubscribe<PawnMovedEvent>(this);
            _eventBus.Unsubscribe<WallPlacedEvent>(this);
            _eventBus.Unsubscribe<WallRemovedEvent>(this);
            _eventBus.Unsubscribe<InputReceivedEvent>(this);

            Object.Destroy(_board);
        }

        public void Notify(MatchReadiedEvent e)
        {
            _board.PlayShow();
        }

        public void Notify(PawnMovedEvent e)
        {
            var pos = e.Target;
            var tile = _tiles[pos.Y, pos.X];
            _pawns[e.PlayerId.ToIndex()].Move(tile);
        }

        public void Notify(WallPlacedEvent e)
        {
            foreach (Position pos in e.Targets)
            {
                _viewModels[pos.Y, pos.X].IsBuilt = true;
            }
        }

        public void Notify(WallRemovedEvent e)
        {
            foreach (Position pos in e.Targets)
            {
                _viewModels[pos.Y, pos.X].IsBuilt = false;
            }
        }

        public void Notify(InputReceivedEvent e)
        {
            UpdateViewModel(e.Target, e.Intent);
            UpdateViewModel(_inputStateStore.HoveredTarget, InputIntent.Hovered);
            UpdateViewModel(_inputStateStore.PressedTarget, InputIntent.Pressed);
        }

        private void UpdateViewModel(InputTarget target, InputIntent intent)
        {
            if (target == null) return;
            if (!IsBoardKind(target)) return;

            var pos = target.Position.Value;
            var state = _interactionStateStore.GetBoardState(pos);
            var vm = _viewModels[pos.Y, pos.X];

            switch (intent)
            {
                case InputIntent.Hovered:
                    vm.IsValid = state.IsValid;
                    vm.IsHovered = true;
                    break;

                case InputIntent.Pressed:
                    vm.IsValid = state.IsValid;
                    vm.IsPressed = true;
                    break;

                case InputIntent.Released:
                    vm.IsHovered = false;
                    vm.IsPressed = false;
                    break;

                case InputIntent.MouseOut:
                    vm.IsHovered = false;
                    vm.IsPressed = false;
                    break;
            }
        }

        private bool IsBoardKind(InputTarget target)
        {
            return target.Kind == InputTargetKind.Tile || target.Kind == InputTargetKind.Wall;
        }
    }
}
