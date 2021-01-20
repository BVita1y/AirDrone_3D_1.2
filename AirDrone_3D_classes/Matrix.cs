using System;

namespace AirDrone_3D_classes
{
    public class Matrix
    {
        public double[,] matrix;

        public Matrix()
        {
            matrix = new double[4, 4];
            for (int i = 0; i < 4; i++)
                matrix[i, i] = 1;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    result.matrix[i, j] = 0;

                    for (int k = 0; k < 4; k++)
                        result.matrix[i, j] += m1.matrix[i, k] * m2.matrix[k, j];
                }
            return result;
        }

        public static Matrix LookAt(Vector eye, Vector center, Vector up)
        {
            Vector z = (eye - center).Normalize();
            Vector x = (up * z).Normalize();
            Vector y = (z * x).Normalize();

            Matrix lookAt = new Matrix();

            lookAt.matrix[0, 0] = x.X;
            lookAt.matrix[1, 0] = y.X;
            lookAt.matrix[2, 0] = z.X;
           
            lookAt.matrix[0, 1] = x.Y;
            lookAt.matrix[1, 1] = y.Y;
            lookAt.matrix[2, 1] = z.Y;

            lookAt.matrix[0, 2] = x.Z;
            lookAt.matrix[1, 2] = y.Z;
            lookAt.matrix[2, 2] = z.Z;
              
            lookAt.matrix[0, 3] = -eye.X;
            lookAt.matrix[1, 3] = -eye.Y;
            lookAt.matrix[2, 3] = -eye.Z;

            return lookAt;
        }

        public static Matrix ViewPort(int x, int y, int width, int height)
        {
            int depth = 255;

            Matrix viewport = new Matrix();
            
            viewport.matrix[0, 0] = width / 2.0;
            viewport.matrix[1, 1] = height / -2.0;
            viewport.matrix[2, 2] = depth / 2.0;

            viewport.matrix[0, 3] = x + width / 2.0;
            viewport.matrix[1, 3] = y + height / 2.0;
            viewport.matrix[2, 3] = depth / 2.0;
             
            return viewport;
        }

        public static Matrix ProjOrto(double l, double r, double b, double t, double n, double f)
        {
            Matrix projorto = new Matrix();

            projorto.matrix[0, 0] = 2.0 / (r - l);
            projorto.matrix[1, 1] = 2.0 / (t - b);
            projorto.matrix[2, 2] = -2.0 / (f - n);

            projorto.matrix[0, 3] = -(r + l) / (r - l);
            projorto.matrix[1, 3] = -(f - n) / (t - b);
            projorto.matrix[2, 3] = -(f + n) / (f - n);

            return projorto;
        }

        public static Matrix ProjOrto(double f)
        {
            Matrix projorto = new Matrix();

            projorto.matrix[3, 2] = - 1.0 / f;

            return projorto;
        }

        public static Matrix Scale(double kx, double ky, double kz)
        {
            Matrix scale = new Matrix();

            scale.matrix[0, 0] = kx;
            scale.matrix[1, 1] = ky;
            scale.matrix[2, 2] = kz;

            return scale;
        }
        public static Matrix Scale(double k)
        {
            Matrix scale = new Matrix();
            scale.matrix[3, 3] = k;
            return scale;
        }

        public static Matrix Rotate(double x_alpha, double y_alpha, double z_alpha)
        {
            double alpha_rad;
            Matrix rotate, xRotate, yRotate, zRotate;

            rotate = new Matrix();

            const double D2R = 0.0174533;

            if (x_alpha != 0)
            {
                xRotate = new Matrix();

                alpha_rad = x_alpha * D2R;

                xRotate.matrix[1, 1] = Math.Cos(alpha_rad);
                xRotate.matrix[1, 2] = -Math.Sin(alpha_rad);
                xRotate.matrix[2, 1] = Math.Sin(alpha_rad);
                xRotate.matrix[2, 2] = Math.Cos(alpha_rad);

                rotate *= xRotate;
            }
            if (y_alpha != 0)
            {
                yRotate = new Matrix();

                alpha_rad = y_alpha * D2R;

                yRotate.matrix[0, 0] = Math.Cos(alpha_rad);
                yRotate.matrix[0, 2] = -Math.Sin(alpha_rad);
                yRotate.matrix[2, 0] = Math.Sin(alpha_rad);
                yRotate.matrix[2, 2] = Math.Cos(alpha_rad);

                rotate *= yRotate;
            }
            if (z_alpha != 0)
            {
               zRotate = new Matrix();

                alpha_rad = z_alpha * D2R;

                zRotate.matrix[0, 0] = Math.Cos(alpha_rad);
                zRotate.matrix[1, 2] = -Math.Sin(alpha_rad);
                zRotate.matrix[2, 1] = Math.Sin(alpha_rad);
                zRotate.matrix[2, 2] = Math.Cos(alpha_rad);

                rotate *= zRotate;
            }

            return rotate;
        }

        public static Matrix Transfer(double tx, double ty, double tz)
        {
            Matrix transfer = new Matrix();

            transfer.matrix[0, 3] = tx;
            transfer.matrix[1, 3] = ty;
            transfer.matrix[2, 3] = tz;

            return transfer;
        }

    }
}
