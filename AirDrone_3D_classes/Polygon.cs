using System.Collections.Generic;

namespace AirDrone_3D_classes
{
    public class Polygon
    {
        public PPoint[] Vertex { get; set; }

        public Polygon(string[] line_data, List<PPoint> coordinates)
        {
            Vertex = new PPoint[line_data.Length - 1];
            for (int i = 1; i < line_data.Length; i++)
            {
                int position = int.Parse(line_data[i].Split('/')[0]);
                if (position > 0)
                    Vertex[i - 1] = coordinates[position];
                else if (position < 0)
                    Vertex[i - 1] = coordinates[coordinates.Count + position];
            }
        }

        public Polygon Normalize(double k)
        {
            for (int i = 0; i < Vertex.Length; i++)
                Vertex[i] = new PPoint(Vertex[i].X / k, Vertex[i].Y / k, Vertex[i].Z / k);
            return this;
        }
    }
}
