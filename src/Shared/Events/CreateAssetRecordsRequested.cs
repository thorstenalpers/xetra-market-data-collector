namespace Shared.Events;

public class CreateAssetRecordsRequested
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> AssetIds { get; set; }
    public List<string> Symbols { get; set; }
}
