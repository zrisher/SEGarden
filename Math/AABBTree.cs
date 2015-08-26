using System;
using System.Collections.Generic;
using VRageMath;

using Sandbox.Common;
using Sandbox.Game.Entities;
using Sandbox.Engine.Utils;

using MyDynamicAABBTree = VRageMath.MyDynamicAABBTree;


///<remarks>
/// Taken from the static SE class MyGamePruningStructure, adapted for flexible use
///</remarks>
namespace SEGarden.Math {

    public class AABBTree {

        public static readonly Vector3D AABB_EXTENSION = new Vector3D(3.0f);
        public const int PROXY_ID_UNITIALIZED = -1;

        private MyDynamicAABBTreeD m_aabbTree;

        public AABBTree() {
            Init();
        }

        /*
        private MyDynamicAABBTreeD GetPrunningStructure() {
            return m_aabbTree;
        }
        */

        private void Init() {
            m_aabbTree = new MyDynamicAABBTreeD(AABB_EXTENSION);
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
            BoundingBoxD bbox = GetEntityAABB(entity);
            if (bbox.Size == Vector3D.Zero) return;  // don't add entities with zero bounding boxes

            entity.TreeProxyID = m_aabbTree.AddProxy(ref bbox, entity, 0);
        }



        public void Remove(AABBEntity entity) {
            if (entity.TreeProxyID != PROXY_ID_UNITIALIZED) {
                m_aabbTree.RemoveProxy(entity.TreeProxyID);
                entity.TreeProxyID = PROXY_ID_UNITIALIZED;
            }
        }

        public void Clear() {
            Init();
            m_aabbTree.Clear();
        }

        public void Move(AABBEntity entity) {
            if (entity.TreeProxyID != PROXY_ID_UNITIALIZED) {
                BoundingBoxD bbox = GetEntityAABB(entity);

                if (bbox.Size == Vector3D.Zero)  // remove entities with zero bounding boxes
                {
                    Remove(entity);
                    return;
                }

                m_aabbTree.MoveProxy(entity.TreeProxyID, ref bbox, Vector3D.Zero);
            }
        }

        public void GetAllEntitiesInBox<T>(ref BoundingBoxD box, List<T> result) {
            m_aabbTree.OverlapAllBoundingBox<T>(ref box, result, 0, false);
        }

        public void GetAllEntitiesInSphere<T>(ref BoundingSphereD sphere, List<T> result) {
            m_aabbTree.OverlapAllBoundingSphere<T>(ref sphere, result, false);
        }

        public void GetAllEntitiesInRay<T>(ref LineD ray, List<MyLineSegmentOverlapResult<T>> result) {
            m_aabbTree.OverlapAllLineSegment<T>(ref ray, result);
        }

    }
}