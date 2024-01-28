namespace GameFunctions {

    public struct GFVector4Int {

        public int x;
        public int y;
        public int z;
        public int w;

        public GFVector4Int(int x, int y, int z, int w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static GFVector4Int operator +(GFVector4Int a, GFVector4Int b) {
            return new GFVector4Int(
                a.x + b.x,
                a.y + b.y,
                a.z + b.z,
                a.w + b.w
            );
        }

        public static GFVector4Int operator -(GFVector4Int a, GFVector4Int b) {
            return new GFVector4Int(
                a.x - b.x,
                a.y - b.y,
                a.z - b.z,
                a.w - b.w
            );
        }

        public static GFVector4Int operator *(GFVector4Int a, int b) {
            return new GFVector4Int(
                a.x * b,
                a.y * b,
                a.z * b,
                a.w * b
            );
        }

        public static GFVector4Int operator *(int a, GFVector4Int b) {
            return new GFVector4Int(
                a * b.x,
                a * b.y,
                a * b.z,
                a * b.w
            );
        }

        public static GFVector4Int operator /(GFVector4Int a, int b) {
            return new GFVector4Int(
                a.x / b,
                a.y / b,
                a.z / b,
                a.w / b
            );
        }

        public static GFVector4Int operator /(int a, GFVector4Int b) {
            return new GFVector4Int(
                a / b.x,
                a / b.y,
                a / b.z,
                a / b.w
            );
        }

        public static bool operator ==(GFVector4Int a, GFVector4Int b) {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        public static bool operator !=(GFVector4Int a, GFVector4Int b) {
            return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
        }

        public override bool Equals(object obj) {
            if (obj is GFVector4Int) {
                GFVector4Int v = (GFVector4Int)obj;
                return x == v.x && y == v.y && z == v.z && w == v.w;
            }
            return false;
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

    }
}