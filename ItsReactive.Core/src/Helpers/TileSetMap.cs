using Microsoft.Xna.Framework;

namespace ItsReactive.Core.Helpers;

public static class TileSetMap
{
    public static Vector2 GetTilePosition(TileSetList sprite)
    {
        return sprite switch
        {
            TileSetList.Esc => new Vector2(0, 0),
            TileSetList.EscDown => new Vector2(0, 1),
            TileSetList.Up => new Vector2(1, 0),
            TileSetList.UpDown => new Vector2(1, 1),
            TileSetList.Left => new Vector2(2, 0),
            TileSetList.LeftDown => new Vector2(2, 1),
            TileSetList.Down => new Vector2(3, 0),
            TileSetList.DownDown => new Vector2(3, 1),
            TileSetList.Right => new Vector2(4, 0),
            TileSetList.RightDown => new Vector2(4, 1),
            TileSetList.SpaceBarL => new Vector2(5, 0),
            TileSetList.SpaceBarLDown => new Vector2(5, 1),
            TileSetList.SpaceBarM => new Vector2(6, 0),
            TileSetList.SpaceBarMDown => new Vector2(6, 1),
            TileSetList.SpaceBarR => new Vector2(7, 0),
            TileSetList.SpaceBarRDown => new Vector2(7, 1),
            TileSetList.EnterL => new Vector2(8, 0),
            TileSetList.EnterLDown => new Vector2(8, 1),
            TileSetList.EnterR => new Vector2(9, 0),
            TileSetList.EnterRDown => new Vector2(9, 1),
            TileSetList.PointerUp => new Vector2(0, 2),
            TileSetList.PointerDown => new Vector2(1, 2),
            TileSetList.PointerTwo => new Vector2(2, 2),
            TileSetList.PointerThree => new Vector2(3, 2),
            TileSetList.PointerNone => new Vector2(4, 2),
            TileSetList.PointerLeft => new Vector2(5, 2),
            TileSetList.PointerRight => new Vector2(6, 2),
            TileSetList.PointerAlright => new Vector2(7, 2),
            TileSetList.PointerBad => new Vector2(8, 2),
            TileSetList.Ok => new Vector2(9, 2),
            TileSetList.Error => new Vector2(10, 2),
            TileSetList.UILine => new Vector2(3, 3),
            TileSetList.UILineStreamAnimation1 => new Vector2(4, 3),
            TileSetList.UILineStreamAnimation2 => new Vector2(5, 3),
            TileSetList.UILineStreamAnimation3 => new Vector2(6, 3),
            TileSetList.UILineStreamAnimation4 => new Vector2(7, 3),
            TileSetList.UILineConnected => new Vector2(8, 3),
            TileSetList.UILineDisconnected => new Vector2(9, 3),
            TileSetList.UINodeCorner => new Vector2(11, 0),
            TileSetList.UINodeConnection => new Vector2(11, 1),
            TileSetList.UINodeWall => new Vector2(11, 2),
            TileSetList.UINodeFill => new Vector2(11, 3),
            _ => throw new ArgumentOutOfRangeException(nameof(sprite), sprite, null)
        };
    }
}

public enum TileSetList
{
    Esc,
    EscDown,
    Up,
    UpDown,
    Left,
    LeftDown,
    Down,
    DownDown,
    Right,
    RightDown,
    SpaceBarL,
    SpaceBarLDown,
    SpaceBarM,
    SpaceBarMDown,
    SpaceBarR,
    SpaceBarRDown,
    EnterL,
    EnterLDown,
    EnterR,
    EnterRDown,
    PointerUp,
    PointerDown,
    PointerLeft,
    PointerRight,
    PointerTwo,
    PointerThree,
    PointerNone,
    PointerAlright,
    PointerBad,
    Ok,
    Error,
    UILine,
    UILineStreamAnimation1,
    UILineStreamAnimation2,
    UILineStreamAnimation3,
    UILineStreamAnimation4,
    UILineConnected,
    UILineDisconnected,
    UINodeCorner,
    UINodeConnection,
    UINodeWall,
    UINodeFill
    
}