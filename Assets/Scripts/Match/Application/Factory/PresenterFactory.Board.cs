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
            TileView[,] tileViews = new TileView[boardSize, boardSize];
            WallView[,] wallViews = new WallView[boardSize, boardSize];
            WallJointView[,] wallJointViews = new WallJointView[boardSize, boardSize];

            BoardCellViewModel[,] viewModels = new BoardCellViewModel[boardSize, boardSize];

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    var vm = new BoardCellViewModel();
                    viewModels[y, x] = vm;

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        Vector3 worldPos = layout.GetTilePosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        var view = _CreateView(tilePrefab, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                        tileViews[y, x] = view;
                    }
                    else if (x % 2 == 1 && y % 2 == 0)
                    {
                        Vector3 worldPos = layout.GetVerticalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        var view = _CreateView(wallPrefabVertical, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                        wallViews[y, x] = view;
                    }
                    else if (x % 2 == 0 && y % 2 == 1)
                    {
                        Vector3 worldPos = layout.GetHorizontalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        var view = _CreateView(wallPrefabHorizontal, localPos, root);
                        view.SetPosition(new Position(x, y));
                        view.BindInputPort(_inputPort);
                        view.BindViewModel(vm);
                        wallViews[y, x] = view;
                    }
                    else // x % 2 == 1 && y % 2 == 1
                    {
                        Vector3 worldPos = layout.GetHorizontalWallPosition(x, y);
                        Vector3 localPos = root.InverseTransformPoint(worldPos);
                        var view = _CreateView(wallPrefabJoint, localPos, root);
                        view.BindViewModel(vm);
                        wallJointViews[y, x] = view;
                    }
                }
            }

            var initPawns = _setting.InitPawns;
            var pawnViews = new PawnView[2];
            var pawnFirstLocalPos = root.InverseTransformPoint(layout.GetPawnPosition(initPawns[0]));
            var pawnSecondLocalPos = root.InverseTransformPoint(layout.GetPawnPosition(initPawns[1]));
            pawnViews[0] = _CreateView(pawnPrefabFirst, pawnFirstLocalPos, root);
            pawnViews[1] = _CreateView(pawnPrefabSecond, pawnSecondLocalPos, root);

            var presenter = new BoardPresenter(
                boardView,
                tileViews,
                wallViews,
                wallJointViews,
                pawnViews,
                viewModels,
                _interactionStateStore,
                _inputStateStore
            );

            presenter.SubscribeTo(_eventBus);

            return presenter;
        }
    }   
}