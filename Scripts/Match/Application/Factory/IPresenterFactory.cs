namespace Quoridor
{
    public interface IPresenterFactory
    {
        MatchControlPresenter CreateControl();
        TurnPanelPresenter CreateTurnPanel();
        StatusPanelPresenter CreateStatusPanel();
        BoardPresenter CreateBoard();
        SkillButtonPresenter CreateSkillButton();
    }
}