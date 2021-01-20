using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using AirDrone_3D_classes;

namespace AirDrone_3D_1._2
{
    public partial class Form1 : Form
    {
        Bitmap buf;
        Graphics gr;
        int width;
        int height;

        double y_angle = 45;
        double z_index = 0.1;
        const double R2D = Math.PI / 180;
        double KF;
        double x_max = 0, y_max = 0, z_max = 0, x_min = 0, y_min = 0, z_min = 0;

        List<PPoint> coordinates;
        List<Polygon> polygons;

        #region Path
        //string path = @"AirDrone.obj";
        //string path = @"batman.obj";
        //string path = @"futuriscticCar.obj";
        string path = @"well.obj";
        //string path = @"Humvee.obj";
        //string path = @"LEGO_Man.obj";
        #endregion

        public Form1()
        {
            InitializeComponent();

            width = workSpace.Width;
            height = workSpace.Height;

            buf = new Bitmap(width, height);
            gr = Graphics.FromImage(buf);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            CreateImage();
        }

        public void CreateImage()
        {
            gr.FillRectangle(new SolidBrush(Color.Black), 0, 0, workSpace.Width, workSpace.Height);
            ReadFile();             // 150 - 200 мс
            DrawFigure();
            workSpace.Image = buf;
        }

        public void ReadFile()
        {
            coordinates = new List<PPoint>();
            polygons = new List<Polygon>();

            coordinates.Add(new PPoint(0, 0, 0));     //выравниваю индексацию, добавляя в начало лишнюю точку, чтобы точки из документа считались с 1

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var line_data = line.Split(' ');

                    if (line_data[0] == "v")
                    {
                        double x = double.Parse(line_data[1], CultureInfo.InvariantCulture.NumberFormat);
                        double y = double.Parse(line_data[2], CultureInfo.InvariantCulture.NumberFormat);
                        double z = double.Parse(line_data[3], CultureInfo.InvariantCulture.NumberFormat);

                        coordinates.Add(new PPoint(x, y, z));

                        if (x > x_max && x != 8) x_max = x;
                        if (x < x_min && x != -8) x_min = x;
                        if (y > y_max && y != 8) y_max = y;
                        if (y < y_min && y != -8) y_min = y;
                        if (z > z_max && z != 8) z_max = z;
                        if (z < z_min && z != -8) z_min = z;
                    }
                    else if (line_data[0] == "f")
                        polygons.Add(new Polygon(line_data, coordinates));
                }
            }

            KF = Math.Max(x_max - x_min, Math.Max(y_max - y_min, z_max - z_min));
        }
        public Vector ChangeCameraPosition(double y_angle)
         {
             double r = 0.2;

             Vector vector = new Vector();
             vector.X = r * Math.Sin(y_angle * R2D);
             vector.Z = r * Math.Cos(y_angle * R2D);
             vector.Y = z_index;

             return vector;
         }    

        public void DrawFigure()
        {
            Vector eye = ChangeCameraPosition(y_angle);
            Vector center = new Vector(0, 0, 0);
            Vector up = new Vector(0, 1, 0);

            #region Матричные преобразования

            Matrix Rot = Matrix.Rotate(0, 0, 0);
            Matrix Sc = Matrix.Scale(0.7);
            Matrix Tr = Matrix.Transfer(-120, -120, 0);

            Matrix VP = Matrix.ViewPort(width / 8, height / 8, width / 8 * 7, height / 8 * 7);
            Matrix MV = Matrix.LookAt(eye, center, up);
            Matrix PO = Matrix.ProjOrto(5);

            Matrix M = Sc * Tr * VP * PO * MV;
            //Matrix M = VP * PO * MV;

            #endregion

            //int[] zBuffer = new int[(width + 1000) * (height + 1000)];
            //for (int i = 0; i < zBuffer.Length; i++)
            //    zBuffer[i] = -10000;

            polygons.RemoveAt(0);   // удаляем из листа полигон площадки под дроном
            for (int i = 0; i < polygons.Count; i++)
            {
                polygons[i] = polygons[i].Normalize(KF);

                #region Применение матричных преобразований
                for (int j = 0; j < polygons[i].Vertex.Length; j++)
                    polygons[i].Vertex[j] = new PPoint(M * new Vector(polygons[i].Vertex[j]));
                #endregion

                #region//Использование z-buffer
                //Работает
                //for (int j = 0; j < polygons[i].Vertex.Length; j++)
                //{
                //    if (i == 0) color = 20;
                //    else color = rnd.Next(150, 200);

                //    UseBuffer(new PPointInt(polygons[i].Vertex[j % polygons[i].Vertex.Length]),
                //        new PPointInt(polygons[i].Vertex[(j + 1) % polygons[i].Vertex.Length]),
                //        new PPointInt(polygons[i].Vertex[(j + 2) % polygons[i].Vertex.Length]),
                //        Color.FromArgb(255, color, color, color), ref zBuffer);
                //}
                #endregion
            }



            #region //Рендер через сортировку
            Polygon[] polygonClone = new Polygon[polygons.Count];
            polygons.CopyTo(polygonClone);

            double[] zIndex = new double[polygonClone.Length];

            polygonClone = SortPolygons(polygonClone, ref zIndex);
            int color = 0;

            int min_zIndex = (int)(zIndex[0]);
            int max_zIndex = (int)(zIndex[zIndex.Length - 1]);
            int zIndexAmplitude = max_zIndex - min_zIndex; 

            for (int i = 0; i < polygonClone.Length; i++)
            {
                //int colorStep = polygonClone.Length / 150;
                //if (i % colorStep == 0 && color < 255) color += 1;

                Point[] pt = new Point[polygonClone[i].Vertex.Length];

                for (int j = 0; j < polygonClone[i].Vertex.Length; j++)
                {
                    pt[j].X = (int)polygonClone[i].Vertex[j].X;
                    pt[j].Y = (int)polygonClone[i].Vertex[j].Y;
                }

                color = (int)((zIndex[i] - min_zIndex) / zIndexAmplitude * 255);
                if (color < 0) color = 0;
                else if (color > 255) color = 255;
                gr.FillPolygon(new SolidBrush(Color.FromArgb(255, color, color, color)), pt);
            }
            #endregion
        }

        
        public static Polygon[] SortPolygons(Polygon[] polygons, ref double[] zIndex)
        {
            //double[] zIndex = new double[polygons.Length];
            for (int i = 0; i < polygons.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < polygons[i].Vertex.Length; j++)
                    sum += polygons[i].Vertex[j].Z;

                zIndex[i] = sum / polygons[i].Vertex.Length;
            }

            for (int i = 0; i < polygons.Length / 2; i++)
            {
                var swapFlag = false;
                for (var j = i; j < polygons.Length - i - 1; j++)
                    if (zIndex[j] > zIndex[j + 1])
                    {
                        var conteiner = polygons[j];
                        polygons[j] = polygons[j + 1];
                        polygons[j + 1] = conteiner;

                        var conteiner1 = zIndex[j];
                        zIndex[j] = zIndex[j + 1];
                        zIndex[j + 1] = conteiner1;
                        swapFlag = true;
                    }

                for (var j = polygons.Length - 2 - i; j > i; j--)
                    if (zIndex[j - 1] > zIndex[j])
                    {
                        var conteiner = polygons[j];
                        polygons[j] = polygons[j - 1];
                        polygons[j - 1] = conteiner;

                        var conteiner1 = zIndex[j];
                        zIndex[j] = zIndex[j - 1];
                        zIndex[j - 1] = conteiner1;
                        swapFlag = true;
                    }
                if (!swapFlag) break;
            }
            return polygons;
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case (char)Keys.Left:
                    y_angle -= 10;
                    CreateImage();
                    break;
                case (char)Keys.Right:
                    y_angle += 10;
                    CreateImage();
                    break;
                case (char)Keys.Up:
                    if (z_index <= 0.20)
                    {
                        z_index += 0.05;
                        CreateImage();
                    }
                    break;
                case (char)Keys.Down:
                    if (z_index > 0.01)
                    {
                        z_index -= 0.05;
                        CreateImage();
                    }
                    break;
                case (char)Keys.Escape:
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        // Использование z-buffer
        //
        //void UseBuffer(PPointInt t0, PPointInt t1, PPointInt t2, Color color, ref int[] zBuffer)
        //{
        //    if (t0.Y == t1.Y && t0.Y == t2.Y)
        //    {
        //        gr.DrawPolygon(new Pen(color), new Point[] { new Point(t0.X, t0.Y), new Point(t1.X, t1.Y), new Point(t2.X, t2.Y) });
        //        return;
        //    }

        //    if (t0.Y > t1.Y) Swap(ref t0, ref t1);
        //    if (t0.Y > t2.Y) Swap(ref t0, ref t2);
        //    if (t1.Y > t2.Y) Swap(ref t1, ref t2);
        //    int total_height = t2.Y - t0.Y;
        //    for (int i = 0; i < total_height; i++)
        //    {
        //        bool second_half = i > t1.Y - t0.Y || t1.Y == t0.Y;
        //        int segment_height = second_half ? t2.Y - t1.Y : t1.Y - t0.Y;
        //        double alpha = (double)i / total_height;
        //        double beta = (double)(i - (second_half ? t1.Y - t0.Y : 0)) / segment_height; // be careful: with above conditions no division by zero here
        //        PPointInt A = t0 + (t2 - t0) * alpha;
        //        PPointInt B = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;
        //        if (A.X > B.X) Swap(ref A, ref B);
        //        for (int j = A.X; j <= B.X; j++)
        //        {
        //            double phi = B.X == A.X ? 1 : (j - A.X) / (double)(B.X - A.X);

        //            PPointInt P = A + (B - A) * phi;
        //            int idx = P.X + P.Y * width;
        //            if (zBuffer[idx] <= P.Z)
        //            {
        //                zBuffer[idx] = P.Z;

        //                gr.DrawLine(new Pen(color), P.X, P.Y, P.X, P.Y + 1);
        //                gr.DrawLine(new Pen(color), P.X, P.Y, P.X + 1, P.Y);
        //                gr.DrawLine(new Pen(color), P.X, P.Y + 1, P.X + 1, P.Y + 1);

        //            }
        //        }
        //    }
        //}

        //public static void Swap<T>(ref T el1, ref T el2)
        //{
        //    T el_conteiner = el1;
        //    el1 = el2;
        //    el2 = el_conteiner;
        //}
    }
}
