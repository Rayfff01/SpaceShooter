using SadConsole;
using SadRogue.Primitives;

namespace SpaceShooter;

public class Enemy : ScreenSurface, IMoveable
{
    public Enemy() : base(1, 1)
    {
        Surface[0, 0].Glyph = 'V';
    }

    public void Move()
    {
        Position += new Point(0, 1);
    }
}