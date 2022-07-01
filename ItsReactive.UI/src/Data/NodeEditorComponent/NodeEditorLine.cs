using System;
using Microsoft.Xna.Framework;

namespace ItsReactive.UI.Data.NodeEditorComponent;

public class NodeEditorLine: NodeEditorDataBase
{
    public NodeEditorNodeConnection[] Connections { get; }
    public CancellationTokenRegistration _cancellationToken = new ();

    public NodeEditorLine(string name, NodeEditorNodeConnection connection1, NodeEditorNodeConnection connection2) : base(name, Rectangle.Union(connection1.Position, connection2.Position))
    {
        Connections = new[] {connection1, connection2};

        foreach (var connection in Connections)
        {
            connection.OnPositionChange.Subscribe(_ => UpdatePosition(), _cancellationToken.Token);
            connection.OnDisposed.Subscribe(_ => Dispose(), _cancellationToken.Token);
        }
    }

    private void UpdatePosition()
    {
        Position = Rectangle.Union(Connections[0].Position, Connections[1].Position);
    }

    protected override void OnDispose()
    {
        _cancellationToken.Dispose();
    }
}