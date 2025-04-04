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
        Surface[0, 0].Foreground = speed switch
        {
            1 => Color.Red,
            2 or 3 => Color.Orange,
            4 or 5 => Color.Yellow,
            _ => Surface[0, 0].Foreground
        };

    }

    public void Move()
    {
        _ticks++;
        if (_ticks % _speed == 0)
            Position += new Point(0, 1);
    }
}