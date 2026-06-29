using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public sealed partial class PresenterFactory
    {
        public BoardPresenter CreateBoard()
        {
            TileView tilePrefab = presenterSetting.TilePrefab;
            WallView wallPrefabVertical = presenterSetting.WallPrefabVertical;
            WallView wallPrefabHorizontal = presenterSetting.WallPrefabHorizontal;
            WallJointView wallPrefabJoint = presenterSetting.WallPrefabJoint;
            PawnView pawnPrefabFirst = presenterSetting.PawnPrefabFirst;
            PawnView pawnPrefabSecond = presenterSetting.PawnPrefabSecond;

            BoardView boardView = layout.BoardView;
            Transform root = boardView.transform;

            int boardSize = _setting.BoardSize;
            BoardCellViewModel[,] cellViewModels = new BoardCellViewModel[boardSize, boardSize];

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    BoardCellViewModel vm = new();
                    cellViewModels[y, x] = vm;

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        Vector3 worldPos = layout.GetTilePosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        TileView view = _CreateView(tilePrefab, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                    }
                    else if (x % 2 == 1 && y % 2 == 0)
                    {
                        Vector3 worldPos = layout.GetVerticalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        WallView view = _CreateView(wallPrefabVertical, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                    }
                    else if (x % 2 == 0 && y % 2 == 1)
                    {
                        Vector3 worldPos = layout.GetHorizontalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        WallView view = _CreateView(wallPrefabHorizontal, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                    }
                    else // x % 2 == 1 && y % 2 == 1
                    {
                        Vector3 worldPos = layout.GetHorizontalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        WallJointView view = _CreateView(wallPrefabJoint, localPos, root);
                        view.BindViewModel(vm);
                    }
                }
            }

            Position[] initPawns = _setting.InitPawns;
            PawnViewModel[] pawnViewModels =
            {
                new PawnViewModel(initPawns[0]),
                new PawnViewModel(initPawns[1]),
            };

            CreatePawnView(pawnPrefabFirst, pawnViewModels[0], root);
            CreatePawnView(pawnPrefabSecond, pawnViewModels[1], root);

            BoardViewModel boardViewModel = new();
            boardView.BindViewModel(boardViewModel);

            BoardCellPresenter cellPresenter = new(
                cellViewModels,
                _interactionStateStore,
                _inputStateStore
            );
            BoardPawnPresenter pawnPresenter = new(pawnViewModels);

            cellPresenter.SubscribeTo(_eventBus);
            pawnPresenter.SubscribeTo(_eventBus);

            BoardPresenter presenter = new(
                boardViewModel,
                new List<IMatchPresenter>
                {
                    cellPresenter,
                    pawnPresenter,
                }
            );
            presenter.SubscribeTo(_eventBus);

            return presenter;
        }

        private PawnView CreatePawnView(
            PawnView prefab,
            PawnViewModel viewModel,
            Transform root
        )
        {
            Vector3 localPos = root.InverseTransformPoint(layout.GetPawnPosition(viewModel.Position));
            PawnView view = _CreateView(prefab, localPos, root);
            view.BindPositionResolver(layout.GetPawnPosition);
            view.BindViewModel(viewModel);
            return view;
        }
    }
}
