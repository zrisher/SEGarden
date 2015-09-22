using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRageMath;

namespace SEGarden.Math {

    /// <summary>
    /// An object that can be stored in multiple generalized AABBTrees
    /// </summary>
    public interface AABBEntity {

        BoundingBoxD BoundingBox { get; }
        Dictionary<long, int> ProxyIdsByTree { get; set; }
        Vector3D WorldTranslation { get; }
        Vector3D LinearVelocity { get; }
        
    }

}
