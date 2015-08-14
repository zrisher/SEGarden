/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;

namespace SEGarden.Utility {

	/// <summary>
	/// Static helper functions
	/// </summary>
	public static class TextFormatting {

		public static String prettySeconds(int seconds) {
			int days = (int)Math.Floor((float)(seconds / 86400));
			if (days > 0)
				return days + " days";

			int hours = (int)Math.Floor((float)(seconds / 3600));
			if (hours > 0)
				return hours + " hours";

			int minutes = (int)Math.Floor((float)(seconds / 60));
			if (minutes > 0)
				return minutes + " minutes";

			return seconds + " seconds";
		}

		public static String prettyDistance(int meters) {
			int km = (int)Math.Floor((float)(meters / 1000));
			if (km > 0)
				return km + "km";

			return meters + "m";
		}



	}

}
*/