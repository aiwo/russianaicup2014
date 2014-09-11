using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk {
    
    public sealed class MyStrategy : IStrategy {
        private const double StrikeAngle = 1.0D*Math.PI/180.0D;
        private const double StrikeDistanceDelta = 50.0D;

        private static World _world;
        private static Hockeyist _self;
        private static Game _game;
        private static Move _move;

        private static PID anglePID;
        private static PID speedPID;

        public void Move(Hockeyist self, World world, Game game, Move move)
        {
            _world = world;
            _self = self;
            _game = game;
            _move = move;

            const double speedKp = 0.005;
            speedPID = new PID(speedKp, 1.0D);

            // TODO: Angle Max Output
            /*
            const double angleKp = 0.5;
            anglePID = new PID(angleKp, 1.0D);
             */

            InitStrikePoints();

            if (self.State == HockeyistState.Swinging) 
            {
                move.Action = ActionType.Strike;
                return;
            }

            if (IsHunter())
            {
                if (world.Puck.OwnerHockeyistId == self.Id)
                {
                    PassThePuck();
                }
                else
                {
                    GoGetThePuck();
                }    
            }
            else
            {
                TakeStrikePosition();
            }
        }

        private static Point[] strikePoints;

        private static void InitStrikePoints()
        {
            strikePoints = new Point[]
            {
                new Point(212, 580),
                new Point(212, 346)
            };
        }

        private static void TakeStrikePosition()
        {
            if (_world.Puck.OwnerHockeyistId == _self.Id)
            {
                PerformAttack();
                return;
            }

            Point point = strikePoints[0];
            double distance = Hypot(_self.X - point.X, _self.Y - point.Y);
            
            if (distance < StrikeDistanceDelta)
            {
                _move.Turn = _self.GetAngleTo(_world.Puck);
                _move.Action = ActionType.TakePuck;
            }
            else
            {
                double angle = _self.GetAngleTo(point.X, point.Y);

                _move.Turn = angle;

                if (Math.Abs(angle) < StrikeAngle)
                {
                    _move.SpeedUp = speedPID.ValueForError(distance);
                }
            }
        }

        
        private static bool IsHunter()
        {
            Hockeyist teammate =
                _world.Hockeyists.First(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.Id != _self.Id);
            return _self.X > teammate.X;
        }

        private static void GoGetThePuck()
        {
            double distance = _self.GetDistanceTo(_world.Puck);
            _move.SpeedUp = speedPID.ValueForError(distance);

            _move.Turn = _self.GetAngleTo(_world.Puck);

            if (_world.Tick % 2 == 0)
            {
                Hockeyist nearestOpponent = NearestOpponent(_self.X, _self.Y, _world);
                if (nearestOpponent != null)
                {
                    if (_self.GetDistanceTo(nearestOpponent) <= _game.StickLength 
                        && Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
                    {
                        _move.Action = ActionType.Strike;
                    }
                }
            }
            else
            {
                _move.Action = ActionType.TakePuck;
            }
        }

        private static double GetInterceptAngleToUnit(Unit unit)
        {
            double x = Math.Abs(_self.X - unit.X) * _self.SpeedX / (_self.SpeedX - unit.SpeedX);
            double y = Math.Abs(_self.Y - unit.Y) * _self.SpeedY / (_self.SpeedY - unit.SpeedY);

            return _self.GetAngleTo(-x, y);
        }

        private static void PassThePuck()
        {
            Hockeyist striker = _world.Hockeyists.First(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.Id != _self.Id);

            double angle = _self.GetAngleTo(striker);
            _move.Turn = angle;

            if (Math.Abs(angle) < StrikeAngle)
            {
                _move.Action = ActionType.Strike;
            }
            /*
            if (_world.Puck.OwnerHockeyistId == _self.Id)
            {
                PerformAttack();
            }
            else
            {
                PerformSupport();
            }
             */
        }

        private static void PerformAttack()
        {
            Player opponentPlayer = _world.GetOpponentPlayer();

            double netX = 0.5D * (opponentPlayer.NetBack + opponentPlayer.NetFront);
            double netY = 0.5D * (opponentPlayer.NetBottom + opponentPlayer.NetTop);
            netY += (_self.Y < netY ? 0.5D : -0.5D) * _game.GoalNetHeight;

            double angleToNet = _self.GetAngleTo(netX, netY);
            _move.Turn = angleToNet;

            if (Math.Abs(angleToNet) < StrikeAngle)
            {
                _move.Action = ActionType.Swing;
            }
        }

        private static void PerformSupport()
        {
            Hockeyist nearestOpponent = NearestOpponent(_self.X, _self.Y, _world);
            if (nearestOpponent != null)
            {
                if (_self.GetDistanceTo(nearestOpponent) > _game.StickLength)
                {
                    _move.SpeedUp = 1.0D;
                }
                else if (Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
                {
                    _move.Action = ActionType.Strike;
                }
                _move.Turn = _self.GetAngleTo(nearestOpponent);
            }
        }

        private static Hockeyist NearestOpponent(double x, double y, World world) {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.Hockeyists) {
                if (hockeyist.IsTeammate 
                    || hockeyist.Type == HockeyistType.Goalie 
                    || hockeyist.State == HockeyistState.KnockedDown
                    || hockeyist.State == HockeyistState.Resting) {
                    continue;
                }

                double opponentRange = Hypot(x - hockeyist.X, y - hockeyist.Y);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange) {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent;
        }

        private static double Hypot(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }
    }

    
}