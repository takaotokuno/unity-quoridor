namespace Quoridor{
    public interface IUserInteractable
    {
        void BindInputPort(MatchInputPort inputPort);
        void Hovered();
        void Pressed();
        void Released();
        void MouseOut();
        void Highlight();
        void Emphasize();
        void Dim();
        void Press();
        void Clear();
        void PlayInvalidFeedback();
    }
}