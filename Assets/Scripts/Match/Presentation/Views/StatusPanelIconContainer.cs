using System.Collections.Generic;

namespace Quoridor
{
    public sealed class StatusPanelIconContainer : IStatusIconContainer
    {
        private readonly StatusPanelView _panel;
        private readonly StatusIconView _prefab;
        private readonly StatusViewCatalog _catalog;
        private readonly Dictionary<StatusId, StatusIconView> _icons = new();

        public StatusPanelIconContainer(
            StatusPanelView panel,
            StatusIconView prefab,
            StatusViewCatalog catalog
        )
        {
            _panel = Guard.ThrowIfNull(panel, nameof(panel));
            _prefab = Guard.ThrowIfNull(prefab, nameof(prefab));
            _catalog = Guard.ThrowIfNull(catalog, nameof(catalog));
        }

        public void AddStatusIcon(StatusId statusId)
        {
            StatusViewEntry entry = _catalog.Find(statusId);
            StatusIconView view = _panel.AddStatusIcon(_prefab, entry);

            if (view == null)
            {
                return;
            }

            _icons[statusId] = view;
        }

        public void RemoveStatusIcon(StatusId statusId)
        {
            if (!_icons.TryGetValue(statusId, out StatusIconView view))
            {
                return;
            }

            _icons.Remove(statusId);
            _panel.RemoveStatusIcon(view);
        }
    }
}
