namespace Quoridor
{
    public sealed class BoardCellPresenter
        : PresenterBase,
          IMatchObserver<WallPlacedEvent>,
          IMatchObserver<WallRemovedEvent>,
          IMatchObserver<InputReceivedEvent>,
          IEventSubscriber
    {
        private readonly BoardCellViewModel[,] _viewModels;
        private readonly InteractionStateStore _interactionStateStore;
        private readonly InputStateStore _inputStateStore;
        private IMatchEventBus _eventBus;

        public BoardCellPresenter(
            BoardCellViewModel[,] viewModels,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore
        ) : base()
        {
            _viewModels = Guard.ThrowIfNull(viewModels, nameof(viewModels));
            _interactionStateStore = Guard.ThrowIfNull(interactionStateStore, nameof(interactionStateStore));
            _inputStateStore = Guard.ThrowIfNull(inputStateStore, nameof(inputStateStore));
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<WallPlacedEvent>(this);
            _eventBus.Subscribe<WallRemovedEvent>(this);
            _eventBus.Subscribe<InputReceivedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<WallPlacedEvent>(this);
            _eventBus.Unsubscribe<WallRemovedEvent>(this);
            _eventBus.Unsubscribe<InputReceivedEvent>(this);
            _eventBus = null;
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

            Position pos = target.Position.Value;
            InteractionState state = _interactionStateStore.GetBoardState(pos);
            BoardCellViewModel vm = _viewModels[pos.Y, pos.X];

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
