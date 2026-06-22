namespace Quoridor
{
    public sealed class BoardCellViewModel : ViewModelBase
    {
        /// <summary>Wall 専用: 壁が建設済みかどうか</summary>
        private bool _isBuilt;
        public bool IsBuilt
        {
            get => _isBuilt;
            set { if (_isBuilt == value) return; _isBuilt = value; OnChanged(); }
        }
    }
}
