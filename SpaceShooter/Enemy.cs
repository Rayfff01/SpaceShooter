using SadConsole;
using SadRogue.Primitives;

namespace SpaceShooter;

public class Enemy : ScreenSurface, IMoveable
{
    private int _ticks;
    private int _speed;     
    public Enemy(int speed) : base(1, 1)
    {
        _speed = speed;
        Surface[0, 0].Glyph = 'V';
    }

    public void Move()
    {
        _ticks++;
        if (_ticks % _speed == 0)
            Position += new Point(0, 1);
    }
}