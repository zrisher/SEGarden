using System;
using System.Collections.Generic;

using VRage.Game;
using VRageMath;

using MyDynamicAABBTree = VRageMath.MyDynamicAABBTree;

///<remarks>
/// Taken from the static SE class MyGamePruningStructure, adapted for flexible use
///</remarks>
namespace SEGarden.Math {

    using Logging;

    public class AABBTree {

        private static readonly Vector3D AABB_EXTENSION = new Vector3D(3.0f);
        private static readonly Logger Log = new Logger("SEGarden.Math.AABBTree");
        private const int PROXY_ID_UNITIALIZED = -1;

        private static int LastAssignedTreeId = 0;

        private MyDynamicAABBTreeD MathTree;
        private int TreeId;

        public AABBTree() {
            Init();
        }

        /*
        private MyDynamicAABBTreeD GetPrunningStructure() {
            return m_aabbTree;
        }
        */

        private void Init() {
            MathTree = new MyDynamicAABBTreeD(AABB_EXTENSION);
            TreeId = LastAssignedTreeId + 1;
            LastAssignedTreeId = TreeId;
        }

        public BoundingBoxD GetEntityAABB(AABBEntity entity) {
            BoundingBoxD bbox = entity.BoundingBox;

            //Include entity velocity to be able to hit fast moving objects
            if (entity.WorldTranslation != null && entity.LinearVelocity != null) {
                bbox = bbox.Include(
                    entity.WorldTranslation + 
                    entity.LinearVelocity * 
                    MyEngineConstants.UPDATE_STEP_SIZE_IN_SECONDS * 5
                    );
            }

            return bbox;
        }

        public void Add(AABBEntity entity) {
            int proxyId = GetProxyIdForEntity(entity);
            Log.Trace("Add entity " + proxyId, "Add");

            BoundingBoxD bbox = GetEntityAABB(entity);
            if (bbox.Size == Vector3D.Zero) return;  // don't add entities with zero bounding boxes

            int newProxyId = MathTree.AddProxy(ref bbox, entity, 0);
            SetProxyIdForEntity(entity, newProxyId);

            Log.Trace("Finished Add entity " + newProxyId, "Add");
        }



        public void Remove(AABBEntity entity) {
            int proxyId = GetProxyIdForEntity(entity);
            Log.Trace("Remove entity " + proxyId, "Remove");

            if (proxyId != PROXY_ID_UNITIALIZED) {
                MathTree.RemoveProxy(proxyId);
                SetProxyIdForEntity(entity, PROXY_ID_UNITIALIZED);
            }

            Log.Trace("Finished Remove entity " + proxyId, "Remove");
        }

        public void Clear() {
            Init();
            MathTree.Clear();
        }

        public void Move(AABBEntity entity) {
            int proxyId = GetProxyIdForEntity(entity);
            if (proxyId != PROXY_ID_UNITIALIZED) {
                BoundingBoxD bbox = GetEntityAABB(entity);

                if (bbox.Size == Vector3D.Zero)  // remove entities with zero bounding boxes
                {
                    Remove(entity);
                    return;
                }

                MathTree.MoveProxy(proxyId, ref bbox, Vector3D.Zero);
            }
        }

        public void GetAllEntitiesInBox<T>(ref BoundingBoxD box, List<T> result) {
            MathTree.OverlapAllBoundingBox<T>(ref box, result, 0, false);
        }

        public void GetAllEntitiesInSphere<T>(ref BoundingSphereD sphere, List<T> result) {
            MathTree.OverlapAllBoundingSphere<T>(ref sphere, result, false);
        }

        public void GetAllEntitiesInRay<T>(ref LineD ray, List<MyLineSegmentOverlapResult<T>> result) {
            MathTree.OverlapAllLineSegment<T>(ref ray, result);
        }

        #region AABBEntity Accessors

        // We define these here because AABBEntity is an interface

        private int GetProxyIdForEntity(AABBEntity entity) {
            if (entity.ProxyIdsByTree == null) {
                entity.ProxyIdsByTree = new Dictionary<long, int>();
            }

            if (!entity.ProxyIdsByTree.ContainsKey(TreeId)) {
                return PROXY_ID_UNITIALIZED;
            }
            else {
                return entity.ProxyIdsByTree[TreeId];
            }
        }

        private void SetProxyIdForEntity(AABBEntity entity, int newId) {
            if (entity.ProxyIdsByTree == null) {
                entity.ProxyIdsByTree = new Dictionary<long, int>();
            }

            entity.ProxyIdsByTree[TreeId] = newId;
        }

        #endregion

    }
}