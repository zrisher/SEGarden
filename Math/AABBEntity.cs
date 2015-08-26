using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRageMath;

namespace SEGarden.Math {

    /// <summary>
    /// An object that can be stored in a generalized AABBTree
    /// </summary>
    public interface AABBEntity {

        BoundingBoxD BoundingBox { get; }
        int TreeProxyID { get; set; }
        Vector3D WorldTranslation { get; }
        Vector3D LinearVelocity { get; }
        
    }

}
