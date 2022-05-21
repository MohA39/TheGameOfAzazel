using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameOfAzazel
{
    internal class Helpers
    {
        public static double AngleFromPoints(Vector2 Point1, Vector2 Point2)
        {

            float xDiff = Point2.X - Point1.X;
            float yDiff = Point2.Y - Point1.Y;
            return Math.Atan2(yDiff, xDiff);// * 180.0 / Math.PI;
        }

        public static Vector2 GetBoundingBox(double rw, double rh, double radians)
        {
            double x1 = -rw / 2,
                x2 = rw / 2,
                x3 = rw / 2,
                x4 = -rw / 2,
                y1 = rh / 2,
                y2 = rh / 2,
                y3 = -rh / 2,
                y4 = -rh / 2;

            double x11 = x1 * Math.Cos(radians) + y1 * Math.Sin(radians),
                y11 = -x1 * Math.Sin(radians) + y1 * Math.Cos(radians),
                x21 = x2 * Math.Cos(radians) + y2 * Math.Sin(radians),
                y21 = -x2 * Math.Sin(radians) + y2 * Math.Cos(radians),
                x31 = x3 * Math.Cos(radians) + y3 * Math.Sin(radians),
                y31 = -x3 * Math.Sin(radians) + y3 * Math.Cos(radians),
                x41 = x4 * Math.Cos(radians) + y4 * Math.Sin(radians),
                y41 = -x4 * Math.Sin(radians) + y4 * Math.Cos(radians);

            double x_min = (new double[] { x11, x21, x31, x41 }).Min();
            double x_max = (new double[] { x11, x21, x31, x41 }).Max();

            double y_min = (new double[] { y11, y21, y31, y41 }).Min();
            double y_max = (new double[] { y11, y21, y31, y41 }).Max();


            return new Vector2((float)(x_max - x_min), (float)(y_max - y_min));
        }


        public static Vector2 intersect(int edgeLine, Vector2 line2point1, Vector2 line2point2, Rectangle rectangle)
        {

            float[] A1 = { -rectangle.Height, 0, rectangle.Height, 0 };
            float[] B1 = { 0, -rectangle.Width, 0, rectangle.Width };
            float[] C1 = { -rectangle.Width * rectangle.Height, -rectangle.Width * rectangle.Height, 0, 0 };

            float A2 = line2point2.Y - line2point1.Y;
            float B2 = line2point1.X - line2point2.X;
            float C2 = A2 * line2point1.X + B2 * line2point1.Y;

            float det = A1[edgeLine] * B2 - A2 * B1[edgeLine];

            return new Vector2((B2 * C1[edgeLine] - B1[edgeLine] * C2) / det, (A1[edgeLine] * C2 - A2 * C1[edgeLine]) / det);
        }

        public static string DegreesToCardinalDirection(float Angle)
        {

            if (Angle < 0)
            {
                Angle += 360;
            }
            string[] Directions = new string[] { "n", "e", "s", "w", "n" };
            int DegreesPerDirection = 360 / (Directions.Length - 1);
            return Directions[(int)Math.Round((Angle / DegreesPerDirection))];
        }

        public static List<Rectangle> SplitRectangle(Rectangle rectangle, int rows, int columns, float Margin)
        {
            List<Rectangle> Rects = new List<Rectangle>();
            for (int j = 0; j < columns; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    Rectangle currentSplit = new Rectangle(rectangle.X + rectangle.Width / rows * i, rectangle.Y + (rectangle.Height / columns * j), rectangle.Width / rows, rectangle.Height / columns);
                    currentSplit.Inflate(-Margin, -Margin); // Add margin
                    Rects.Add(currentSplit);
                }
            }

            return Rects;
        }


    }


}
