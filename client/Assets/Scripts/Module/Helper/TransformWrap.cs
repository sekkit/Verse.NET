using UnityEngine;

namespace Module.Helper
{
    public struct TransformWrap
    { 
        public Vector3 Position { get; set; } //= Vector3.zero;
        public Vector3 Euler { get; set; } //= Vector3.zerodxs
                                           //;
        public Vector3 Scale { get; set; } //= Vector3.zero;
 
        public bool IsInited;

        public TransformWrap(Transform transform) :this()
        {
            Position = transform.position;
            Euler = transform.eulerAngles;
            Scale = transform.localScale;
            IsInited = true;
        }

        public void ApplyTo(Transform transform)
        {
            transform.position = Position;
            transform.eulerAngles = Euler;
            transform.localScale = Scale;
        }

        public override bool Equals(object obj)
        {
            if (obj is TransformWrap)
            {
                var v = (TransformWrap)obj;
                return v.Position == this.Position && v.Euler == this.Euler && v.Scale == this.Scale;
            }
            if (obj is Transform)
            {
                var v = (Transform)obj;
                return v.position == this.Position && v.eulerAngles == this.Euler && v.localScale == this.Scale;
            }

            return base.Equals(obj);
    } 
    }
}