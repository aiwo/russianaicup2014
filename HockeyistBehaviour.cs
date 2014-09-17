using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public abstract class HockeyistBehaviour
	{
		public HockeyistBehaviour (Hockeyist h)
		{
			this.hId = h.Id;
		}
		long hId;

		public World world { get { return Get<World>.Current (); } }
		public Hockeyist me { get { return world.Hockeyists.First (x => x.Id == hId); }  }

		public const double DistanceDelta = 80.0D;

		public abstract IEnumerable<Action<Move>> Perform ();
	}
}

