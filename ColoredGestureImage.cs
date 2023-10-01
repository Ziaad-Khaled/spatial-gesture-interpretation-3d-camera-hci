using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class ColoredGestureImage : GestureImage
    {

        // Create a Pen
        private Pen preperationPen = new Pen(Color.FromArgb(255,0,0,255), 10);
        private Pen nucleusPen = new Pen(Color.FromArgb(255, 0, 255, 0), 10);
        private Pen retractionPen = new Pen(Color.FromArgb(255, 255, 0, 0), 10);


        GreyscaleGestureImage greyImage;

        // not recognized: -1
        // no swipe: 0
        //Swipe left: 1
        //Swipe Right: 2
        //swipe up: 3
        //swipe down: 4
        int currentSwipe = -2;
        int previousSwipe = -2;
        int previousDoneSwipe = -1;
        Boolean swipeChangedFlag = false;
        int changes = 0;
        Boolean isASwipe = false;
        int notSureCounter = 0;
        Point3D retractionFirstNodeCandidate; 

        public ColoredGestureImage(string pathToGesture, int gestureCounter)
        {
            base.Bitmap = new Bitmap(350, 350, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            base.Graphics = Graphics.FromImage(base.Bitmap);
            base.Path = pathToGesture + "\\Human Images\\" + gestureCounter + ".png";

            greyImage = new GreyscaleGestureImage(pathToGesture, gestureCounter);
        }
        


        // type 0 : preperation
        // type 1 : nucleus
        // type 2 : retraction
        public void DrawLine(int type, Point3D point1In3D, Point3D point2In3D)
        {
            Point point1 = Point3D.ConvertTo2DPoint(point1In3D);
            Point point2 = Point3D.ConvertTo2DPoint(point2In3D);

            switch (type)
            {
                case 0:
                    //change thickness of the pen for CNN
                    base.Graphics.DrawLine(preperationPen, point1, point2);
                    break;
                case 1:
                    base.Graphics.DrawLine(nucleusPen, point1, point2);
                    break;
                case 2:
                    base.Graphics.DrawLine(retractionPen, point1, point2);
                    break;
            }
        }

        public void DrawLine(Pen pen, Point point1, Point point2)
        {
            base.Graphics.DrawLine(pen, point1, point2);
        }

        // for drawing the image for the machine learning approach
        public void DrawLines(int type, LinkedList<Point3D> list, Point3D averageOfPoints, int closedShape)
        {
            
            switch (type)
            {
                case 0:
                    base.PreperationLinkedList = new LinkedList<Point3D>();
                    LinkedListNode<Point3D> currentNode = list.First;
                    while (currentNode != null)
                    {
                        Point3D newPoint = new Point3D(currentNode.Value.GetX(), currentNode.Value.GetY(), currentNode.Value.GetZ());
                        base.PreperationLinkedList.AddLast(newPoint);
                        currentNode = currentNode.Next;
                    }

                    break;
                case 1:
                    Console.WriteLine(base.NucleusLinkedList.Count);

                    //an error happens when the length of the list is only 1
                    if (base.PreperationLinkedList.Count == 1)
                        base.PreperationLinkedList.AddLast(list.First.Value);

                    //first calculate the new nucleus linkedlist after seeing if the last swipe down a retraction
                    Console.WriteLine("average of points: " + averageOfPoints.ToString());
                    averageOfPoints = SetNewLinkedLists(list, closedShape);
                   

                    Console.WriteLine("average of points: " + averageOfPoints.ToString());


                    //then continue to draw the colored image lines as usual
                    if(base.PreperationLinkedList.Count != 0)
                    {
                        base.DrawLinesWithDifferientOpacityAndColor(1, base.PreperationLinkedList, averageOfPoints);
                        Console.WriteLine("last preperation point: " + base.PreperationLinkedList.Last.Value.ToString());
                    }

                    base.DrawLinesWithDifferientOpacityAndColor(2, base.NucleusLinkedList, averageOfPoints);
                        Console.WriteLine("First Nuclues point: " + base.NucleusLinkedList.First.Value.ToString());


                    //save the image as greyscale
                    greyImage.DrawLinesWithDifferientOpacityAndColor(0, base.NucleusLinkedList, averageOfPoints);
                   
                    //save average to be used later in retraction
                    base.NucleusAverage = new Point3D(averageOfPoints.GetX(), averageOfPoints.GetY(), averageOfPoints.GetZ());


                    break;
                case 2:
                    Console.WriteLine("preparation linked list: " + base.PreperationLinkedList.Count);
                    Console.WriteLine("nucleus linked list count: " + base.NucleusLinkedList.Count);
                    Console.WriteLine("retraction linked list count: " + (base.RetractionLinkedList.Count + list.Count));

                    base.DrawLinesWithDifferientOpacityAndColor(3, list, base.NucleusAverage);
                    if(base.RetractionLinkedList.Count != 0)
                        base.DrawLinesWithDifferientOpacityAndColor(3, base.RetractionLinkedList, base.NucleusAverage);

                    this.SaveImage();

                    break;
            }
        }

        private Point3D SetNewLinkedLists(LinkedList<Point3D> list, int closedShape)
        {
         
            Point3D averageOfPoints = new Point3D(0, 0, 0);
            Point3D sumOfPoints = new Point3D(0, 0, 0);

            LinkedListNode<Point3D> currentNode = list.First;


            Point3D firstPoint = currentNode.Value;

            LinkedListNode<Point3D> previousNode = null;
            int slopeDecider = 0;

            int i = 0;
            double ratio = 0;
            switch(closedShape)
            {
                case 0:
                    ratio = (double)list.Count / 2.0;
                    break;
                case 1:
                    ratio = (double)list.Count / 3.2;
                    slopeDecider = 3;
                    break;
                case 2:
                    ratio = (double)list.Count / 3.8;
                    slopeDecider = 3;
                    break;

            }
            Console.WriteLine("ratiooooooooooo: " + ratio);
            int noise = list.Count - (int) ratio;
            Console.WriteLine("noise: " + noise);
            Console.WriteLine("count: " + list.Count);

            Point3D totalSlope = new Point3D(0, 0, 0);
            double threshold = 15;
            int count = 0;
            Boolean firstNodeFlag = false;
            Boolean retractionFirstNodeDecided = true;
            Point3D retractionFirstNode = list.Last.Value;
            Boolean retractionCandidateAssigned = false;

            Boolean changedFlag = false;

            while ((currentNode != null))
            {
                if(retractionFirstNode != null)
                    Console.WriteLine("retraction first node: " + retractionFirstNode.ToString());
                // check if there is a change in acceleration;
                if (previousNode != null)
                {
                    count++;
                    Console.WriteLine("count: " + count);
                    Point3D slope = new Point3D(currentNode.Value.GetX() - previousNode.Value.GetX(), currentNode.Value.GetY() - previousNode.Value.GetY(), currentNode.Value.GetZ() - previousNode.Value.GetZ());
                    //       Point3D unitVectorSlope = HandMotionData.UnitVector(slope);
                    Point3D unitVectorSlope = slope;
                    Console.WriteLine((totalSlope.GetX() * (count - 1) / count));
                    totalSlope.SetNewPoints((totalSlope.GetX()*(count-1)/count) + (unitVectorSlope.GetX() / count), (totalSlope.GetY() * (count - 1) / count) + (unitVectorSlope.GetY() / count), (totalSlope.GetZ() * (count - 1) / count) + (unitVectorSlope.GetZ() / count));


                    double xDifference = Math.Abs(totalSlope.GetX() - unitVectorSlope.GetX());
                    double yDifference = Math.Abs(totalSlope.GetY() - unitVectorSlope.GetY());

                    Console.WriteLine("X Difference: " + xDifference);
                    Console.WriteLine("y Difference: " + yDifference);

                    Console.WriteLine((totalSlope.GetX() >= 0) ^ (unitVectorSlope.GetX() < 0));



                    Console.WriteLine("count: " + count);
                    Console.WriteLine("noise: " + noise);
                    if (CheckSwipeType(unitVectorSlope) && count < noise && changes > 1)
                    {
                        Console.WriteLine("retraaaaaaaaction first node: " + currentNode.Value.ToString());
                        retractionFirstNode = currentNode.Value;
                        retractionCandidateAssigned = true;

                    }
                    /*
                    if(swipeChangedFlag && isASwipe && retractionCandidateAssigned)
                    {
                        
                        changedFlag = true;
                        totalSlope = unitVectorSlope;
                        retractionFirstNode = retractionFirstNodeCandidate;
                        firstNodeFlag = true;
                        retractionFirstNodeDecided = true;
                        swipeChangedFlag = false;
                        retractionCandidateAssigned = false;
                    }
                        */

                    Console.WriteLine("unit vector slope: " + unitVectorSlope.ToString());
                    Console.WriteLine("total slope: " + totalSlope.ToString());
                    Console.WriteLine("changes: " + changes);
                    Console.WriteLine("slope decider: " + slopeDecider);
                    /*
                    switch (slopeDecider)
                    {
                        //deciding the type of the swipe
                        case 0:
                            if (i > 5)
                            {
                                Point3D point= new Point3D(currentNode.Value.GetX() - firstPoint.GetX(), currentNode.Value.GetY() - firstPoint.GetY(), currentNode.Value.GetZ() - firstPoint.GetZ());
                                Boolean flag = IsSlopeUpOrRight(point);
                                if (changes < 1 && flag)
                                {
                                    slopeDecider = 1;
                                    retractionFirstNodeDecided = false;
                                }
                                else
                                {
                                    slopeDecider = 2;
                                }
                                
                            }
                            
                            break;
                        // when the first swipe down done for swipe right
                        case 1:
                            if(changes > 0 && changedFlag)
                            {
                                if (currentNode.Next != null)
                                    retractionFirstNode = currentNode.Next;
                                else
                                    retractionFirstNode = currentNode;
                                firstNodeFlag = true;
                                base.RetractionLinkedList.Clear();
                                retractionFirstNodeDecided = true;
                                changedFlag = false;
                                threshold += 15;
                            }
                            break;

                        case 2:
                            if (changes > 0 && changedFlag)
                            {
                                if (currentNode.Next != null)
                                    retractionFirstNode = currentNode.Next;
                                else
                                    retractionFirstNode = currentNode;
                                firstNodeFlag = true;
                                base.RetractionLinkedList.Clear();
                                retractionFirstNodeDecided = true;
                                changedFlag = false;
                                threshold += 1;
                                if (threshold > 19)
                                {
                                    slopeDecider = 4;
                                    retractionFirstNodeDecided = true;
                                }
                            }
                            break;
                        // not a swipe up or down
                        case 3:
                            retractionFirstNodeDecided = false;
                            count = 0;
                            totalSlope.Reset();
                            break;
                        case 4:
                            if (changes > 1)
                            {
                                if (currentNode.Next != null)
                                    retractionFirstNode = currentNode.Next;
                                else
                                    retractionFirstNode = currentNode;
                                firstNodeFlag = true;
                                base.RetractionLinkedList.Clear();
                                retractionFirstNodeDecided = true;
                            }
                            break;
                    }
                    */
                }
                /*
                if (!retractionFirstNodeDecided)
                    {
                        int decider = IsFirstNodeInSwipeDown(previousNode.Value, currentNode.Value, firstNodeFlag);
                        switch (decider)
                        {
                            case 0:
                                retractionFirstNode = currentNode;
                                firstNodeFlag = true;
                                base.RetractionLinkedList.Clear();
                                retractionFirstNodeDecided = true;
                                break;
                            case 2:
                                firstNodeFlag = false;
                                break;
                        }
                    }
                */
                //preperaring for the next iteration
                previousNode = currentNode;
                currentNode = currentNode.Next;
                i++;
            }
            Console.WriteLine("nucleus count: " + base.NucleusLinkedList.Count);
            Console.WriteLine("list count: " + list.Count);
            Console.WriteLine("RetractionLinkedList count: " + base.RetractionLinkedList.Count);
            //edit the linkedlists links to begin retraction accuratly
            LinkedListNode<Point3D> retractionCurrentNode;
            i = 0;
            Console.WriteLine("list count: " + list.Count);
            retractionCurrentNode = list.First;
            while (retractionCurrentNode != null)
             {

                sumOfPoints.SetNewPoints(sumOfPoints.GetX() + retractionCurrentNode.Value.GetX(), sumOfPoints.GetY() + retractionCurrentNode.Value.GetY(), sumOfPoints.GetZ() + retractionCurrentNode.Value.GetZ());
                averageOfPoints.SetNewPoints(sumOfPoints.GetX() / (i + 1), sumOfPoints.GetY() / (i + 1), sumOfPoints.GetZ() / (i + 1));
                base.NucleusLinkedList.AddLast(retractionCurrentNode.Value);
                if (retractionCurrentNode.Value.Equals(retractionFirstNode))
                    break;
                retractionCurrentNode = retractionCurrentNode.Next;
                i++;
            }
            Console.WriteLine("hereeeeeeeeee");
            while (retractionCurrentNode != null)
            {
                base.RetractionLinkedList.AddLast(retractionCurrentNode.Value);
                retractionCurrentNode = retractionCurrentNode.Next;
            }
                Console.WriteLine("list count: " + list.Count);
            /*
            else
            {
                LinkedListNode<Point3D> nucleusNode = list.First;
                while (nucleusNode != null)
                {
                    base.NucleusLinkedList.AddLast(nucleusNode.Value);
                    sumOfPoints.SetNewPoints(sumOfPoints.GetX() + nucleusNode.Value.GetX(), sumOfPoints.GetY() + nucleusNode.Value.GetY(), sumOfPoints.GetZ() + nucleusNode.Value.GetZ());
                    averageOfPoints.SetNewPoints(sumOfPoints.GetX() / (i + 1), sumOfPoints.GetY() / (i + 1), sumOfPoints.GetZ() / (i + 1));
                    nucleusNode = nucleusNode.Next;
                    i++;
                }
                base.RetractionLinkedList.Clear();
            }
            */
            return averageOfPoints;
        }

        private Boolean CheckSwipeType(Point3D instantVelocity)
        {

            if(currentSwipe != -1)
            {
                previousSwipe = currentSwipe;
            }


            if (instantVelocity.GetX() > 8 && Math.Abs(instantVelocity.GetX()) > 7 + Math.Abs(instantVelocity.GetY()))
            {
                currentSwipe = 2;
            }
            else if (instantVelocity.GetX() < -8 && Math.Abs(instantVelocity.GetX()) > 7 + Math.Abs(instantVelocity.GetY()) )
            {
                currentSwipe = 1;
            }
            else if (instantVelocity.GetY() < -10 && Math.Abs(instantVelocity.GetY()) > 7 + Math.Abs(instantVelocity.GetX()))
            {
                currentSwipe = 3;
            }
            else if (instantVelocity.GetY() > 10 && Math.Abs(instantVelocity.GetY()) > 7 + Math.Abs(instantVelocity.GetX()))
            {
                currentSwipe = 4;
            }
            else if (Math.Abs(instantVelocity.GetX()) < 7 && Math.Abs(instantVelocity.GetY()) < 7 && Math.Abs(instantVelocity.GetX()) + Math.Abs(instantVelocity.GetY()) < 8)
                currentSwipe = 0;
            else
            {
                currentSwipe = previousSwipe;
                notSureCounter++;
            }

            if (currentSwipe != 0)
                isASwipe = true;
            else
                isASwipe = false;

            Console.WriteLine("current swipe: " + currentSwipe);
            Console.WriteLine("previous swipe: " + previousSwipe);
            Console.WriteLine("swipe changed flag: " + swipeChangedFlag);
            Console.WriteLine("previous done swipe: " + previousDoneSwipe);
            Console.WriteLine("noSureCounter: " + notSureCounter);

            if (swipeChangedFlag && currentSwipe == previousSwipe)
            {
                Boolean flag = true;
                changes++;
                swipeChangedFlag = false;
                notSureCounter = 0;
                if (previousDoneSwipe == 0 || (notSureCounter > 2))
                    flag = false;

                previousDoneSwipe = currentSwipe;

 
                return flag;
            }

            if (currentSwipe != previousSwipe && previousDoneSwipe != currentSwipe)
                swipeChangedFlag = true;
            else
                swipeChangedFlag = false;

            return false;
        }

        private int IsFirstNodeInSwipeDown(Point3D previousPoint, Point3D currentPoint, Boolean firstNodeFlag)
        {
            Console.WriteLine("current point: " + currentPoint.ToString()); 
            double yDelta = currentPoint.GetY() - previousPoint.GetY();
            double xDelta = currentPoint.GetX() - previousPoint.GetX();
            Console.WriteLine("xDelta: " + xDelta);
            Console.WriteLine("yDelta: " + yDelta);

            //case 0: first node is not assigned yet
            //case 1: first node assigned and this a continue of the same swipe down
            //case 2: the swipe down ended
            if (yDelta > 10)
                if (!firstNodeFlag)
                    return 0;
                else
                    return 1;
            else if (yDelta < 0)
                return 2;
            else
                return 1;



        }

        public Boolean IsSlopeUpOrRight(Point3D point)
        {
            Console.WriteLine("first and lastpoint difference: " + point.ToString());
            if (Math.Abs(point.GetX()) > Math.Abs(point.GetY()))
                return true;
            else
                return false;
        }





        //for drawing the image of the interpolated circle or rectangle
        public void DrawLines(int type, LinkedListNode<Point3D>[] list)
        {
            Point[] points = Point3D.ConvertTo2DPoints(list);
            Console.WriteLine("hereeee");
            //base.NucleusLinkedList.AddLast(list[0].Value);
            switch (type)
            {
                case 0:
                    base.Graphics.DrawLines(preperationPen, points);
                    break;
                case 1:
                    base.Graphics.DrawLines(nucleusPen, points);
                    break;
                case 2:
                    base.Graphics.DrawLines(retractionPen, points);
                    break;
            }
            SaveImageForRotation();
        }

        public void SaveImage()
        {

            if(base.NucleusLinkedList.Count > 0)
            {
                base.SaveImage();
                greyImage.SaveImage();
            }     
        }
        public void SaveImageForRotation()
        {
            base.SaveImage();
        }


    }
}
