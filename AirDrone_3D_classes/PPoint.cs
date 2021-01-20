//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace AirDrone_3D_classes
{
    public class PPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public PPoint(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public PPoint(Vector v)
        {
            X = v.X / v.W;
            Y = v.Y / v.W;
            Z = v.Z / v.W;
        }
        public PPoint()
        { X = Y = Z = 0; }
    }
}
