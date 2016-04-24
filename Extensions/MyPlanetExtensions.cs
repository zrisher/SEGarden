using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace SEGarden.Extensions {

    public static class MyPlanetExtensions {

		public static MyPlanet GetClosestPlanet(Vector3D position)
		{
			double distSquared;
			return GetClosestPlanet(position, out distSquared);
		}

		public static MyPlanet GetClosestPlanet(Vector3D position, out double distSquared)
		{
			IMyVoxelBase closest = null;
			double bestDistance = double.MaxValue;
			MyAPIGateway.Session.VoxelMaps.GetInstances(null, voxel => {
					if (voxel is MyPlanet) {
						double distance = Vector3D.DistanceSquared(
                            position, voxel.WorldMatrix.Translation
                        );
						if (distance < bestDistance) {
							bestDistance = distance;
							closest = voxel;
						}
					}
				return false;
			});

			distSquared = bestDistance;
			return (MyPlanet)closest;
		}

    }

}
