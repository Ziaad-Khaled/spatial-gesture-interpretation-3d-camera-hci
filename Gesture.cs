using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class Gesture
    {
        private bool gesture;

        private int state = 0;
        //1 : preperation
        //2 : nucleus
        //3 : retraction

        private Boolean[] gesturesDictonery = { false, false, false };
        //0 : swipe left
        //1: swipe right
        //2 : rotate

        private int activeGesturesCount = 0;
        private LinkedList<int> activeGestures = new LinkedList<int>();
        
        private int swipeLeftCount = 0;
        private int swipeRightCount = 0;
        private int rotate = 0;


        Boolean rotationFlag = true;

        double rotationFirstPointX = -99;
        double rotationFirstPointY = -99;

        Boolean clockWiseRotation ;

        double circleRadius = 0;

        double standardRadius;

        int swipeLeftNumber;
        int swipeRightNumber;
        int rotateNumber;

        public double GetStandardRadius()
        {
            return standardRadius;
        }

        public void SetStandardRadius(double value)
        {
            standardRadius = value;
        }

        public bool GetClockWiseRotation()
        {
            return clockWiseRotation;
        }

        public void SetClockWiseRotation(bool value)
        {
            clockWiseRotation = value;
        }

        public double GetRotationFirstPointX()
        {
            return rotationFirstPointX;
        }

        public void SetRotationFirstPointX(double value)
        {
            rotationFirstPointX = value;
        }

        public double GetRotationFirstPointY()
        {
            return rotationFirstPointY;
        }

        public void SetRotationFirstPointY(double value)
        {
            rotationFirstPointY = value;

        }

        public bool GetRotationFlag()
        {
            return rotationFlag;
        }

        public void SetRotationFlag(bool value)
        {
            rotationFlag = value;
        }

        public int GetState()
        {
            return state;
        }

        public void SetState(int value)
        {
            state = value;
        }

        public bool GetGesture()
        {
            return gesture;
        }

        public void SetGesture(bool value)
        {
            gesture = value;
        }

        public void AddActiveGesture(int gestureNumber)
        {
            gesturesDictonery[gestureNumber] = true;
            gestureNumber++;
            activeGestures.AddLast(gestureNumber);
        }

        public int GetActiveGesturesCount()
        {
            return activeGesturesCount;
        }

        public LinkedList<int> GetActiveGestures()
        {
            return activeGestures;
        }

        public void KillActiveGesture(int gestureNumber)
        {
            gesturesDictonery[gestureNumber] = false;
            gestureNumber--;
            activeGestures.Remove(gestureNumber);
        }

        public Boolean[] GetGesturesDictonery()
        {
            return gesturesDictonery;
        }

        public void resetCircle()
        {
            this.rotationFlag = true;
            this.rotationFirstPointX = -99;
            this.rotationFirstPointY = -99;
            this.circleRadius = 0;

        }
    }
}
