using UnityEngine;
using UnityEngine.UI;

namespace Quoridor
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public sealed class StatusIconRootView : ViewBase
    {
        private HorizontalLayoutGroup _layoutGroup;

        protected override void OnInitialize()
        {
            _layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (_layoutGroup == null)
            {
                Debug.LogError($"{nameof(StatusIconRootView)} requires {nameof(HorizontalLayoutGroup)}.");
            }
        }
    }
}