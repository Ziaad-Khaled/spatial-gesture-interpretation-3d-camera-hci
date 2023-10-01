using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class CircleOrRectangle
    {
        private int numberOfCirclePoints = 16;
        private LinkedListNode<Point3D>[] circlePoint;
        private int[] circleNodesCounter;


        private Point3D firstPointInterpolated;
        private Point3D centerInterpolated;


        
        private static double[] diameter;

        private static int numberOfCenters;
        private Point3D[] center;

        ColoredGestureImage coloredGestureImage = null;
        Boolean createImage = false;
        int gestureCounter = 1;

        private Point3D minPoint = new Point3D(3000,3000,3000);
        private Point3D maxPoint = new Point3D(-999, -999, -999);


        private Point3D detectedCircleFirstPoint;

        internal LinkedListNode<Point3D>[] CirclePoint { get => circlePoint; set => circlePoint = value; }

        public CircleOrRectangle(LinkedListNode<Point3D> firstPoint)
        {
            //setting the circle points to be intialized as the first node
            circlePoint = new LinkedListNode<Point3D>[numberOfCirclePoints + 1];

            for (int i = 0; i < circlePoint.Length; i++)
            {
                circlePoint[i] = firstPoint;
            }

            //setting the circle node counter to see if I will go to the next node if I added another node
            circleNodesCounter = new int[numberOfCirclePoints + 1];
            for (int i = 0; i < circleNodesCounter.Length; i++)
            {
                circleNodesCounter[i] = 0;
            }

            diameter = new double[numberOfCirclePoints / 2];
            numberOfCenters = numberOfCirclePoints / 4;
            center = new Point3D[numberOfCenters];


            updateMinAndMaxPoints(firstPoint.Value);
        }

        public void AddLastPoint(LinkedListNode<Point3D> ninthPoint)
        {
            this.circlePoint[circlePoint.Length - 1] = ninthPoint;
            updateCircleNodes();
            updateMinAndMaxPoints(ninthPoint.Value);
        }


        public void updateCircleNodes()
        {
            Console.WriteLine("Point 0: " + circlePoint[0].Value.ToString()); 
            circleNodesCounter[circleNodesCounter.Length - 1]++;
            for(int i=1; i < circleNodesCounter.Length-1;i++)
            {
                int currentNode = circleNodesCounter[i];
                double ratio = (double) (i+1) / (numberOfCirclePoints+1);
                int nextNode = (int)(ratio * circleNodesCounter[circleNodesCounter.Length - 1]);
                if (nextNode > currentNode)
                {
                    circlePoint[i] = circlePoint[i].Next;
                    circleNodesCounter[i] = nextNode;
                }
                Console.WriteLine("Point " + i + ": " + circlePoint[i].Value.ToString());
            }
        }

        private void updateMinAndMaxPoints(Point3D value)
        {
            if (value.GetX() < minPoint.GetX())
                minPoint.SetX(value.GetX());
            if (value.GetY() < minPoint.GetY())
                minPoint.SetY(value.GetY());

            if (value.GetX() > maxPoint.GetX())
                maxPoint.SetX(value.GetX());
            if (value.GetY() > maxPoint.GetY())
                maxPoint.SetY(value.GetY());
        }

        public Point3D GetDetectedCircleFirstPoint()
        {
            return detectedCircleFirstPoint;
        }

        public void SetDetectedCircleFirstPoint(Point3D value)
        {
            detectedCircleFirstPoint = value;
        }

        internal LinkedListNode<Point3D> GetFirstPoint()
        {
            return circlePoint[0];
        }

        public Boolean IsTheFirstPointTheSameAsTheLastPoint()
        {
            double threshold = 40;

            double xDifference = Math.Abs(circlePoint[0].Value.GetX() - circlePoint[circlePoint.Length - 1].Value.GetX());
            double yDifference = Math.Abs(circlePoint[0].Value.GetY() - circlePoint[circlePoint.Length - 1].Value.GetY());
            if(xDifference < threshold && yDifference < threshold)
            {
                firstPointInterpolated = new Point3D((circlePoint[0].Value.GetX() + circlePoint[circlePoint.Length - 1].Value.GetX()) / 2, (circlePoint[0].Value.GetY() + circlePoint[circlePoint.Length-1].Value.GetY()) / 2, (circlePoint[0].Value.GetZ() + circlePoint[circlePoint.Length - 1].Value.GetZ()) / 2);
                return true;
            }
            return false;
            
        }

        private bool IsTheCircleBigEnough(double shoulderReference)
        {
            diameter[0] = Point3D.distanceBetweenTwoPoints(firstPointInterpolated, circlePoint[numberOfCirclePoints / 2].Value);
            for (int i=1 ; i < diameter.Length ; i++)
            {
                diameter[i] = Point3D.distanceBetweenTwoPoints(circlePoint[i].Value, circlePoint[i+ (numberOfCirclePoints / 2)].Value);
            }


            
            for (int i = 0;i< diameter.Length;i++)
            {
            }


            for(int i=0;i< diameter.Length; i++)
            {
                if (diameter[i] < (shoulderReference * 3) / 4)
                    return false;
            }
            return true;
        }

        private bool IsItACircleShape()
        {
            double[] distanceBetweenPointAndCenter = new double[circlePoint.Length];
            distanceBetweenPointAndCenter[0] = Point3D.distanceBetweenTwoPoints(firstPointInterpolated, centerInterpolated);
            for (int i = 1; i < distanceBetweenPointAndCenter.Length - 1; i++)
            {
                distanceBetweenPointAndCenter[i] = Point3D.distanceBetweenTwoPoints(circlePoint[i].Value, centerInterpolated);
                Console.WriteLine("distance between point and center: " + distanceBetweenPointAndCenter[i]);
                
            }


            double[] quarterDiameters = new double [diameter.Length];
            for(int i = 0; i< diameter.Length; i++)
            {
                quarterDiameters[i] = diameter[i]/4;
            }

            for(int i = 0;i< distanceBetweenPointAndCenter.Length -1;i++)
            {
                Console.WriteLine("distance between point and center: " + distanceBetweenPointAndCenter[i]);
                Console.WriteLine("quarter diameter: " + quarterDiameters[i / 4]);
                Console.WriteLine("three quarters diameter: " + (diameter[i / 4] * 3 / 4));
                if (distanceBetweenPointAndCenter[i] < quarterDiameters[i / 4] || distanceBetweenPointAndCenter[i] > (diameter[i /4]*3/4))
                    return false;
            }
            return true;

        }

        

        public Boolean IsThisAClosedShape(double shoulderReference)
        {

            if (!IsTheFirstPointTheSameAsTheLastPoint())
                return false;
//            Console.WriteLine("heree");
            if(!IsTheCircleBigEnough(shoulderReference))
                return false;

            Console.WriteLine("hereeeeeeee");

            CalculateCenters();
            centerInterpolated = calculateMeanOfCenters();
            if (IsCneterLayingWithinCircle() == false)
                return false;

            Console.WriteLine("hererrrrrrrrrrrr");
            if(GetDetectedCircleFirstPoint() == null)
                SetDetectedCircleFirstPoint(this.GetFirstPoint().Value);
            return true;
            
        }

        private Boolean IsCneterLayingWithinCircle()
        {
            if (centerInterpolated.GetX() < minPoint.GetX() || centerInterpolated.GetY() < minPoint.GetY() || centerInterpolated.GetX() > maxPoint.GetX() || centerInterpolated.GetY() > maxPoint.GetY())
                return false;
            return true;
        }

        //0 = not a circle nor a rectangle
        //1 = circle
        //2 = rectangle
        public int IsThisCircleOrRectangle(double shoulderReference, string path)
        {
            if(createImage == false)
            {
                coloredGestureImage = new ColoredGestureImage(path + "as Circle", gestureCounter);
                createImage = true;
                gestureCounter++;
            }
            
            if (!IsThisAClosedShape(shoulderReference) || centerInterpolated.GetX() < 0 || centerInterpolated.GetY() < 0)
                return 0;


            double[] slopes = CalculateSlopes();

            int slope = AreSlopesRectangleOrCircle(slopes);

            //if the center really lies within the circle
            if (IsItACircleShape() && slope == 1)
                return 1;


            //To replace the first point with the interpolated one
            circlePoint[0].Value = firstPointInterpolated;
       //     coloredGestureImage.DrawLines(1, circlePoint);
       //     coloredGestureImage.SaveImage();
            //    if(IsThisARectangle() && slope == 2)
            if (slope == 2)
                return 2;

            return 0;
        }

        private int AreSlopesRectangleOrCircle(double[] slopes)
        {
            double threshold = 0.3;
            int conterThreshold = numberOfCirclePoints * 3/4;
            int counter = 0;

            

            for (int i = 0; i < slopes.Length; i++)
            {
                Console.WriteLine("slopes: " + slopes[i]);
                if (slopes[i] < threshold)
                    counter++;
            }
            Console.WriteLine("threshold counter: " + counter);
            if (counter >= conterThreshold)
                return 2;
            else
                return 1;
        }

        private double[] CalculateSlopes()
        {
            double[] slopes = new double[circlePoint.Length];
            for (int i = 0; i < circlePoint.Length - 1; i++)
            {
                slopes[i] = CalculateSlope(circlePoint[i].Value, circlePoint[i + 1].Value);
            }
            slopes[slopes.Length - 1] = CalculateSlope(circlePoint[0].Value, circlePoint[circlePoint.Length - 1].Value);

            return slopes;
        }

        private bool IsThisARectangle()
        {
            double[] distanceBetweenPointAndCenter = new double[circlePoint.Length];
            distanceBetweenPointAndCenter[0] = Point3D.distanceBetweenTwoPoints(firstPointInterpolated, centerInterpolated);
            for (int i = 1; i < distanceBetweenPointAndCenter.Length; i++)
            {
                distanceBetweenPointAndCenter[i] = Point3D.distanceBetweenTwoPoints(circlePoint[i].Value, centerInterpolated);
            }


            double[] quarterDiameters = new double[diameter.Length];
            for (int i = 0; i < diameter.Length; i++)
            {
                quarterDiameters[i] = diameter[i] / 4;
            }

            for (int i = 0; i < distanceBetweenPointAndCenter.Length; i++)
            {
                if (distanceBetweenPointAndCenter[i] < quarterDiameters[i / 4])
                    return false;
            }


           
            return true;
        }

        private static double CalculateSlope(Point3D point1, Point3D point2)
        {
            double dx = Math.Abs(point1.GetX() - point2.GetX()); 
            double dy = Math.Abs(point1.GetY() - point2.GetY());
            if (dx < dy)
                return dx / dy;
            else
                return dy / dx;
        }

        private double[] IsThisARectangleCandidate()
        {
            double[] sides = new double[4];

            sides[0] = Point3D.distanceBetweenTwoPoints(firstPointInterpolated, circlePoint[3].Value);
            sides[1] = Point3D.distanceBetweenTwoPoints(circlePoint[3].Value, circlePoint[4].Value);
            sides[2] = Point3D.distanceBetweenTwoPoints(circlePoint[4].Value, circlePoint[7].Value);
            sides[3] = Point3D.distanceBetweenTwoPoints(circlePoint[7].Value, firstPointInterpolated);

            double side1 = sides[0] / sides[2];
            double side2 = sides[1] / sides[3];


                if (side1 < 0.6 || side1 > 1.4)
                return null;
            if (side2 < 0.6 || side2 > 1.4)
                return null;

            return sides;
        }

        private bool IsThisALongRectangle(double[] sides)
        {
            double wideSideInterpolation = (sides[1] + sides[3]) / 2;
            double shortSideInterpolation = (sides[0] + sides[2]) / 2;
            Console.WriteLine("wide side: " + wideSideInterpolation);
            Console.WriteLine("short side: " + shortSideInterpolation);

            if (shortSideInterpolation / wideSideInterpolation > 0.9)
                return false;




            double firstSideThreshold = sides[1] / 2;
            double distanceBetweenThirdPointAndSide = ShortestDistance(circlePoint[1].Value, circlePoint[4].Value, circlePoint[2].Value);
            double distanceBetweenFourthPointAndSide = ShortestDistance(circlePoint[1].Value, circlePoint[4].Value, circlePoint[3].Value);


            Console.WriteLine("first Side Threshold: " + firstSideThreshold);
            Console.WriteLine("distanceBetweenThirdPointAndSide: " + distanceBetweenThirdPointAndSide);
            Console.WriteLine("distanceBetweenFourthPointAndSide: " + distanceBetweenFourthPointAndSide);


            if (distanceBetweenThirdPointAndSide > firstSideThreshold || distanceBetweenFourthPointAndSide > firstSideThreshold)
                return false;

            double secondSideThreshold = sides[3] / 2;
            double distanceBetweenSeventhPointAndSide = ShortestDistance(circlePoint[5].Value, firstPointInterpolated, circlePoint[6].Value);
            double distanceBetweenEighthPointAndSide = ShortestDistance(circlePoint[5].Value, firstPointInterpolated, circlePoint[7].Value);


            Console.WriteLine("secondSideThreshold: " + secondSideThreshold);
            Console.WriteLine("distanceBetweenSeventhPointAndSide: " + distanceBetweenSeventhPointAndSide);
            Console.WriteLine("distanceBetweenEighthPointAndSide: " + distanceBetweenEighthPointAndSide);


            if (distanceBetweenSeventhPointAndSide > secondSideThreshold || distanceBetweenEighthPointAndSide > secondSideThreshold)
                return false;

            return true;
        }

        private bool IsThisAWideRectangle(double [] sides)
        {
            double wideSideInterpolation = (sides[0] + sides[2]) / 2;
            double shortSideInterpolation = (sides[1] + sides[3]) / 2;

            Console.WriteLine("wide side: " + wideSideInterpolation);
            Console.WriteLine("short side: " + shortSideInterpolation);

            if (shortSideInterpolation / wideSideInterpolation > 0.9)
                return false;


            double firstSideThreshold = sides[0] / 2;
            double distanceBetweenSecondPointAndSide = ShortestDistance(firstPointInterpolated, circlePoint[3].Value, circlePoint[1].Value);
            double distanceBetweenThirdPointAndSide = ShortestDistance(firstPointInterpolated, circlePoint[3].Value, circlePoint[2].Value);

            Console.WriteLine("first Side Threshold: " + firstSideThreshold);
            Console.WriteLine("distanceBetweenSecondPointAndSide: " + distanceBetweenSecondPointAndSide);
            Console.WriteLine("distanceBetweenThirdPointAndSide: " + distanceBetweenThirdPointAndSide);

            if (distanceBetweenSecondPointAndSide > firstSideThreshold || distanceBetweenThirdPointAndSide > firstSideThreshold)
                return false;

            double secondSideThreshold = sides[2] / 2;
            double distanceBetweenSixthPointAndSide = ShortestDistance(circlePoint[4].Value, circlePoint[7].Value, circlePoint[5].Value);
            double distanceBetweenSeventhPointAndSide = ShortestDistance(circlePoint[4].Value, circlePoint[7].Value, circlePoint[6].Value);

            Console.WriteLine("secondSideThreshold: " + secondSideThreshold);
            Console.WriteLine("distanceBetweenSixthPointAndSide: " + distanceBetweenSixthPointAndSide);
            Console.WriteLine("distanceBetweenSeventhPointAndSide: " + distanceBetweenSeventhPointAndSide);

            if (distanceBetweenSixthPointAndSide > secondSideThreshold || distanceBetweenSeventhPointAndSide > secondSideThreshold)
                return false;

            return true;
        }




        //point1, point2 are the line and point 3 is the target
        private static double ShortestDistance(Point3D point1, Point3D point2, Point3D point3)
        {
            double x1 = point1.GetX();
            double y1 = point1.GetY();
            double x2 = point2.GetX();
            double y2 = point2.GetY();
            double x3 = point3.GetX();
            double y3 = point3.GetY();
            double px = x2 - x1;
            double py = y2 - y1;
            double temp = (px * px) + (py * py);
            double u = ((x3 - x1) * px + (y3 - y1) * py) / (temp);
            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }
            double x = x1 + u * px;
            double y = y1 + u * py;

            double dx = x - x3;
            double dy = y - y3;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;

        }

        private void CalculateCenters()
        {

            for(int i = 0; i< center.Length;i++)
            {
                if(i==0)
                    center[i] = CalculateCenterPoint(firstPointInterpolated, circlePoint[i + (numberOfCirclePoints / 4)].Value, circlePoint[i + (numberOfCirclePoints / 2)].Value, circlePoint[i + ((numberOfCirclePoints * 3) / 4)].Value);
                else
                    center[i] = CalculateCenterPoint(circlePoint[i].Value, circlePoint[i + (numberOfCirclePoints / 4)].Value, circlePoint[i + (numberOfCirclePoints / 2)].Value, circlePoint[i + ((numberOfCirclePoints * 3) / 4)].Value);

                Console.WriteLine("center " + i + ": " + center[i].ToString());
            }
        }

        //  Returns Point of intersection if do intersect otherwise default Point (null)
        public static Point3D CalculateCenterPoint(Point3D point1, Point3D point2, Point3D point3, Point3D point4 , double tolerance = 0.00001)
        {
            double x1 = point1.GetX(), y1 = point1.GetY();
            double x2 = point3.GetX(), y2 = point3.GetY();

            double x3 = point2.GetX(), y3 = point2.GetY();
            double x4 = point4.GetX(), y4 = point4.GetY();


            // equations of the form x = c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
            {
                throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
            {
                throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
            }

            //equations of the form x=c (two vertical parallel lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
            {
                //return default (no intersection)
                return new Point3D(-99,-99,-99);
            }

            //equations of the form y=c (two horizontal parallel lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
            {
                //return default (no intersection)
                return new Point3D(-99, -99, -99); ;
            }

            //general equation of line is y = mx + c where m is the slope
            //assume equation of line 1 as y1 = m1x1 + c1 
            //=> -m1x1 + y1 = c1 ----(1)
            //assume equation of line 2 as y2 = m2x2 + c2
            //=> -m2x2 + y2 = c2 -----(2)
            //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
            //so we will get below two equations 
            //-m1x + y = c1 --------(3)
            //-m2x + y = c2 --------(4)

            double x, y;

            //lineA is vertical x1 = x2
            //slope will be infinity
            //so lets derive another solution
            if (Math.Abs(x1 - x2) < tolerance)
            {
                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x1=c1=x
                //subsitute x=x1 in (4) => -m2x1 + y = c2
                // => y = c2 + m2x1 
                x = x1;
                y = c2 + m2 * x1;
            }
            //lineB is vertical x3 = x4
            //slope will be infinity
            //so lets derive another solution
            else if (Math.Abs(x3 - x4) < tolerance)
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x3=c3=x
                //subsitute x=x3 in (3) => -m1x3 + y = c1
                // => y = c1 + m1x3 
                x = x3;
                y = c1 + m1 * x3;
            }
            //lineA & lineB are not vertical 
            //(could be horizontal we can handle it with slope = 0)
            else
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                //verify by plugging intersection point (x, y)
                //in orginal equations (1) & (2) to see if they intersect
                //otherwise x,y values will not be finite and will fail this check
                if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                    && Math.Abs(-m2 * x + y - c2) < tolerance))
                {
                    //return default (no intersection)
                    return new Point3D(-99, -99, -99);
                }
            }

            //x,y can intersect outside the line segment since line is infinitely long
            //so finally check if x, y is within both the line segments
            if (IsInsideLine(point1, point3, x, y) &&
                IsInsideLine(point2, point4, x, y))
            {
                return new Point3D(x,y,point1.GetZ());
            }

            //return default (no intersection)
            return new Point3D(-99, -99, -99);

        }

        // Returns true if given point(x,y) is inside the given line segment
        private static bool IsInsideLine(Point3D firstPoint, Point3D secondPoint, double x, double y)
        {
            return (x >= firstPoint.GetX() && x <= secondPoint.GetX() 
                        || x >= secondPoint.GetX() && x <= firstPoint.GetX())
                   && (y >= firstPoint.GetY() && y <= secondPoint.GetY()
                        || y >= secondPoint.GetY() && y <= firstPoint.GetY());
        }

        public static int factorial(int n)
        {
            int factorial = 1;
            for(int i=1; i<=n; i++)
            {
                factorial *= i;
            }
            return factorial;
        }

        public Point3D calculateMeanOfCenters()
        {
            double xCenter = 0;
            double yCenter = 0;
            for(int i=0; i< this.center.Length; i++)
            {
                if(center[i].GetX() > 0 && center[i].GetY()>0)
                {
                    xCenter += center[i].GetX();
                    yCenter += center[i].GetY();
                }

            }
            xCenter /= center.Length;
            yCenter /= center.Length;
            Point3D newCenter = new Point3D(xCenter, yCenter, 0);
            return newCenter;
        }



    }
}
