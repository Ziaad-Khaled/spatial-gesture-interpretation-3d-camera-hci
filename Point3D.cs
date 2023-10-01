using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    [Serializable]
    internal class Point3D
    {
        private double x;
        private double y;
        private double z;

        private static int screenwidth = 350;
        private static int screenheight = 350;

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double GetX()
        {
            return x;
        }

        public void SetX(double value)
        {
            x = value;
        }

        public double GetY()
        {
            return y;
        }

        public void SetY(double value)
        {
            y = value;
        }

        public double GetZ()
        {
            return z;
        }

        public void SetZ(double value)
        {
            z = value;
        }

        public void SetNewPoints(double x, double y, double z)
        {
            this.x=x;
            this.y=y;
            this.z=z;
        }
        public void Reset()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public static Boolean AreTheseTwoPointsNearEachOther(Point3D point1, Point3D point2, double threshold)
        {
            double xDifference =Math.Abs(point1.GetX() - point2.GetX());
            double yDifference = Math.Abs(point1.GetY() - point2.GetY());
            Console.WriteLine("XDifference: " + xDifference);
            Console.WriteLine("YDifference: " + yDifference);
            if(yDifference < threshold && xDifference < 0)
                return true;
            else
                return false;
        }

        public static double distanceBetweenTwoPoints(Point3D point1, Point3D point2)
        {
            double xSquared = Math.Pow(point1.GetX() - point2.GetX(), 2);
            double ySquared = Math.Pow(point1.GetY() - point2.GetY(), 2);
            return Math.Sqrt(xSquared + ySquared);
        }

        public string ToString()
        {
            return "X: " + x + ", " + "Y: " + y;
        }

        public static Point ConvertTo2DPoint(Point3D point3D, Point3D averageOfPoints)
        {
            Point point = new Point();
            if(averageOfPoints != null)
            {
                point.X = (int)(point3D.x - averageOfPoints.x + screenwidth / 2);
                point.Y = (int)(point3D.y - averageOfPoints.y + screenheight / 2);
            }
            
            return point;
        }
        public static Point ConvertTo2DPoint(Point3D point3D)
        {
            Point point = new Point();
            point.X = (int)point3D.x ;
            point.Y = (int)point3D.y ;
            return point;
        }

        public static Point[] ConvertTo2DPoints(LinkedList<Point3D> list, Point3D averageOfPoints)
        {
            Point[] points = new Point[list.Count];
            var currentNode = list.First;
            int i= 0;
            while ((currentNode != null) )
            {
                points[i] = ConvertTo2DPoint(currentNode.Value, averageOfPoints);
                currentNode = currentNode.Next;
                i++;
            }
            return points;
        }

        public static Point[] ConvertTo2DPoints(LinkedListNode<Point3D>[] list)
        {
            Point[] points = new Point[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                points[i] = ConvertTo2DPoint(list[i].Value);
            }
            return points;
        }
    }
}
