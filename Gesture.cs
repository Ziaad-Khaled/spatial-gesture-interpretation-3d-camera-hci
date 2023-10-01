using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class Gesture
    {
        public enum State
        {
            NoGesture,
            Preperation,
            Nucleus,
            Retraction
        }

        private State rightHandState = State.NoGesture;

        private State leftHandState = State.NoGesture;


        //0 : swipe left
        //1: swipe right
        //2 : rotate
        //3 : swipe up
        //4: swpipe down
        //5: rectangle
        private int[] totalNumberOfGestures = { 0, 0, 0, 0, 0, 0 };


        private int swipeDownCounter;

        CircleOrRectangle circle;

        private int numberOfGestures = 6;

        private Boolean[] gesturesDictonery = {false, false, false, false, false, false};
        //0 : swipe left
        //1: swipe right
        //2 : rotate
        //3 : swipe up
        //4: swpipe down
        //5: rectangle
        private int[] gesturesCounter = { 0, 0, 0, 0, 0, 0};

        private LinkedList<String> lastMadeGestures = new LinkedList<String>();


        private string lastMadeGesture;

        //threholds
        private double swipeLeftThreshold = -9;
        private double swipeRightThreshold = 9;
        private double swipeUpThreshold = -8;
        private double swipeDownThreshold = 9;
        private double otherGesturesThreshold = 4;
        private double gestureEndedThreshold = 2;

        private int gestureCounter = 0;
        private int numberOfFrames = 0;

        private Boolean vaildGesture = true;

        private LinkedList<String> madeGesturesWithinNucleus = new LinkedList<String>();
        private LinkedList<String> madeGesturesWithinNucleusShortened = new LinkedList<string>();
        private LinkedList<int> madeGesturesWithinNucleusCounter = new LinkedList<int>();

        public LinkedList<string> MadeGesturesWithinNucleus { get => madeGesturesWithinNucleus; set => madeGesturesWithinNucleus = value; }
        public int[] TotalNumberOfGestures { get => totalNumberOfGestures; set => totalNumberOfGestures = value; }

        internal State GetLeftHandState1()
        {
            return leftHandState;
        }

        internal void SetLeftHandState1(State value)
        {
            leftHandState = value;
        }

        public State GetLeftHandState()
        {
            return leftHandState;
        }

        public void SetLeftHandState(State value)
        {
            leftHandState = value;
        }

        public string GetLastMadeGesture()
        {
            return lastMadeGesture;
        }

        public void SetLastMadeGesture(string value)
        {
            lastMadeGesture = value;
        }

        public LinkedList<string> GetLastMadeGestures()
        {
            return lastMadeGestures;
        }

        public void SetLastMadeGestures(LinkedList<string> value)
        {
            lastMadeGestures = value;
        }

        public State GetRightHandState()
        {
            return rightHandState;
        }

        public void SetRightHandState(State value)
        {
            rightHandState = value;
        }

        
       

        public bool IsThereAGesture()
        {
            if(rightHandState == State.NoGesture)
                return false;
            else
                return true;
        }

        

        
        public void KillActiveGesture(int gestureNumber)
        {
            gesturesDictonery[gestureNumber] = false;
            gestureNumber--;
        }

        public Boolean[] GetGesturesDictonery()
        {
            return gesturesDictonery;
        }

       

        public void SetRightHandNoGesture()
        {
            this.rightHandState = State.NoGesture;
        }
        public void SetLeftHandNoGesture()
        {
            this.leftHandState = State.NoGesture;
        }
        public ColoredGestureImage SetStateToPreperation(RightHandMotionData rightHandMotionData, string path, int counter)
        {
            this.MadeGesturesWithinNucleus.Clear();
            this.madeGesturesWithinNucleusShortened.Clear()
;           this.madeGesturesWithinNucleusCounter.Clear();
            this.numberOfFrames = 0;


            this.rightHandState = State.Preperation;
            rightHandMotionData.FlushData();
            ColoredGestureImage coloredImage = new ColoredGestureImage(path, counter);

            return coloredImage;
        }
        public ColoredGestureImage SetStateToPreperation(LeftHandMotionData leftHandMotionData, string path, int counter)
        {
            this.MadeGesturesWithinNucleus.Clear();
            this.madeGesturesWithinNucleusShortened.Clear()
;           this.madeGesturesWithinNucleusCounter.Clear();
            this.numberOfFrames = 0;


            this.leftHandState = State.Preperation;
            leftHandMotionData.FlushData();
            ColoredGestureImage coloredImage = new ColoredGestureImage(path, counter);

            return coloredImage;
        }


        public void SetStateToNucleus(LinkedListNode<Point3D> point, int condition)
        {
            circle = new CircleOrRectangle(point);
            this.gesturesDictonery = new Boolean[numberOfGestures];
            this.gesturesCounter = new int[numberOfGestures];
            for(int i = 0; i < numberOfGestures; i++)
            {
                this.gesturesDictonery[i]= false;
                this.gesturesCounter[i]= 0;
            }
            //0: right hand 
            //1: left hand
            switch(condition)
            {
                case 0:
                    this.rightHandState = State.Nucleus;
                    break;
                case 1:
                    this.leftHandState = State.Nucleus;
                    break;
            }
            
            
        }

        public void SetStateToRetraction(LinkedList<Point3D> nucleusLinkedList, int condition)
        {
            

            switch (condition)
            {
                case 0:
                    this.rightHandState = State.Retraction;
                    break;
                case 1:
                    this.leftHandState = State.Retraction;
                    break;
            }

            //check which microgestures were done
            LinkedListNode<Point3D> node = nucleusLinkedList.First;
            LinkedListNode<Point3D> nextNode = node.Next;
            while (nextNode != null)
            {
                Point3D instantVelcoity = new Point3D(nextNode.Value.GetX() - node.Value.GetX(), nextNode.Value.GetY() - node.Value.GetY(), nextNode.Value.GetZ() - node.Value.GetZ());
                ChcekMicroGestures(instantVelcoity);
                node = node.Next;
                nextNode = nextNode.Next;
            }


            // if slope is negative then it is a swipe left or right 
            // if slope is positive then it is swipe up or down
            Boolean slope = IsSwipeLeftOrSwipeDown(nucleusLinkedList.First.Value, nucleusLinkedList.Last.Value);

            editMadeGesturesWithinNucleusShortened();
            LinkedListNode<String> currentNode = madeGesturesWithinNucleusShortened.First;
            LinkedListNode<int> currentNodeInt = madeGesturesWithinNucleusCounter.First;
            while (currentNode != null)
            {
                Console.WriteLine(currentNode.Value);
                Console.WriteLine(currentNodeInt.Value);
                currentNode = currentNode.Next;
                currentNodeInt = currentNodeInt.Next;
            }

            if (gesturesDictonery[2] == true && RotationIsOnlyDoneOnce(2))
            {
                AddToLastMadeGestures("Rotation");
                SetLastMadeGesture("Rotation");
                totalNumberOfGestures[2]++;
            }
            else
            {
                if (gesturesDictonery[5] == true && RotationIsOnlyDoneOnce(5) )
                {
                    AddToLastMadeGestures("Rectangle");
                    SetLastMadeGesture("Rectangle");
                    totalNumberOfGestures[5]++;
                }
                else
                {
                    if (gesturesCounter[0] > 0 && OnlyOneSwipeIsDone(0) && slope &&  HasAValidRatio(0))
                    {
                        AddToLastMadeGestures("Swipe Left");
                        SetLastMadeGesture("Swipe Left");
                        totalNumberOfGestures[0]++;
                    }
                    else if (gesturesCounter[1] > 0 && OnlyOneSwipeIsDone(1) && slope && HasAValidRatio(1))

                    {
                        AddToLastMadeGestures("Swipe Right");
                        SetLastMadeGesture("Swipe Right");
                        totalNumberOfGestures[1]++;
                    }

                    else if (gesturesCounter[3] > 0 && HasAValidRatio(3) && OnlyOneSwipeIsDone(3))

                    {
                        AddToLastMadeGestures("Swipe Up");
                        SetLastMadeGesture("Swipe Up");
                        totalNumberOfGestures[3]++;
                    }
                    else if (gesturesCounter[4] > 0 && HasAValidRatio(4) && OnlyOneSwipeIsDone(4))

                    {
                        AddToLastMadeGestures("Swipe Down");
                        SetLastMadeGesture("Swipe Down");
                        totalNumberOfGestures[4]++;
                    }
                    else
                    {

                        SetLastMadeGesture("null");
                    }

                }

            }
            this.numberOfFrames = 0;
            vaildGesture = true;
        }

        private void ChcekMicroGestures(Point3D instantVelcoity)
        {
            checkForSwipeLeft(instantVelcoity);
            checkForSwipeRight(instantVelcoity);
            checkForSwipeDown(instantVelcoity);
            checkForSwipeUp(instantVelcoity);
        }

        private bool IsSwipeLeftOrSwipeDown(Point3D value1, Point3D value2)
        {
            double xDelta = Math.Abs(value1.GetX() - value2.GetX());
            double yDelta = Math.Abs(value1.GetY() - value2.GetY());

            Console.WriteLine("xDelta in gesture: " + xDelta);
            Console.WriteLine("yDelta in gesture: " + yDelta);

            if (yDelta > xDelta + xDelta/10)
                return false;
            else
                return true;
        }

        private Boolean IsAValidRectangle()
        {
            int counter = 0;
            for(int i=0; i< gesturesDictonery.Length; i++)
            {
                if(gesturesDictonery[i] == true)
                    counter++;
            }
            if (counter < 4)
                return false;
            return true;
        }

        private bool RotationIsOnlyDoneOnce(int c)
        {
            if (gesturesCounter[2] > 1 || gesturesCounter[5] > 1 || (gesturesCounter[5] == 1 && gesturesCounter[2] == 1))
                return false;

            switch (c)
            {
                //I iterate over the done gestures and if a gesture after the candidate gesture was done more than 1 time it will counted as false
                // except for swiping down for retraction
                case 2:
                    LinkedListNode<String> currentNode = madeGesturesWithinNucleusShortened.First;
                    LinkedListNode<String> nextNode = currentNode.Next;
                    LinkedListNode<int> currentNodeInt = madeGesturesWithinNucleusCounter.First;


                    //left , right, up, down
                    int[] flags = new int[4];

                    int gestureCountAfterCandidateGesture = 0;

                    Boolean candidateGestureFound = false;
                    while (nextNode != null)
                    {
                        if(candidateGestureFound)
                            gestureCountAfterCandidateGesture+= currentNodeInt.Value;

                        //my gesture
                        if (currentNode.Value.Equals("Rotation"))
                            candidateGestureFound = true;

                        if(candidateGestureFound && currentNode.Value.Equals("Swipe Left"))
                        {
                            flags[0]++;
                        }

                        if(candidateGestureFound && currentNode.Value.Equals("Swipe Right"))
                        {
                            flags[1]++;
                        }
                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Up"))
                        {
                            flags[2]++;

                        }
                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Down"))
                        {
                            flags[3]++;
                        }

                        int count = 0;
                        for(int i = 0; i < flags.Length; i++)
                        {
                            if (flags[i] > 2)
                                return false;
                        }



                        //go to next node
                        currentNode = currentNode.Next;
                        nextNode = nextNode.Next;
                        currentNodeInt = currentNodeInt.Next;
                    }
                    break;
                case 5:
                    currentNode = madeGesturesWithinNucleusShortened.First;
                    nextNode = currentNode.Next;
                    currentNodeInt = madeGesturesWithinNucleusCounter.First;
                    int counter = 0;



                    //left , right, up, down
                    flags = new int[4];

                    gestureCountAfterCandidateGesture = 0;

                    candidateGestureFound = false;
                    while (nextNode != null)
                    {

                        if (candidateGestureFound)
                        {
                            //both are needed to calculate the number of times another gesture was done after doing the rectangle
                            gestureCountAfterCandidateGesture += currentNodeInt.Value;
                            counter++;
                        }

                        //my gesture
                        if (currentNode.Value.Equals("Rectangle"))
                            candidateGestureFound = true;


                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Left"))
                        {
                            flags[0]++;
                        }

                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Right"))
                        {
                            flags[1]++;
                        }
                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Up"))
                        {
                            flags[2]++;
                        }
                        if (candidateGestureFound && currentNode.Value.Equals("Swipe Down"))
                        {
                            flags[3]++;
                        }

                        int count = 0;
                        for (int i = 0; i < flags.Length; i++)
                        {
                            if (flags[i] > 2)
                                count++;
                            if (count > 2)
                                return false;
                        }



                        //go to next node
                        currentNode = currentNode.Next;
                        nextNode = nextNode.Next;
                        currentNodeInt = currentNodeInt.Next;
                    }
                    break;
            }

                    return true;
        }

        private bool OnlyOneSwipeIsDone(int c)
        {
            if(!(madeGesturesWithinNucleusShortened.Count <= 2))
                return false;
            if (madeGesturesWithinNucleus.Count < 2)
                return false;
            
            //to detect if I did another gesture after finishing my gesture 
            switch(c)
            {
                //I iterate over the done gestures and if a gesture after the candidate gesture was done more than 1 time it will counted as false
                // except for swiping down for retraction
                case 0:
                    LinkedListNode<String> currentNode = MadeGesturesWithinNucleus.First;
                    LinkedListNode<int> currentNodeInt = madeGesturesWithinNucleusCounter.First;

                    //two flags to check what is done before doing my gesture 
                    int upFlag = 0;
                    int downFlag = 0;
                    int leftFlag = 0;
                    int rightFlag = 0;

                    Boolean candidateGestureFound = false;
                    while (currentNode != null)
                    {
                        /*
                        //what happens after my gesture
                        if (candidateGestureFound)
                            if (currentNodeInt.Value > 1 && !currentNode.Value.Equals("Swipe Down"))
                                return false;
                        */
                        //my gesture
                        if (currentNode.Value.Equals("Swipe Left"))
                        {
                            candidateGestureFound = true;
                            leftFlag = 0;
                            rightFlag = 0;
                            upFlag = 0;
                            downFlag = 0;
                        }

                        // i want to check if two opposite gestures where done before or after getting my gesture 
                        if (currentNode.Value.Equals("Swipe Right"))
                        {
                            rightFlag++;
                            if (rightFlag > 2)
                                return false;
                        }

                        /*
                                                if (currentNode.Value.Equals("Swipe Up"))
                                                {
                                                    upFlag = true;
                                                    if (downFlag == true)
                                                        return false;
                                                }

                                                if (currentNode.Value.Equals("Swipe Down"))
                                                {
                                                    downFlag = true;
                                                    if (upFlag == true)
                                                        return false;
                                                }
                        */
                        //go to next node
                        Console.WriteLine("upflag: " + upFlag);
                        Console.WriteLine("downflag: " + downFlag);
                        Console.WriteLine("left flag: " + leftFlag);
                        Console.WriteLine("right flag: " + rightFlag);

                        currentNode = currentNode.Next;
                    }
                    break;
                case 1:
                    currentNode = MadeGesturesWithinNucleus.First;

                    //two flags to check what is done before doing my gesture 
                    upFlag = 0;
                    downFlag = 0;
                    leftFlag = 0;
                    rightFlag = 0;


                    candidateGestureFound = false;
                    while (currentNode != null)
                    {
                        /*
                        if (candidateGestureFound)
                            if (currentNodeInt.Value > 1 && !currentNode.Value.Equals("Swipe Down") && !currentNode.Value.Equals("Swipe Up"))
                                return false;
                        */

                        if (currentNode.Value.Equals("Swipe Right"))
                        {
                            candidateGestureFound = true;
                            leftFlag = 0;
                            rightFlag = 0;
                            upFlag = 0;
                            downFlag = 0;
                        }
                        
                        // i want to check if two opposite gestures where done before or after getting my gesture 


                        if (currentNode.Value.Equals("Swipe Left"))
                        {
                            leftFlag++; ;
                            if (leftFlag > 2)
                                return false;
                        }
                        /*
                        if (currentNode.Value.Equals("Swipe Up"))
                        {
                            upFlag = true;
                            if (downFlag == true)
                                return false;
                        }

                        if (currentNode.Value.Equals("Swipe Down"))
                        {
                            downFlag = true;
                            if (upFlag == true)
                                return false;
                        }
                        */

                        Console.WriteLine("upflag: " + upFlag);
                        Console.WriteLine("downflag: " + downFlag);
                        Console.WriteLine("left flag: " + leftFlag);
                        Console.WriteLine("right flag: " + rightFlag);

                        currentNode = currentNode.Next;
                    }
                    break;
                case 3:
                    currentNode = MadeGesturesWithinNucleus.First;

                    //two flags to check what is done before doing my gesture 
                    upFlag = 0;
                    downFlag = 0;
                    leftFlag = 0;
                    rightFlag = 0;


                    candidateGestureFound = false;
                    while (currentNode != null)
                    {
                        /*
                        if (candidateGestureFound)
                            if (currentNodeInt.Value > 1 && !currentNode.Value.Equals("Swipe Down"))
                                return false;
                        */
                        if (currentNode.Value.Equals("Swipe Up"))
                        {
                            candidateGestureFound = true;
                            leftFlag = 0;
                            rightFlag = 0;
                            upFlag = 0;
                            downFlag = 0;
                        }


                        // i want to check if two opposite gestures where done before or after getting my gesture 
                        if (currentNode.Value.Equals("Swipe Right"))
                        {
                            rightFlag++;
                            if (rightFlag > 1 && leftFlag > 1)
                                return false;
                        }

                        if (currentNode.Value.Equals("Swipe Left"))
                        {
                            leftFlag++;
                            if (leftFlag > 1 && rightFlag > 1)
                                return false;
                        }
                        /*
                        if (currentNode.Value.Equals("Swipe Up"))
                        {
                            upFlag = true;
                            if (downFlag == true)
                                return false;
                        }

                        if (currentNode.Value.Equals("Swipe Down"))
                        {
                            downFlag = true;
                            if (upFlag == true)
                                return false;
                        }
                        */

                        currentNode = currentNode.Next;
                    }
                    break;
                case 4:
                    currentNode = MadeGesturesWithinNucleus.First;

                    //two flags to check what is done before doing my gesture 
                    upFlag = 0;
                    downFlag = 0;
                    leftFlag = 0;
                    rightFlag = 0;

                    candidateGestureFound = false;
                    while (currentNode != null)
                    {
                        /*
                        if (candidateGestureFound && currentNodeInt.Value > 1)
                            return false;
                        */
                        if (currentNode.Value.Equals("Swipe Down"))
                        {
                            candidateGestureFound = true;
                            leftFlag = 0;
                            rightFlag = 0;
                            upFlag = 0;
                            downFlag = 0;
                        }

                        // i want to check if two opposite gestures where done before or after getting my gesture 
                        if (currentNode.Value.Equals("Swipe Right"))
                        {
                            rightFlag++;
                            if (rightFlag > 1 && leftFlag > 1)
                                return false;
                        }

                        if (currentNode.Value.Equals("Swipe Left"))
                        {
                            leftFlag++;
                            if (leftFlag > 1 && rightFlag > 1)
                                return false;
                        }
                        /*
                        if (currentNode.Value.Equals("Swipe Up"))
                        {
                            upFlag = true;
                            if (downFlag == true)
                                return false;
                        }

                        if (currentNode.Value.Equals("Swipe Down"))
                        {
                            downFlag = true;
                            if (upFlag == true)
                                return false;
                        }
                        */


                        currentNode = currentNode.Next;
                    }
                    break;

            }
            return true;


        }

        private bool HasAValidRatio(int c)
        {
            /*
            // second condition because slope down is done so fast
            if (numberOfFrames < 3 && c != 4)
                return false;
            */

            double threshold = 0.5;
            double ratio = ((double)gesturesCounter[c]/numberOfFrames);
            if(ratio < threshold)
                return false;

            //ratio for getting the gegenteil swipe 
            double swipeLeftOrRightThreshold = 0.25;
            double swipeDownThreshold = 0.2;
            double swipeUpThreshold = 0.4;
            switch(c)
            {
                case 0:
                    
                    double swipeRightRatio = ((double)gesturesCounter[1] / numberOfFrames);
                    if(swipeRightRatio > swipeLeftOrRightThreshold)
                        return false;
                    
                    break;
                case 1:
                    
                    double swipeLeftRatio = ((double)gesturesCounter[0] / numberOfFrames);
                    if (swipeLeftRatio > swipeLeftOrRightThreshold)
                        return false;
                    
                    break;
                case 3:
                    double swipeDownRatio = ((double)gesturesCounter[4] / numberOfFrames);
                    
                    if (swipeDownRatio > swipeUpThreshold || ratio <= 0.6)
                        return false;
                    break;
                case 4:
                    double swipeUpRatio = ((double)gesturesCounter[3] / numberOfFrames);
                    if (swipeUpRatio > swipeDownThreshold)
                        return false;
                    break;

            }
            return true;
        }

        public Boolean gestureNucleusIsReady(Hashtable pointsTable, RightHandMotionData rightHandMotionData)
        {
            Console.WriteLine("acceleration magnitude: " + rightHandMotionData.GetInstantAccelerationMagnitude());
            Console.WriteLine("differnece between hand right Z and spine mid Z");
            Console.WriteLine(((Point3D)pointsTable["HandRight"]).GetZ());
            Console.WriteLine(((Point3D)pointsTable["SpineMid"]).GetZ());

            if (((rightHandMotionData.AccelerationFlag && rightHandMotionData.GetInstantAccelerationMagnitude() > 5) || rightHandMotionData.ChangeInDirection()) && ((Point3D)pointsTable["HandRight"]).GetZ() + 0.01 < ((Point3D)pointsTable["SpineMid"]).GetZ() && leftHandState != Gesture.State.Nucleus)
                return true;
            return false;
        }

        public Boolean gestureNucleusIsReady(Hashtable pointsTable, LeftHandMotionData leftHandMotionData)
        {
            Console.WriteLine("acceleration magnitude: " + leftHandMotionData.GetInstantAccelerationMagnitude());
            if (((leftHandMotionData.AccelerationFlag && leftHandMotionData.GetInstantAccelerationMagnitude() > 5) || leftHandMotionData.ChangeInDirection()) && ((Point3D)pointsTable["HandLeft"]).GetZ() + 0.01 < ((Point3D)pointsTable["SpineMid"]).GetZ() && rightHandState != Gesture.State.Nucleus)
                return true;
            return false;
        }


        internal bool gestureNucleusEnded(Hashtable pointsTable, RightHandMotionData handMotionData)
        {
            Boolean case1 = handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames) < 3 && handMotionData.GetPoints().Count > 14 && ((Point3D)pointsTable["HandRight"]).GetZ() + 0.2 > ((Point3D)pointsTable["SpineMid"]).GetZ();
            Console.WriteLine("absolute average velocity last 20 frames: " + handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames));
            Console.WriteLine("right hand count last 10 frames: " + handMotionData.GetPoints().Count);
            Boolean case2 = handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames) < 3 && handMotionData.GetPoints().Count > 14;

            if (case1 || case2)
            {
                return true;
            }
            return false;
        }

        internal bool gestureNucleusEnded(Hashtable pointsTable, LeftHandMotionData handMotionData)
        {
            Boolean case1 = handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames) < 3 && handMotionData.GetPoints().Count > 14 && ((Point3D)pointsTable["HandRight"]).GetZ() + 0.2 > ((Point3D)pointsTable["SpineMid"]).GetZ();
            Console.WriteLine("absolute average velocity last 20 frames: " + handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames));
            Console.WriteLine("right hand count last 10 frames: " + handMotionData.GetPoints().Count);
            Boolean case2 = handMotionData.Get2DMagnitude(handMotionData.AbsoluteAverageVelocityLast20Frames) < 3 && handMotionData.GetPoints().Count > 14;

            if (case1 || case2)
            {
                return true;
            }
            return false;
        }

        public void AddToLastMadeGestures(string str)
        {
            lastMadeGestures.AddLast(str);
            if(lastMadeGestures.Count > 3)
                lastMadeGestures.RemoveFirst();
        }

        public void AddActiveGesture(int gestureNumber)
        {
            gesturesDictonery[gestureNumber] = true;
            gesturesCounter[gestureNumber]++;
            switch (gestureNumber)
            {
                case 0:
                    if (MadeGesturesWithinNucleus.Count > 0)
                        if (MadeGesturesWithinNucleus.Last.Value != "Swipe Left")
                            editMadeGesturesWithinNucleusShortened();
                    MadeGesturesWithinNucleus.AddLast("Swipe Left");
                    break;
                case 1:
                    if (MadeGesturesWithinNucleus.Count > 0)
                        if(MadeGesturesWithinNucleus.Last.Value != "Swipe Right")
                            editMadeGesturesWithinNucleusShortened();
                    MadeGesturesWithinNucleus.AddLast("Swipe Right");
                    break;
                case 2:
                    if (MadeGesturesWithinNucleus.Count > 0 )
                        if(MadeGesturesWithinNucleus.Last.Value != "Rotation")
                            editMadeGesturesWithinNucleusShortened();

                    //reset the circle to get if another circle was done in the future
                    LinkedListNode<Point3D> newCircleFirstPoint = circle.CirclePoint[circle.CirclePoint.Length-1];
                    circle = new CircleOrRectangle(newCircleFirstPoint);

                    MadeGesturesWithinNucleus.AddLast("Rotation");                
                    break;
                case 3:
                    if (MadeGesturesWithinNucleus.Count > 0)
                        if(MadeGesturesWithinNucleus.Last.Value != "Swipe Up")
                            editMadeGesturesWithinNucleusShortened();
                    MadeGesturesWithinNucleus.AddLast("Swipe Up");
                    break;
                case 4:
                    if (MadeGesturesWithinNucleus.Count > 0)
                        if(MadeGesturesWithinNucleus.Last.Value != "Swipe Down")
                            editMadeGesturesWithinNucleusShortened();
                    MadeGesturesWithinNucleus.AddLast("Swipe Down");
                    break;
                case 5:
                    if (MadeGesturesWithinNucleus.Count > 0)
                        if(MadeGesturesWithinNucleus.Last.Value != "Rectangle")
                            editMadeGesturesWithinNucleusShortened();

                    //reset the rectangle to get if another rectangle was done in the future
                    newCircleFirstPoint = circle.CirclePoint[circle.CirclePoint.Length - 1];
                    circle = new CircleOrRectangle(newCircleFirstPoint);


                    MadeGesturesWithinNucleus.AddLast("Rectangle");
                    break;
            }
            numberOfFrames++;
            gestureCounter++;
            if (lastMadeGestures.Count > 3)
                lastMadeGestures.RemoveFirst();

            //       nucleusGesture = true;

            gestureNumber++;
        }

        private void editMadeGesturesWithinNucleusShortened()
        {
            if(MadeGesturesWithinNucleus.Count >0)
            {
                madeGesturesWithinNucleusShortened.AddLast(MadeGesturesWithinNucleus.Last.Value);
                madeGesturesWithinNucleusCounter.AddLast(gestureCounter);
                gestureCounter = 0;
            }    
        }

        public void CheckGestureType(Hashtable pointsTable, RightHandMotionData handMotionData, string path)
        {
            circle.AddLastPoint(handMotionData.GetRightHand().Last);
            Boolean[] gesturesDictionery = this.GetGesturesDictonery();

            CheckForCircleOrRectangle(pointsTable ,path);
        }

        public void CheckGestureType(Hashtable pointsTable, LeftHandMotionData handMotionData, string path)
        {
            circle.AddLastPoint(handMotionData.GetPoints().Last);
            Boolean[] gesturesDictionery = this.GetGesturesDictonery();

            CheckForCircleOrRectangle(pointsTable, path);
        }

        private void updateGestureCounter()
        {
            for(int i = 0; i < gesturesDictonery.Length; i++)
            {
                if (gesturesDictonery[i] == true)
                {
                    gestureCounter++;
                    break;
                }
            }
        }

        private void CheckForCircleOrRectangle(Hashtable pointsTable, string path)
        {
            double shoulderReference = Math.Abs(((Point3D)pointsTable["ShoulderRight"]).GetX() - ((Point3D)pointsTable["ShoulderLeft"]).GetX());
            int isACircle = circle.IsThisCircleOrRectangle(shoulderReference, path);
            switch (isACircle)
            {
                case 1:
                    AddActiveGesture(2);
        //            circle = new CircleOrRectangle(circle.CirclePoint[17]);
                    break;
                case 2:
                    AddActiveGesture(5);
                    Console.WriteLine("rectangle detected: " + circle.CirclePoint[16].Value.ToString());
                    break;
                default:
                    break;
            }

        }

        private void checkForSwipeLeft(Point3D instantVelocity)
        {
            if (instantVelocity.GetX() < swipeLeftThreshold && instantVelocity.GetY() > - otherGesturesThreshold && instantVelocity.GetY() < otherGesturesThreshold)
            {
                AddActiveGesture(0);
            }


        }

        private void checkForSwipeUp(Point3D instantVelocity)
        {

            if (instantVelocity.GetY() < swipeUpThreshold && instantVelocity.GetX() > -otherGesturesThreshold && instantVelocity.GetX() < otherGesturesThreshold)
            {
                AddActiveGesture(3);
            }


        }
        private void checkForSwipeDown(Point3D instantVelocity)
        {
            if (instantVelocity.GetY() > swipeDownThreshold && instantVelocity.GetX() > -otherGesturesThreshold && instantVelocity.GetX() < otherGesturesThreshold)
            {
                AddActiveGesture(4);
            }


        }

        private void checkForSwipeRight(Point3D instantVelocity)
        {;
            // seperate parameters 
            if (instantVelocity.GetX() > swipeRightThreshold && instantVelocity.GetY() > -otherGesturesThreshold && instantVelocity.GetY() < otherGesturesThreshold+0.5)
                AddActiveGesture(1);

        }

        public static Boolean IsBodyMovingWithTheHand(HandMotionData handMotionData)
        {
            Console.WriteLine("class type: " + handMotionData.GetType());
            if(handMotionData.GetType() == typeof(RightHandMotionData))
            {
                double threshold = 5;
                Point3D rightHandAvg = ((RightHandMotionData) handMotionData).GetAverageVelocityLast20Frames();
                Point3D spineMidAvg = ((RightHandMotionData)handMotionData).GetAverageVelocityLast20FramesForSpineMid();
                double yDifference = Math.Abs(rightHandAvg.GetY() - spineMidAvg.GetY());

                if (yDifference < threshold && Math.Abs(spineMidAvg.GetX()) > 2)
                    return true;
                else
                    return false;
            }
            else
            {
                double threshold = 5;
                Point3D rightHandAvg = ((LeftHandMotionData)handMotionData).GetAverageVelocityLast20Frames();
                Point3D spineMidAvg = ((LeftHandMotionData)handMotionData).GetAverageVelocityLast20FramesForSpineMid();
                double yDifference = Math.Abs(rightHandAvg.GetY() - spineMidAvg.GetY());

                if (yDifference < threshold && Math.Abs(spineMidAvg.GetX()) > 2)
                    return true;
                else
                    return false;
            }
            
        }
    }
}
