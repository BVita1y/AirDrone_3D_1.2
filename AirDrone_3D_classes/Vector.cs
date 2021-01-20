using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirDrone_3D_classes
{
    public class Vector : PPoint
    {
        public double W { get; private set; }

        public Vector() : base(0,0,0)
        {
            W = 0;
        }

        public Vector(double x, double y, double z) : base(x, y, z)
        {
            W = 1;
        }

        public Vector(PPoint p) : base(p.X, p.Y, p.Z)
        {
            W = 1;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            Vector result = new Vector();

            result.X = m.matrix[0, 0] * v.X + m.matrix[0, 1] * v.Y + m.matrix[0, 2] * v.Z + m.matrix[0, 3] * v.W;
            result.Y = m.matrix[1, 0] * v.X + m.matrix[1, 1] * v.Y + m.matrix[1, 2] * v.Z + m.matrix[1, 3] * v.W;
            result.Z = m.matrix[2, 0] * v.X + m.matrix[2, 1] * v.Y + m.matrix[2, 2] * v.Z + m.matrix[2, 3] * v.W;
            result.W = m.matrix[3, 0] * v.X + m.matrix[3, 1] * v.Y + m.matrix[3, 2] * v.Z + m.matrix[3, 3] * v.W;

            return result;
        }

        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }
        public Vector Normalize()
        {
            return new Vector(this.X / this.Length(), this.Y / this.Length(), this.Z / this.Length());
        }
    }
}
