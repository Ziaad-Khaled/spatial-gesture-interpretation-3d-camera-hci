using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class GestureImage
    {

        //setting colored image

        private Bitmap bitmap; 
        //create Graphics
        private Graphics graphics;


        //image path
        private string path;

        //saving the preperation points to get them centered after getting the average of nucleus points
        private LinkedList<Point3D> preperationLinkedList = new LinkedList<Point3D>();
        private LinkedList<Point3D> nucleusLinkedList = new LinkedList<Point3D>();
        private LinkedList<Point3D> retractionLinkedList = new LinkedList<Point3D>(); 

        private Point3D nucleusAverage = null;


        // to bound the opacity for a given pen color
        private int minBound = 0;
        private int maxBound = 255;

        public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
        public Graphics Graphics { get => graphics; set => graphics = value; }
        public string Path { get => path; set => path = value; }
        public LinkedList<Point3D> PreperationLinkedList { get => preperationLinkedList; set => preperationLinkedList = value; }
        public Point3D NucleusAverage { get => nucleusAverage; set => nucleusAverage = value; }
        public LinkedList<Point3D> NucleusLinkedList { get => nucleusLinkedList; set => nucleusLinkedList = value; }
        public LinkedList<Point3D> RetractionLinkedList { get => retractionLinkedList; set => retractionLinkedList = value; }

        public void DrawLinesWithDifferientOpacityAndColor(int type, LinkedList<Point3D> list, Point3D averageOfPoints)
        {
            if(list == null)
                list = new LinkedList<Point3D>();
            Point[] points = new Point[list.Count];
            LinkedListNode<Point3D> currentNode = list.First;
            LinkedListNode<Point3D> previousNode = null;
            int i = 0;

            double stepSize = CalculateStepSize(list.Count);


            while ((currentNode != null))
            {
                points[i] = Point3D.ConvertTo2DPoint(currentNode.Value, averageOfPoints);

                // there is a minimum of two points so I can begin to draw lines
                if(previousNode != null)
                {
                    float width = CalculatePenWidth(points[i - 1],points[i]);
                    Pen pen = CreatePen(type, stepSize*i, width);
                    DrawLine(pen, points[i-1], points[i]);
                }

                //preperaring for the next iteration
                previousNode = currentNode;
                currentNode = currentNode.Next;
                i++;
            }
        }

        private float CalculatePenWidth(Point point1, Point point2)
        {
            float maxSpeed = 30;

            float minWidth = 10;
            float maxWidth = 30;

            float speed = CalculateSpeed(point1, point2);

            //to set an upper bound for the thickness
            if (speed > 30)
                speed = 30;

            float step = (speed / maxSpeed) * (maxWidth - minWidth);
            float width = step + minWidth;
            return width;
        }

        private float CalculateSpeed(Point point1, Point point2)
        {
            float xSquared = (float) Math.Pow(point1.X - point2.X, 2);
            float ySquared = (float) Math.Pow(point1.Y - point2.Y, 2);
            return (float) Math.Sqrt(xSquared + ySquared);
        }

        // type: 0 greyImage
        // type: 1 preperation
        // type: 2 nucleus 
        // type: 3 retraction
        //remember you should implement the width of the pen refrenced to speed
        private Pen CreatePen(int type, double stepSize, float width)
        {
            int penSize = (int)(maxBound - stepSize);
            Pen pen = null; 
            switch (type)
            {
                case 0:
                    pen = new Pen(Color.FromArgb(penSize, 0, 0, 0), width);
                    break;
                case 1:
                    pen = new Pen(Color.FromArgb(255, 0, 0, penSize), width);
                    break;
                case 2:
                    pen = new Pen(Color.FromArgb(255, 0, penSize, 0), width);
                    break;
                case 3:
                    pen = new Pen(Color.FromArgb(255, penSize, 0, 0), width);
                    break;
            }
            return pen;
        }

        //to calculate the step size of the opacity using the length of the list to begin at min bound and finish at the max bound
        public double CalculateStepSize(int size)
        {
            if (size == 0)
                return 0;
            double stepSize = (maxBound - minBound) / (size);
            return stepSize;
        }

        public void DrawLine(Pen pen, Point point1, Point point2)
        {
            this.Graphics.DrawLine(pen, point1, point2);
        }


        public void SaveImage()
        {
            bitmap.Save(path);
        }

    }
}
