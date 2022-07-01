using ItsReactive.UI.Data.NodeEditorComponent.List;
using Microsoft.Xna.Framework;

namespace ItsReactive.UI.Data.NodeEditorComponent;

public class NodeEditorNodeConnection: NodeEditorDataBase
{
    public NodeEditorNodeConnectionPosition Side { get; }

    public NodeEditorNodeConnection(string name, Rectangle position, NodeEditorNodeConnectionPosition side) : base(name, position)
    {
        Side = side;
    }
}