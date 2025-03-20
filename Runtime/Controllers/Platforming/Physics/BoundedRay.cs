using UnityEngine;

namespace BBUnity.Entities.Controllers.Platforming.Physics {

    /// <summary>
    /// A single ray
    /// </summary>
    public class BoundedRay {

        private Vector2 _start;
        private Vector2 _end;
        private Vector2 _direction;

        public Vector2 Start { get { return _start; } }
        public Vector2 End { get { return _end; } }
        public Vector2 Direction { get { return _direction; } }

        public BoundedRay(Vector2 direction) {
            _start = Vector2.zero;
            _end = Vector2.zero;
            _direction = direction;
        }

        private void Update(float startX, float startY, float endX, float endY) {
            _start.x = startX;
            _start.y = startY;
            _end.x = endX;
            _end.y = endY;
        }

        public void UpdateDown(Bounds b, float buffer) {
            Update(b.min.x + buffer, b.min.y, b.max.x - buffer, b.min.y);
        }

        public void UpdateUp(Bounds b, float buffer) {
            Update(b.min.x + buffer, b.max.y, b.max.x - buffer, b.max.y);
        }

        public void UpdateLeft(Bounds b, float buffer) {
            Update(b.min.x, b.min.y + buffer, b.min.x,  b.max.y - buffer);
        }

        public void UpdateRight(Bounds b, float buffer) {
            Update(b.max.x, b.min.y + buffer, b.max.x, b.max.y - buffer);
        }
    }
}