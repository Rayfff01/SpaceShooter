using System.Collections;
using SadConsole.Entities;
using SadConsole.Input;

namespace SpaceShooter;

using SadConsole;
using SadRogue.Primitives;

public class GameScreen : ScreenObject
    {
        private Console _console;
        private Player _player;
        private List<Bullet> _bullets;
        private List<Enemy> _enemies;
        private List<Supply> _supplies;
        private List<IMoveable> _moveableEntities;
        private Random _random;
        private bool _isGameOver;
        private int _score = 0;
        private int _health = 1;
        private int _ammo = 10;

        public GameScreen()
        {
            _console = new Console(40, 25);
            Children.Add(_console);

            _player = new Player();
            _player.Position = new Point(20, 22);
            _console.Children.Add(_player);

            _bullets = new List<Bullet>();
            _enemies = new List<Enemy>();
            _supplies = new List<Supply>();
            _random = new Random();
            
            Game.Instance.FrameUpdate += OnFrameUpdate;
        }
        
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (_isGameOver && info.KeysPressed.Count > 0)
            {
                RestartGame();
                return true;
            }
            
            if (info.IsKeyPressed(Keys.Left) && _player.Position.X > 0)
                _player.Position += new Point(-1, 0);
            else if (info.IsKeyPressed(Keys.Right) && _player.Position.X < 39)
                _player.Position += new Point(1, 0);
            
            if (info.IsKeyPressed(Keys.Up) && _player.Position.Y > 0)
                _player.Position += new Point(0, -1);
            else if (info.IsKeyPressed(Keys.Down) && _player.Position.Y < 25)
                _player.Position += new Point(0, 1);
            
            if (info.IsKeyPressed(Keys.Space))
            {
                if (_ammo > 0)
                {
                    var bullet = new Bullet
                    {
                        Position = _player.Position + new Point(0, -1)
                    };
                    
                    _bullets.Add(bullet);
                    _console.Children.Add(bullet);

                    _ammo--;
                }
            }

            return base.ProcessKeyboard(info);
        }

        private void OnFrameUpdate(object? sender, GameHost e)
        {
            if (_isGameOver)
                return;
            
            _console.Clear();
            _console.Print(1, 0, $"Score: {_score}");
            _console.Print(13, 0, $"Ammo: {_ammo}");
            _console.Print(25, 0, $"Health: {_health}");
            
            _moveableEntities = _bullets.Cast<IMoveable>().Concat(_enemies).Concat(_supplies).ToList();
            _moveableEntities.ForEach(x => x.Move());

            foreach (var supply in _supplies.ToArray())
            {
                if (supply.IsExpired())
                {
                    _supplies.Remove(supply);
                    _console.Children.Remove(supply);
                }
            }
            
            SpawnRandomEntities();
            
            UpdateProjectiles();
            UpdateEnemies();

            CheckPlayerEnemyCollisions();
            CheckBulletEnemyCollisions();
            CheckPlayerSupplyCollisions();
        }
        
        private void CheckPlayerEnemyCollisions()
        {
            foreach (var enemy in _enemies.ToArray())
            {
                if (!_player.CheckCollision(enemy)) continue;
        
                RemoveEntity(enemy, _enemies);
                
                _health--;
                
                if (_health <= 0)
                {
                    _isGameOver = true;
                    _console.Clear();
                    _console.Print(15, 10, "GAME OVER", Color.Red, Color.Black);
                    _console.Print(8, 12, "Press any key to restart", Color.Red, Color.Black);
                }
                
                break;
            }
        }
        
        private void CheckBulletEnemyCollisions()
        {
            foreach (var bullet in _bullets.ToArray())
            foreach (var enemy in _enemies.ToArray())
            {
                if (bullet.Position != enemy.Position) continue;
        
                RemoveEntity(bullet, _bullets);
                RemoveEntity(enemy, _enemies);
                _score += 10;
                break;
            }
        }
        
        private void CheckPlayerSupplyCollisions()
        {
            foreach (var supply in _supplies.ToArray())
            {
                if (!_player.CheckCollision(supply)) continue;
        
                RemoveEntity(supply, _supplies);
                _ammo += 5;
                break;
            }
        }

        private void UpdateProjectiles()
        {
            foreach (var bullet in _bullets.ToArray())
            {
                if (bullet.Position.Y < 0)
                    RemoveEntity(bullet, _bullets);
            }
        }
        
        private void UpdateEnemies()
        {
            foreach (var enemy in _enemies.ToArray())
            {
                if (enemy.Position.Y > 24)
                    RemoveEntity(enemy, _enemies);
            }
        }
        
        private void RestartGame()
        {
            _isGameOver = false;
            _health = 3;
            _ammo = 10;
            _score = 0;

            _bullets.Clear();
            _enemies.Clear();
            _supplies.Clear();
            _console.Children.Clear();

            _player.Position = new Point(20, 22);
            _console.Children.Add(_player);
        }


        private void SpawnRandomEntities()
        {
            TrySpawnEntity(1, () =>
            {
                var direction = _random.Next(0, 2);
                
                var position = direction == 0
                    ? new Point(-5, _random.Next(6, 12))
                    : new Point(45, _random.Next(6, 12));
                
                var ammo = new Supply(position.X < 0 ? 1 : -1)
                {
                    Position = position
                };
                
                return (ammo, _supplies);
            });
            
            TrySpawnEntity(50, () =>
            {
                var enemy = new Enemy(_random.Next(1, 5)){ Position = new Point(_random.Next(0, 40), 0) };

                return (enemy, _enemies);
            });
        }
        
        private void TrySpawnEntity(int chancePercent, Func<(ScreenSurface entity, IList collection)> createEntity)
        {
            if (_random.Next(0, 1000) >= chancePercent) return;
    
            var (entity, collection) = createEntity();
            collection.Add(entity);
            _console.Children.Add(entity);
        }

        private void RemoveEntity(ScreenSurface entity, IList collection)
        {
            collection.Remove(entity);
            _console.Children.Remove(entity);
        }
    }