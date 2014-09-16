using System;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class Point
	{
		private const double MaxX = 1200;
		private const double MaxY = 800;

		private double _x;
		private double _y;

		public bool Passed = false;

		public double X
		{
			get { return _x; }
		}

		public double Y
		{
			get { return _y; }
		}

		public Point(double x, double y)
		{
			_x = Math.Min(x, MaxX);
			_y = Math.Min(y, MaxY);
		}
	}
}

