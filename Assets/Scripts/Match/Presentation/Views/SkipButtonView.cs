namespace Quoridor
{
    public sealed class SkipButtonView : CanvasButtonViewBase
    {
        protected override ButtonId ButtonId => ButtonId.Skip;

        public override void Highlight()
        {
            // TODO: hover時の見た目
        }

        public override void Emphasize()
        {
            // TODO: 強調表示
        }

        public override void Dim()
        {
            // TODO: 無効・暗転表示
        }

        public override void Press()
        {
            // TODO: 押下中表示
        }

        public override void Clear()
        {
            // TODO: 通常表示
        }

        public override void PlayInvalidFeedback()
        {
            // TODO: 無効入力時の演出
        }
    }
}