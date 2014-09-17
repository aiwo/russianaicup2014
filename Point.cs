using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class Point : Unit
	{
		public Point(double x, double y) : base(0, 0, 0, x, y, 0, 0, 0, 0)
		{

		}

		public Point() : this(0, 0)
		{

		}
	}
}

