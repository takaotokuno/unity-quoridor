namespace Quoridor
{
    public sealed class MatchControlPresenter
        : PresenterBase,
          IMatchObserver<MatchReadiedEvent>,
          IMatchObserver<MatchStartedEvent>,
          IMatchObserver<MatchFinishedEvent>,
          IMatchObserver<CheckmateEvent>,
          IMatchObserver<InputReceivedEvent>,
          IEventSubscriber
    {
        private readonly MatchControlView _control;

        // index = ButtonId - 1
        private readonly ButtonViewModel[] _buttonViewModels;

        private IMatchEventBus _eventBus;
        private readonly InteractionStateStore _interactionStateStore;
        private readonly InputStateStore _inputStateStore;

        public MatchControlPresenter(
            MatchControlView control,
            ButtonViewModel[] buttonViewModels,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore
        ) : base()
        {
            _control = control;
            _buttonViewModels = buttonViewModels;
            _interactionStateStore = interactionStateStore;
            _inputStateStore = inputStateStore;
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchReadiedEvent>(this);
            _eventBus.Subscribe<MatchStartedEvent>(this);
            _eventBus.Subscribe<MatchFinishedEvent>(this);
            _eventBus.Subscribe<CheckmateEvent>(this);
            _eventBus.Subscribe<InputReceivedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<MatchReadiedEvent>(this);
            _eventBus.Unsubscribe<MatchStartedEvent>(this);
            _eventBus.Unsubscribe<MatchFinishedEvent>(this);
            _eventBus.Unsubscribe<CheckmateEvent>(this);
            _eventBus.Unsubscribe<InputReceivedEvent>(this);
            _eventBus = null;
        }

        public void Notify(MatchReadiedEvent e)
        {
            _control.Show();
            RefreshButtons();
        }

        public void Notify(MatchStartedEvent e)
        {
            RefreshButtons();
        }

        public void Notify(MatchFinishedEvent e)
        {
            RefreshButtons();
        }

        public void Notify(CheckmateEvent e)
        {
            RefreshButtons();
        }

        public void Notify(InputReceivedEvent e)
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            RefreshButton(ButtonId.Resign);
            RefreshButton(ButtonId.Skip);
        }

        private void RefreshButton(ButtonId buttonId)
        {
            InteractionState buttonState =
                _interactionStateStore.GetButtonState(buttonId);

            ButtonViewModel vm = GetViewModel(buttonId);

            vm.IsVisible = buttonState.IsActive;
            vm.IsValid   = buttonState.IsValid;
            vm.IsDimmed  = buttonState.IsActive && !buttonState.IsValid;

            ApplyInputState(vm, buttonId);
        }

        private void ApplyInputState(ButtonViewModel vm, ButtonId buttonId)
        {
            bool isHovered =
                _inputStateStore.HoveredTarget != null &&
                _inputStateStore.HoveredTarget.ButtonId == buttonId;

            bool isPressed =
                _inputStateStore.PressedTarget != null &&
                _inputStateStore.PressedTarget.ButtonId == buttonId;

            vm.IsHovered = isHovered;
            vm.IsPressed = isPressed;
        }

        private ButtonViewModel GetViewModel(ButtonId buttonId)
        {
            return _buttonViewModels[(int)buttonId - 1];
        }
    }
}
