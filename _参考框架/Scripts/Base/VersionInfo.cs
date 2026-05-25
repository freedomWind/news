using System;

[Serializable]
public class VersionInfo
{
    public bool ForceGameUpdate;

    public string LatestGameVersion;

    public int InternalGameVersion;

    public int InternalResourceVersion;

    public string GameUpdateUrl;
    
    public string ResourceUpdateUrl;

    public int VersionListLength;

    public int VersionListHashCode;

    public int VersionListCompressedLength;

    public int VersionListCompressedHashCode;
}