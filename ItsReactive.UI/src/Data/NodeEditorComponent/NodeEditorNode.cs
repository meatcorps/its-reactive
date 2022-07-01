using Microsoft.Xna.Framework;

namespace ItsReactive.UI.Data.NodeEditorComponent;

public class NodeEditorNode : NodeEditorDataBase
{
    public List<NodeEditorNodeConnection> Connections = new ();
    public CancellationTokenRegistration _cancellationToken = new ();

    public NodeEditorNode(string name, Rectangle position, IEnumerable<NodeEditorNodeConnection> emptyConnections) : base(name, position)
    {
        Connections.AddRange(emptyConnections);

        OnPositionChange.Subscribe((tuple => UpdatePositionConnection(tuple.from, tuple.to)), _cancellationToken.Token);
    }

    private void UpdatePositionConnection(Rectangle from, Rectangle to)
    {
        var offset = to.Location - from.Location;
        foreach (var connection in Connections)
        {
            var connectionPosition = connection.Position;
            connectionPosition.Location += offset;
            connection.Position = connectionPosition;
        }
    }

    protected override void OnDispose()
    {
        _cancellationToken.Dispose();
        foreach (var connection in Connections)
        {
            connection.Dispose();
        }
    }
}