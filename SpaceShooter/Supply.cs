using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;


namespace SpaceShooter
{
    public class Supply : ScreenSurface, IMoveable
    {
        private Random _random;
        private int _middleX;
        private int _direction;
        private int _ticks;
        public Supply(int direction) : base(1, 1)
        {
            _random = new Random();
            _middleX = _random.Next(15, 25);
            _direction = direction;
            Surface[0, 0].Glyph = '#';
            Surface[0, 0].Foreground = Color.Purple;
        }
        public void Move()
        {
            _ticks++;
            if (_ticks % 5 != 0)
                return;
            if (Position.X == _middleX)
            {
                return;
            }
            Position += new Point(_direction, 0);
        }
    }
}
