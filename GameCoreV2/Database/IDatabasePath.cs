namespace GameCore
{
    public interface IDatabasePath
    {
        string ContainerID { get; }

        string[] PathParts { get; }

        string Item { get; }
    }
}
