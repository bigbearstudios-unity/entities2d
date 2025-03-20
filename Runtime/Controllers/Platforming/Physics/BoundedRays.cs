using UnityEngine;

namespace BBUnity.Entities.Controllers.Platforming.Physics {

    /// <summary>
    /// 
    /// </summary>
    public class BoundedRays {
        BoundedRay[] _rays;
        Bounds _calculatedBounds;

        public BoundedRays() {
            _rays = new BoundedRay[4] { 
                new BoundedRay(Vector2.left), 
                new BoundedRay(Vector2.up), 
                new BoundedRay(Vector2.right), 
                new BoundedRay(Vector2.down) 
            };

            _calculatedBounds = new Bounds();
        }

        public void Update(Bounds bounds, Vector3 position, float buffer) {
            _calculatedBounds.center = position + bounds.center;
            _calculatedBounds.size = bounds.size;

            Left.UpdateLeft(_calculatedBounds, buffer);
            Up.UpdateUp(_calculatedBounds, buffer);
            Right.UpdateRight(_calculatedBounds, buffer);
            Down.UpdateDown(_calculatedBounds, buffer);
        }

        public BoundedRay Left { get { return _rays[0]; } }
        public BoundedRay Up { get { return _rays[1]; } }
        public BoundedRay Right { get { return _rays[2]; } }
        public BoundedRay Down { get { return _rays[3]; } }

        public BoundedRay[] All {
            get {
                return _rays;
            }
        }
    }
}