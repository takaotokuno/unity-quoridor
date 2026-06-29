using System.Collections.Generic;

namespace Quoridor
{
    public sealed class BoardPresenter
        : PresenterBase,
          IMatchObserver<MatchReadiedEvent>,
          IEventSubscriber
    {
        private readonly BoardViewModel _viewModel;
        private readonly IReadOnlyList<IMatchPresenter> _presenters;
        private IMatchEventBus _eventBus;

        public BoardPresenter(
            BoardViewModel viewModel,
            IReadOnlyList<IMatchPresenter> presenters
        ) : base()
        {
            _viewModel = Guard.ThrowIfNull(viewModel, nameof(viewModel));
            _presenters = Guard.ThrowIfNull(presenters, nameof(presenters));
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchReadiedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<MatchReadiedEvent>(this);
                _eventBus = null;
            }

            foreach (IMatchPresenter presenter in _presenters)
            {
                presenter.Dispose();
            }
        }

        public void Notify(MatchReadiedEvent e)
        {
            _viewModel.IsVisible = true;
        }
    }
}
