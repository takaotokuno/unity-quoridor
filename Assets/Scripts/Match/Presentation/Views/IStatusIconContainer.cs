namespace Quoridor
{
    public interface IStatusIconContainer
    {
        void AddStatusIcon(StatusId statusId);
        void RemoveStatusIcon(StatusId statusId);
    }
}
