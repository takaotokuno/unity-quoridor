namespace Quoridor
{
    public sealed class InputStateStore
    {
        public InputTarget HoveredTarget { get; private set; }
        public InputTarget PressedTarget { get; private set; }
        public InputTarget CapturedTarget { get; private set; }

        public void ApplyIntent(InputTarget target, InputIntent intent)
        {
            switch (intent)
            {
                case InputIntent.Hovered:
                    HoveredTarget = target;
                    break;
                
                case InputIntent.Pressed:
                    PressedTarget = target;
                    CapturedTarget = target;
                    break;

                case InputIntent.Released:
                    PressedTarget = null;
                    CapturedTarget = null;
                    break;

                case InputIntent.MouseOut:
                    if(HoveredTarget != null && HoveredTarget.Equals(target))
                    {
                        HoveredTarget = null;
                    }
                    break;
            }   
        }
    }
}