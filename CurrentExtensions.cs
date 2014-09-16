using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public static class CurrentExtensions
	{
		public static Dictionary<Type, Object> currents = new Dictionary<Type, Object>();

		public static void SetCurrent<T>(this T me)
		{
			CurrentExtensions.currents[typeof(T)] = me;
		}
	}

	public static class Get<T> where T: class
	{
		public static T Current()
		{
			return CurrentExtensions.currents [typeof(T)] as T;
		}
	}
}


