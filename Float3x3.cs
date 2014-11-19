namespace perspectivePlayground
{
    /// <summary>
    /// Ported from gimp. See gimpmatrix.c and gimp-transform-utils.c
    /// </summary>
    public class Float3x3
    {
        private static readonly Float3x3 Identity;
        private float[][] coeff;

        static Float3x3()
        {
            Identity = new Float3x3();
            Identity.coeff[0][0] = 1;
            Identity.coeff[1][1] = 1;
            Identity.coeff[2][2] = 1;
        }

        public Float3x3()
        {
            coeff = new float[3][];
            for (int i = 0; i < 3; i++)
            {
                coeff[i] = new float[3];
            }
        }
      

        public static Float3x3 Perspective(float x, float y, float width, float height, Float2 t1, Float2 t2, Float2 t3, Float2 t4)
        {

            var scalex = 1.0f;
            var scaley = 1.0f;

            if (width > 0)
                scalex = 1.0f / width;

            if (height > 0)
                scaley = 1.0f / height;

            var matrix = Float3x3.Identity;
            matrix = matrix.Translate(-x, -y);
            matrix = matrix.Scale(scalex, scaley);

            var trafo = new Float3x3();
            {

                var t_x1 = t1.X;
                var t_y1 = t1.Y;
                var t_x2 = t2.X;
                var t_y2 = t2.Y;
                var t_x3 = t3.X;
                var t_y3 = t3.Y;
                var t_x4 = t4.X;
                var t_y4 = t4.Y;
                var dx1 = t_x2 - t_x4;
                var dx2 = t_x3 - t_x4;
                var dx3 = t_x1 - t_x2 + t_x4 - t_x3;
                var dy1 = t_y2 - t_y4;
                var dy2 = t_y3 - t_y4;
                var dy3 = t_y1 - t_y2 + t_y4 - t_y3;
                /*  Is the mapping affine?  */
                var epsilon = 1e-4f;
                if ((dx3.EpsilonEquals(0, epsilon)) && (dy3.EpsilonEquals(0, epsilon)))
                {
                    trafo.coeff[0][0] = t_x2 - t_x1;
                    trafo.coeff[0][1] = t_x4 - t_x2;
                    trafo.coeff[0][2] = t_x1;
                    trafo.coeff[1][0] = t_y2 - t_y1;
                    trafo.coeff[1][1] = t_y4 - t_y2;
                    trafo.coeff[1][2] = t_y1;
                    trafo.coeff[2][0] = 0.0f;
                    trafo.coeff[2][1] = 0.0f;
                }
                else
                {
                    var det1 = dx3 * dy2 - dy3 * dx2;
                    var det2 = dx1 * dy2 - dy1 * dx2;

                    trafo.coeff[2][0] = det2.EpsilonEquals(0, epsilon) ? 1.0f : det1 / det2;

                    det1 = dx1 * dy3 - dy1 * dx3;

                    trafo.coeff[2][1] = det2.EpsilonEquals(0, epsilon) ? 1.0f : det1 / det2;

                    trafo.coeff[0][0] = t_x2 - t_x1 + trafo.coeff[2][0] * t_x2;
                    trafo.coeff[0][1] = t_x3 - t_x1 + trafo.coeff[2][1] * t_x3;
                    trafo.coeff[0][2] = t_x1;

                    trafo.coeff[1][0] = t_y2 - t_y1 + trafo.coeff[2][0] * t_y2;
                    trafo.coeff[1][1] = t_y3 - t_y1 + trafo.coeff[2][1] * t_y3;
                    trafo.coeff[1][2] = t_y1;
                }
                trafo.coeff[2][2] = 1.0f;
            }

            return trafo.Multiply(matrix);
        }

        public float Determinant()
        {
            var matrix = this;
            var determinant = (matrix.coeff[0][0]
                               * (matrix.coeff[1][1] * matrix.coeff[2][2] - matrix.coeff[1][2] * matrix.coeff[2][1]));
            determinant -= (matrix.coeff[1][0]
                            * (matrix.coeff[0][1] * matrix.coeff[2][2] - matrix.coeff[0][2] * matrix.coeff[2][1]));
            determinant += (matrix.coeff[2][0]
                            * (matrix.coeff[0][1] * matrix.coeff[1][2] - matrix.coeff[0][2] * matrix.coeff[1][1]));

            return determinant;
        }

        public Float3x3 Invert()
        {
            var inv = new Float3x3();
            var matrix = this;

            var det = Determinant();

            if (det.EpsilonEquals(0, 1e-5f))
                det = 1.0f / det;

            inv.coeff[0][0] = (matrix.coeff[1][1] * matrix.coeff[2][2] -
                                 matrix.coeff[1][2] * matrix.coeff[2][1]) * det;

            inv.coeff[1][0] = -(matrix.coeff[1][0] * matrix.coeff[2][2] -
                                 matrix.coeff[1][2] * matrix.coeff[2][0]) * det;

            inv.coeff[2][0] = (matrix.coeff[1][0] * matrix.coeff[2][1] -
                                 matrix.coeff[1][1] * matrix.coeff[2][0]) * det;

            inv.coeff[0][1] = -(matrix.coeff[0][1] * matrix.coeff[2][2] -
                                 matrix.coeff[0][2] * matrix.coeff[2][1]) * det;

            inv.coeff[1][1] = (matrix.coeff[0][0] * matrix.coeff[2][2] -
                                 matrix.coeff[0][2] * matrix.coeff[2][0]) * det;

            inv.coeff[2][1] = -(matrix.coeff[0][0] * matrix.coeff[2][1] -
                                 matrix.coeff[0][1] * matrix.coeff[2][0]) * det;

            inv.coeff[0][2] = (matrix.coeff[0][1] * matrix.coeff[1][2] -
                                 matrix.coeff[0][2] * matrix.coeff[1][1]) * det;

            inv.coeff[1][2] = -(matrix.coeff[0][0] * matrix.coeff[1][2] -
                                 matrix.coeff[0][2] * matrix.coeff[1][0]) * det;

            inv.coeff[2][2] = (matrix.coeff[0][0] * matrix.coeff[1][1] -
                                 matrix.coeff[0][1] * matrix.coeff[1][0]) * det;

            return inv;
        }

        public Float2 TransformPoint(Float2 point)
        {
            var x = point.X;
            var y = point.Y;
            var matrix = this;

            var w = matrix.coeff[2][0] * x + matrix.coeff[2][1] * y + matrix.coeff[2][2];

            if (w == 0.0)
                w = 1.0f;
            else
                w = 1.0f / w;

            var newx = (matrix.coeff[0][0] * x +
                     matrix.coeff[0][1] * y +
                     matrix.coeff[0][2]) * w;
            var newy = (matrix.coeff[1][0] * x +
                     matrix.coeff[1][1] * y +
                     matrix.coeff[1][2]) * w;
            return new Float2(newx, newy);
        }

        public Float3x3 Translate(float x, float y)
        {
            var matrix = Clone();
            var g = matrix.coeff[2][0];
            var h = matrix.coeff[2][1];
            var i = matrix.coeff[2][2];
            matrix.coeff[0][0] += x * g;
            matrix.coeff[0][1] += x * h;
            matrix.coeff[0][2] += x * i;
            matrix.coeff[1][0] += y * g;
            matrix.coeff[1][1] += y * h;
            matrix.coeff[1][2] += y * i;
            return matrix;
        }

        public Float3x3 Scale(float x, float y)
        {
            var matrix = Clone();
            matrix.coeff[0][0] *= x;
            matrix.coeff[0][1] *= x;
            matrix.coeff[0][2] *= x;

            matrix.coeff[1][0] *= y;
            matrix.coeff[1][1] *= y;
            matrix.coeff[1][2] *= y;

            return matrix;
        }

        public Float3x3 Clone()
        {
            var r = new Float3x3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    r.coeff[i][j] = coeff[i][j];
                }
            }
            return r;
        }

        public Float3x3 Multiply(Float3x3 other)
        {
            var tmp = new Float3x3();

            var matrix1 = this;
            var matrix2 = other;

            for (var i = 0; i < 3; i++)
            {
                var t1 = matrix1.coeff[i][0];
                var t2 = matrix1.coeff[i][1];
                var t3 = matrix1.coeff[i][2];

                for (var j = 0; j < 3; j++)
                {
                    tmp.coeff[i][j] = t1 * matrix2.coeff[0][j];
                    tmp.coeff[i][j] += t2 * matrix2.coeff[1][j];
                    tmp.coeff[i][j] += t3 * matrix2.coeff[2][j];
                }
            }
            return tmp;
        }
    }
}