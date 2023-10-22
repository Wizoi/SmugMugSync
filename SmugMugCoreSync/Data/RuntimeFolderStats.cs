namespace SmugMugCoreSync;

public class RuntimeFolderStats
{
    public RuntimeFolderStats()
    {
        FolderFileStats = new ();
        ProcessedFolders = 0;
        SkippedFolders = 0;
    }

    public int ProcessedFolders;
    public int SkippedFolders;

    private List<RuntimeFolderFileStats> FolderFileStats;

    public RuntimeFolderFileStats StartNewFolderStats()
    {
        var newFolderFileStats = new RuntimeFolderFileStats();
        FolderFileStats.Add(newFolderFileStats);
        return newFolderFileStats;
    }

    public RuntimeFolderFileStats[] RetrieveFolderFileStats()
    {
        return FolderFileStats.ToArray();
    }
}
