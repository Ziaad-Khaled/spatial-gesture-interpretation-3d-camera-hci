using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class LeftHandMotionData : HandMotionData
    {
        private LinkedList<Point3D> leftHand = new LinkedList<Point3D>();

        private LinkedList<Point3D> spineMid = new LinkedList<Point3D>();

        private LinkedList<Point3D> leftHandLast10Frames = new LinkedList<Point3D>();

        private LinkedList<Point3D> leftHandLast20Frames = new LinkedList<Point3D>();

        private LinkedList<VelocityPoint> leftHandVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> spineMidVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<AccelerationPoint> leftHandAcceleration = new LinkedList<AccelerationPoint>();

        private LinkedList<VelocityPoint> leftHandLast2FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> leftHandLast10FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> leftHandLast20FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> leftHandLast30FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> spineMidLast20FramesVelocity = new LinkedList<VelocityPoint>();


        private LinkedList<VelocityLine> velocityLines = new LinkedList<VelocityLine>();

        private Point3D sumLeftHand = new Point3D(0, 0, 0);

        private Point3D averageLeftHand = new Point3D(0, 0, 0);

        private Point3D sumLeftHandLast20Frames = new Point3D(0, 0, 0);

        private Point3D sumLeftHandLast10Frames = new Point3D(0, 0, 0);

        private Point3D averageLeftHandLast20Frames = new Point3D(0, 0, 0);

        private Point3D averageLeftHandLast10Frames = new Point3D(0, 0, 0);



        private Point3D averageVelocity = new Point3D(0, 0, 0);

        private Point3D averageVelocitySpineMid = new Point3D(0, 0, 0);

        private Point3D sumVelocity = new Point3D(0, 0, 0);

        private Point3D sumVelocitySpineMid = new Point3D(0, 0, 0);


        private double instantVelocityMagnitude;

        private Point3D sumVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast10Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast20FramesForSpineMid = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast10Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D absoluteAverageVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D absoluteAverageVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D absoluteAverageVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast20FramesForSpineMid = new Point3D(0, 0, 0);

        private Point3D instantVelocity = new Point3D(0, 0, 0);

        private Point3D averageAccelration = new Point3D(0, 0, 0);

        private Point3D sumAcceleration = new Point3D(0, 0, 0);

        private List<Hashtable> motionData = new List<Hashtable>();


        Boolean firstFrameAdded = false;

        Boolean secondFrameAdded = false;

        private Boolean accelerationFlag;

        public bool AccelerationFlag { get => accelerationFlag; set => accelerationFlag = value; }
        internal Point3D AverageRightHandLast20Frames { get => averageLeftHandLast20Frames; set => averageLeftHandLast20Frames = value; }
        internal Point3D AverageRightHandLast10Frames { get => averageLeftHandLast10Frames; set => averageLeftHandLast10Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast2Frames { get => absoluteAverageVelocityLast2Frames; set => absoluteAverageVelocityLast2Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast20Frames { get => absoluteAverageVelocityLast20Frames; set => absoluteAverageVelocityLast20Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast30Frames { get => absoluteAverageVelocityLast30Frames; set => absoluteAverageVelocityLast30Frames = value; }

        internal LinkedList<Point3D> GetPoints1()
        {
            return leftHand;
        }

        internal void SetPoints1(LinkedList<Point3D> value)
        {
            leftHand = value;
        }

        internal Point3D GetAverageLeftHand()
        {
            return averageLeftHand;
        }

        internal void SetAverageRightHand(Point3D value)
        {
            averageLeftHand = value;
        }

        public List<Hashtable> GetMotionData()
        {
            return motionData;
        }

        public void SetMotionData(List<Hashtable> value)
        {
            motionData = value;
        }

        public Point3D GetAverageVelocityLast20FramesForSpineMid()
        {
            return averageVelocityLast20FramesForSpineMid;
        }

        public void SetAverageVelocityLast20FramesForSpineMid(Point3D value)
        {
            averageVelocityLast20FramesForSpineMid = value;
        }

        public Point3D GetSumVelocityLast20FramesForSpineMid()
        {
            return sumVelocityLast20FramesForSpineMid;
        }

        public void SetSumVelocityLast20FramesForSpineMid(Point3D value)
        {
            sumVelocityLast20FramesForSpineMid = value;
        }

        public Point3D GetAverageVelocityLast20Frames()
        {
            return averageVelocityLast20Frames;
        }

        public void SetAverageVelocityLast20Frames(Point3D value)
        {
            averageVelocityLast20Frames = value;
        }

        public bool GetAccelerationFlag()
        {
            return accelerationFlag;
        }

        public void SetAcceleration(bool value)
        {
            accelerationFlag = value;
        }

        public Point3D GetAverageAccelration()
        {
            return averageAccelration;
        }

        public void SetAverageAccelration(Point3D value)
        {
            averageAccelration = value;
        }

        public double GetInstantAccelerationMagnitude()
        {
            double magnitude;
            try
            {
                double xSquared = leftHandAcceleration.Last().GetXAcceleration() * leftHandAcceleration.Last().GetXAcceleration();
                double ySquared = leftHandAcceleration.Last().GetYAcceleration() * leftHandAcceleration.Last().GetYAcceleration();
                double zSquared = leftHandAcceleration.Last().GetZAcceleration() * leftHandAcceleration.Last().GetZAcceleration();
                magnitude = Math.Sqrt(xSquared + ySquared + zSquared);
                if (magnitude < 1)
                    accelerationFlag = true;
            }
            catch (Exception)
            {
                return 0;
            }

            return magnitude;
        }

        public Boolean endPreperation()
        {
            Boolean end = false;
            try
            {
                if (accelerationFlag == true && GetInstantAccelerationMagnitude() > 2)
                    end = true;
            }
            catch (Exception e)
            {
                // recover from exception
            }

            return end;
        }

        public LinkedList<VelocityLine> GetVelocityLines()
        {
            return velocityLines;
        }

        public void SetVelocityLines(LinkedList<VelocityLine> value)
        {
            velocityLines = value;
        }

        public double GetInstantVelocityMagnitude()
        {
            return instantVelocityMagnitude;
        }

        public void SetInstantVelocityMagnitude(double value)
        {
            instantVelocityMagnitude = value;
        }

        public LinkedList<Point3D> GetPoints()
        {
            return GetPoints1();
        }

        public void SetRightHand(LinkedList<Point3D> value)
        {
            SetPoints1(value);
        }

        public LinkedList<Point3D> GetRightHandLast10Frames()
        {
            return leftHandLast10Frames;
        }

        public void SetRightHandLast10Frames(LinkedList<Point3D> value)
        {
            leftHandLast10Frames = value;
        }

        public LinkedList<VelocityPoint> GetRightHandVelocity()
        {
            return leftHandVelocity;
        }

        public void SetRightHandVelocity(LinkedList<VelocityPoint> value)
        {
            leftHandVelocity = value;
        }

        public LinkedList<VelocityPoint> GetRightHandLast10FramesVelocity()
        {
            return leftHandLast10FramesVelocity;
        }

        public void SetRightHandLast10FramesVelocity(LinkedList<VelocityPoint> value)
        {
            leftHandLast10FramesVelocity = value;
        }

        public Point3D GetAverageVelocity()
        {
            return averageVelocity;
        }

        public void SetAverageVelocity(Point3D value)
        {
            averageVelocity = value;
        }

        public Point3D GetSumVelocity()
        {
            return sumVelocity;
        }

        public void SetSumVelocity(Point3D value)
        {
            sumVelocity = value;
        }

        public Point3D GetAverageVelocityLast10Frames()
        {
            return averageVelocityLast10Frames;
        }

        public void SetAverageVelocityLast10Frames(Point3D value)
        {
            averageVelocityLast10Frames = value;
        }

        public Point3D GetSumVelocityLast10Frames()
        {
            return sumVelocityLast10Frames;
        }

        public void SetSumVelocityLast10Frames(Point3D value)
        {
            sumVelocityLast10Frames = value;
        }

        public Point3D GetInstantVelocity()
        {
            return instantVelocity;
        }

        public void SetInstantVelocity(VelocityPoint point)
        {
            instantVelocity.SetNewPoints(point.GetXVelocity(), point.GetYVelocity(), point.GetZVelocity());
        }

        public void AddFrame(Hashtable hashtable)
        {
            GetMotionData().Add(hashtable);

            Point3D lastFrame = ((Point3D)hashtable["HandLeft"]);
            Point3D lastFrameInLast10Frames = ((Point3D)hashtable["HandLeft"]);
            Point3D lastFrameSpineMid = (Point3D)hashtable["SpineMid"];

            if (firstFrameAdded)
            {
                lastFrame = this.GetPoints1().Last();

                lastFrameSpineMid = this.spineMid.Last();

                lastFrameInLast10Frames = this.leftHandLast10Frames.Last();
            }

            this.GetPoints1().AddLast(((Point3D)hashtable["HandLeft"]));

            this.spineMid.AddLast(((Point3D)hashtable["SpineMid"]));

            leftHandLast10Frames.AddLast((Point3D)hashtable["HandLeft"]);

            leftHandLast20Frames.AddLast((Point3D)hashtable["HandLeft"]);

            sumLeftHand.SetNewPoints(sumLeftHand.GetX() + ((Point3D)hashtable["HandLeft"]).GetX(), sumLeftHand.GetY() + ((Point3D)hashtable["HandLeft"]).GetY(), sumLeftHand.GetZ() + ((Point3D)hashtable["HandLeft"]).GetZ());
            averageLeftHand.SetNewPoints(sumLeftHand.GetX() / GetPoints1().Count, sumLeftHand.GetY() / GetPoints1().Count, sumLeftHand.GetZ() / GetPoints1().Count);


            sumLeftHandLast20Frames.SetNewPoints(sumLeftHandLast20Frames.GetX() + ((Point3D)hashtable["HandLeft"]).GetX(), sumLeftHandLast20Frames.GetY() + ((Point3D)hashtable["HandLeft"]).GetY(), sumLeftHandLast20Frames.GetZ() + ((Point3D)hashtable["HandLeft"]).GetZ());

            if (leftHandLast20FramesVelocity.Count > 20)
            {
                sumLeftHandLast20Frames.SetNewPoints(sumLeftHandLast20Frames.GetX() - leftHandLast20Frames.First.Value.GetX(), sumLeftHandLast20Frames.GetY() - leftHandLast20Frames.First.Value.GetY(), sumLeftHandLast20Frames.GetZ() - leftHandLast20Frames.First.Value.GetZ());
                leftHandLast20Frames.RemoveFirst();
            }

            averageLeftHandLast20Frames.SetNewPoints(sumLeftHandLast20Frames.GetX() / leftHandLast20Frames.Count, sumLeftHandLast20Frames.GetY() / leftHandLast20Frames.Count, sumLeftHandLast20Frames.GetZ() / leftHandLast20Frames.Count);

            sumLeftHandLast10Frames.SetNewPoints(sumLeftHandLast10Frames.GetX() + ((Point3D)hashtable["HandLeft"]).GetX(), sumLeftHandLast10Frames.GetY() + ((Point3D)hashtable["HandLeft"]).GetY(), sumLeftHandLast10Frames.GetZ() + ((Point3D)hashtable["HandLeft"]).GetZ());
            if (leftHandLast10Frames.Count > 10)
            {
                sumLeftHandLast10Frames.SetNewPoints(sumLeftHandLast10Frames.GetX() - leftHandLast10Frames.First.Value.GetX(), sumLeftHandLast10Frames.GetY() - leftHandLast10Frames.First.Value.GetY(), sumLeftHandLast10Frames.GetZ() - leftHandLast10Frames.First.Value.GetZ());
                leftHandLast10Frames.RemoveFirst();
            }
            averageLeftHandLast10Frames.SetNewPoints(sumLeftHandLast10Frames.GetX() / leftHandLast10Frames.Count, sumLeftHandLast10Frames.GetY() / leftHandLast10Frames.Count, sumLeftHandLast10Frames.GetZ() / leftHandLast10Frames.Count);


            if (firstFrameAdded)
            {
                this.UpdateVelocities(hashtable, lastFrame, lastFrameSpineMid);
            }
            if (!firstFrameAdded)
                firstFrameAdded = true;
        }

        private void UpdateVelocities(Hashtable hashtable, Point3D lastFrame, Point3D lastFrameSpineMid)
        {
            double handPositionX = ((Point3D)hashtable["HandLeft"]).GetX();
            double handPositionY = ((Point3D)hashtable["HandLeft"]).GetY();
            double handPositionZ = ((Point3D)hashtable["HandLeft"]).GetZ();

            Point3D spineMid = ((Point3D)hashtable["SpineMid"]);
            VelocityPoint velocityNewPoint = new VelocityPoint(handPositionX - lastFrame.GetX(), handPositionY - lastFrame.GetY(), handPositionZ - lastFrame.GetZ(), handPositionX, handPositionY);
            VelocityPoint spineMidVelocityNewPoint = new VelocityPoint(spineMid.GetX() - lastFrameSpineMid.GetX(), spineMid.GetY() - lastFrameSpineMid.GetY(), spineMid.GetZ() - lastFrameSpineMid.GetZ(), spineMid.GetX(), spineMid.GetY());
            VelocityPoint point1SpineMid = null;
            VelocityPoint point1 = null;
            VelocityPoint spineMidBeforeLastVelocity = null;
            VelocityPoint beforeLastVelocity = null;
            if (secondFrameAdded)
            {
                beforeLastVelocity = leftHandVelocity.Last();
                spineMidBeforeLastVelocity = spineMidVelocity.Last();
                point1 = leftHandVelocity.Last();
                point1SpineMid = spineMidVelocity.Last();
            }


            leftHandVelocity.AddLast(velocityNewPoint);

            spineMidVelocity.AddLast(spineMidVelocityNewPoint);

            sumVelocity.SetNewPoints(sumVelocity.GetX() + handPositionX - lastFrame.GetX(), sumVelocity.GetY() + handPositionY - lastFrame.GetY(), sumVelocity.GetZ() + handPositionZ - lastFrame.GetZ());
            averageVelocity.SetNewPoints(sumVelocity.GetX() / leftHandVelocity.Count, sumVelocity.GetY() / leftHandVelocity.Count, sumVelocity.GetZ() / leftHandVelocity.Count);

            sumVelocitySpineMid.SetNewPoints(sumVelocitySpineMid.GetX() + spineMid.GetX() - lastFrameSpineMid.GetX(), sumVelocitySpineMid.GetY() + spineMid.GetY() - lastFrameSpineMid.GetY(), sumVelocitySpineMid.GetZ() + spineMid.GetZ() - lastFrameSpineMid.GetZ());
            averageVelocitySpineMid.SetNewPoints(sumVelocitySpineMid.GetX() / spineMidVelocity.Count, sumVelocitySpineMid.GetY() / spineMidVelocity.Count, sumVelocitySpineMid.GetZ() / spineMidVelocity.Count);


            spineMidLast20FramesVelocity.AddLast(spineMidVelocityNewPoint);
            sumVelocityLast20FramesForSpineMid.SetNewPoints(sumVelocityLast20FramesForSpineMid.GetX() + spineMid.GetX() - lastFrameSpineMid.GetX(), sumVelocityLast20FramesForSpineMid.GetY() + spineMid.GetY() - lastFrameSpineMid.GetY(), sumVelocityLast20FramesForSpineMid.GetZ() + spineMid.GetZ() - lastFrameSpineMid.GetZ());

            if (spineMidLast20FramesVelocity.Count > 20)
            {
                sumVelocityLast20FramesForSpineMid.SetNewPoints(sumVelocityLast20FramesForSpineMid.GetX() - spineMidLast20FramesVelocity.First().GetXVelocity(), sumVelocityLast20FramesForSpineMid.GetY() - spineMidLast20FramesVelocity.First().GetYVelocity(), sumVelocityLast20FramesForSpineMid.GetZ() - spineMidLast20FramesVelocity.First().GetZVelocity());
                spineMidLast20FramesVelocity.RemoveFirst();
            }
            averageVelocityLast20FramesForSpineMid.SetNewPoints(sumVelocityLast20FramesForSpineMid.GetX() / spineMidLast20FramesVelocity.Count, sumVelocityLast20FramesForSpineMid.GetY() / spineMidLast20FramesVelocity.Count, sumVelocityLast20FramesForSpineMid.GetZ() / spineMidLast20FramesVelocity.Count);


            leftHandLast20FramesVelocity.AddLast(velocityNewPoint);
            sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() + handPositionX - lastFrame.GetX(), sumVelocityLast20Frames.GetY() + handPositionY - lastFrame.GetY(), sumVelocityLast20Frames.GetZ() + handPositionZ - lastFrame.GetZ());
            absoluteSumVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() + Math.Abs(handPositionX - lastFrame.GetX()), absoluteSumVelocityLast20Frames.GetY() + Math.Abs(handPositionY - lastFrame.GetY()), absoluteSumVelocityLast20Frames.GetZ() + Math.Abs(handPositionZ - lastFrame.GetZ()));

            if (leftHandLast20FramesVelocity.Count > 25)
            {
                sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() - leftHandLast20FramesVelocity.First().GetXVelocity(), sumVelocityLast20Frames.GetY() - leftHandLast20FramesVelocity.First().GetYVelocity(), sumVelocityLast20Frames.GetZ() - leftHandLast20FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() - Math.Abs(leftHandLast20FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast20Frames.GetY() - Math.Abs(leftHandLast20FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast20Frames.GetZ() - Math.Abs(leftHandLast20FramesVelocity.First().GetZVelocity()));
                leftHandLast20FramesVelocity.RemoveFirst();
            }
            averageVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() / leftHandLast20FramesVelocity.Count, sumVelocityLast20Frames.GetY() / leftHandLast20FramesVelocity.Count, sumVelocityLast20Frames.GetZ() / leftHandLast20FramesVelocity.Count);
            absoluteAverageVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() / leftHandLast20FramesVelocity.Count, absoluteSumVelocityLast20Frames.GetY() / leftHandLast20FramesVelocity.Count, absoluteSumVelocityLast20Frames.GetZ() / leftHandLast20FramesVelocity.Count);



            leftHandLast30FramesVelocity.AddLast(velocityNewPoint);
            sumVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() + handPositionX - lastFrame.GetX(), sumVelocityLast30Frames.GetY() + handPositionY - lastFrame.GetY(), sumVelocityLast30Frames.GetZ() + handPositionZ - lastFrame.GetZ());
            absoluteSumVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() + Math.Abs(handPositionX - lastFrame.GetX()), absoluteSumVelocityLast30Frames.GetY() + Math.Abs(handPositionY - lastFrame.GetY()), absoluteSumVelocityLast30Frames.GetZ() + Math.Abs(handPositionZ - lastFrame.GetZ()));

            if (leftHandLast30FramesVelocity.Count > 35)
            {
                sumVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() - leftHandLast30FramesVelocity.First().GetXVelocity(), sumVelocityLast30Frames.GetY() - leftHandLast30FramesVelocity.First().GetYVelocity(), sumVelocityLast30Frames.GetZ() - leftHandLast30FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() - Math.Abs(leftHandLast30FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast30Frames.GetY() - Math.Abs(leftHandLast30FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast30Frames.GetZ() - Math.Abs(leftHandLast30FramesVelocity.First().GetZVelocity()));
                leftHandLast30FramesVelocity.RemoveFirst();
            }
            averageVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() / leftHandLast30FramesVelocity.Count, sumVelocityLast30Frames.GetY() / leftHandLast30FramesVelocity.Count, sumVelocityLast30Frames.GetZ() / leftHandLast30FramesVelocity.Count);
            AbsoluteAverageVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() / leftHandLast30FramesVelocity.Count, absoluteSumVelocityLast30Frames.GetY() / leftHandLast30FramesVelocity.Count, absoluteSumVelocityLast30Frames.GetZ() / leftHandLast30FramesVelocity.Count);



            /*
            double ratio = (rightHandXVelocity.Count - 1) / rightHandXVelocity.Count;

            averageVelocityX = averageVelocityX*ratio + rightHandXVelocity.Last()/ rightHandXVelocity.Count;
            averageVelocityY = averageVelocityY*ratio + rightHandYVelocity.Last()/ rightHandYVelocity.Count;
            averageVelocityZ = averageVelocityZ*ratio + rightHandZVelocity.Last()/ rightHandZVelocity.Count;
            */

            leftHandLast2FramesVelocity.AddLast(velocityNewPoint);
            sumVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() + handPositionX - lastFrame.GetX(), sumVelocityLast2Frames.GetY() + handPositionY - lastFrame.GetY(), sumVelocityLast2Frames.GetZ() + handPositionZ - lastFrame.GetZ());
            absoluteSumVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() + Math.Abs(handPositionX - lastFrame.GetX()), absoluteSumVelocityLast2Frames.GetY() + Math.Abs(handPositionY - lastFrame.GetY()), absoluteSumVelocityLast2Frames.GetZ() + Math.Abs(handPositionZ - lastFrame.GetZ()));


            if (leftHandLast2FramesVelocity.Count > 10)
            {
                sumVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() - leftHandLast2FramesVelocity.First().GetXVelocity(), sumVelocityLast2Frames.GetY() - leftHandLast2FramesVelocity.First().GetYVelocity(), sumVelocityLast2Frames.GetZ() - leftHandLast2FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() - Math.Abs(leftHandLast2FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast2Frames.GetY() - Math.Abs(leftHandLast2FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast2Frames.GetZ() - Math.Abs(leftHandLast2FramesVelocity.First().GetZVelocity()));

                leftHandLast2FramesVelocity.RemoveFirst();
            }
            averageVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() / leftHandLast2FramesVelocity.Count, sumVelocityLast2Frames.GetY() / leftHandLast2FramesVelocity.Count, sumVelocityLast2Frames.GetZ() / leftHandLast2FramesVelocity.Count);
            absoluteAverageVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() / leftHandLast2FramesVelocity.Count, absoluteSumVelocityLast2Frames.GetY() / leftHandLast2FramesVelocity.Count, absoluteSumVelocityLast2Frames.GetZ() / leftHandLast2FramesVelocity.Count);

            Console.WriteLine("absolute sum is: " + absoluteSumVelocityLast2Frames.ToString());
            Console.WriteLine("absolute average velocity is: " + absoluteAverageVelocityLast2Frames.ToString());



            velocityNewPoint = new VelocityPoint(handPositionX - lastFrame.GetX(), handPositionY - lastFrame.GetY(), handPositionZ - lastFrame.GetZ(), handPositionX, handPositionY);
            leftHandLast10FramesVelocity.AddLast(velocityNewPoint);

            sumVelocityLast10Frames.SetNewPoints(sumVelocityLast10Frames.GetX() + handPositionX - lastFrame.GetX(), sumVelocityLast10Frames.GetY() + handPositionY - lastFrame.GetY(), sumVelocityLast10Frames.GetZ() + handPositionZ - lastFrame.GetZ());


            if (leftHandLast10FramesVelocity.Count > 10)
            {
                sumVelocityLast10Frames.SetX(sumVelocityLast10Frames.GetX() - leftHandLast10FramesVelocity.First().GetXVelocity());
                sumVelocityLast10Frames.SetY(sumVelocityLast10Frames.GetY() - leftHandLast10FramesVelocity.First().GetYVelocity());
                sumVelocityLast10Frames.SetZ(sumVelocityLast10Frames.GetZ() - leftHandLast10FramesVelocity.First().GetZVelocity());

                leftHandLast10FramesVelocity.RemoveFirst();

            }

            averageVelocityLast10Frames.SetX(sumVelocityLast10Frames.GetX() / leftHandLast10FramesVelocity.Count);
            averageVelocityLast10Frames.SetY(sumVelocityLast10Frames.GetY() / leftHandLast10FramesVelocity.Count);
            averageVelocityLast10Frames.SetZ(sumVelocityLast10Frames.GetZ() / leftHandLast10FramesVelocity.Count);



            if (secondFrameAdded)
            {
                VelocityLine newVelocityLine = new VelocityLine(point1, velocityNewPoint);
                velocityLines.AddLast(newVelocityLine);
                this.updateAcceleration(beforeLastVelocity, velocityNewPoint);
            }
            else
            {
                secondFrameAdded = true;
            }

            if (velocityLines.Count > 10)
            {
                velocityLines.RemoveFirst();
            }
            /*
            double last10FramesRatio = (rightHandXLast10Frames.Count - 1) / rightHandXLast10Frames.Count;

            averageVelocityXLast10Frames = averageVelocityXLast10Frames*last10FramesRatio + rightHandXLast10FramesVelocity.Last()/ rightHandXLast10FramesVelocity.Count;
            averageVelocityYLast10Frames = averageVelocityYLast10Frames * last10FramesRatio + rightHandYLast10FramesVelocity.Last() / rightHandYLast10FramesVelocity.Count;
            averageVelocityZLast10Frames = averageVelocityZLast10Frames * last10FramesRatio + rightHandZLast10FramesVelocity.Last() / rightHandZLast10FramesVelocity.Count;
            */
            /*
            double ratio = (rightHandX.Count - 1) / rightHandX.Count;
            SetAverageVelocityX((GetAverageVelocityX() * ratio) + (handPositionX - lastFrameX) / rightHandX.Count);
            SetAverageVelocityY((GetAverageVelocityY() * ratio) + (handPositionY - lastFrameY) / rightHandY.Count);
            SetAverageVelocityZ((GetAverageVelocityZ() * ratio) + (handPositionZ - lastFrameZ) / rightHandZ.Count);

            double last10FramesRatio = (rightHandXLast10Frames.Count - 1) / rightHandXLast10Frames.Count;
            SetAverageVelocityXLast10Frames((GetAverageVelocityXLast10Frames() * last10FramesRatio) + (handPositionX - lastFrameX) / rightHandXLast10Frames.Count);
            SetAverageVelocityYLast10Frames((GetAverageVelocityYLast10Frames() * last10FramesRatio) + (handPositionY - lastFrameY) / rightHandYLast10Frames.Count);
            SetAverageVelocityZLast10Frames((GetAverageVelocityZLast10Frames() * last10FramesRatio) + (handPositionZ - lastFrameZ) / rightHandZLast10Frames.Count);*/

            ComputeInstantVelocityMagnitude();
            SetInstantVelocity(velocityNewPoint);
        }

        public void updateAcceleration(VelocityPoint beforeLastVelocity, VelocityPoint lastVelocity)
        {
            AccelerationPoint newPoint = new AccelerationPoint(lastVelocity.GetXVelocity() - beforeLastVelocity.GetXVelocity(), lastVelocity.GetYVelocity() - beforeLastVelocity.GetYVelocity(), lastVelocity.GetZVelocity() - beforeLastVelocity.GetZVelocity(), lastVelocity.GetPosition());
            sumAcceleration.SetNewPoints(sumAcceleration.GetX() + newPoint.GetXAcceleration(), sumAcceleration.GetY() + newPoint.GetYAcceleration(), sumAcceleration.GetZ() + newPoint.GetZAcceleration());

            leftHandAcceleration.AddLast(newPoint);

            GetAverageAccelration().SetX(sumAcceleration.GetX() / leftHandAcceleration.Count);
            GetAverageAccelration().SetY(GetAverageAccelration().GetY() / leftHandAcceleration.Count);
            GetAverageAccelration().SetZ(GetAverageAccelration().GetZ() / leftHandAcceleration.Count);
        }



        public void ComputeInstantVelocityMagnitude()
        {
            double total = instantVelocity.GetX() * instantVelocity.GetX() + instantVelocity.GetY() * instantVelocity.GetY();
            instantVelocityMagnitude = Math.Sqrt(total);
        }

        public Boolean inActiveFor30Frames()
        {
            // Console.WriteLine("average velocity Magnitude: " + GetAcerageVelocityMagnitudeForTheLast20Frames());
            //  Console.WriteLine("Count: " + rightHandLast20FramesVelocity.Count);
            if (Get2DMagnitude(AbsoluteAverageVelocityLast30Frames) < 2 && leftHandLast30FramesVelocity.Count > 24)
            {
                return true;
            }
            return false;
        }

        public double GetAverageVelocityMagnitudeForTheLast10Frames()
        {
            double xSquared = averageVelocityLast2Frames.GetX() * averageVelocityLast2Frames.GetX();
            double ySquared = averageVelocityLast2Frames.GetY() * averageVelocityLast2Frames.GetY();
            double zSquared = averageVelocityLast2Frames.GetZ() * averageVelocityLast2Frames.GetZ();
            double magnitude = Math.Sqrt(xSquared + ySquared + zSquared);
            return magnitude;
        }

        public double GetAverageVelocityMagnitudeForTheLast20Frames()
        {
            double xSquared = averageVelocityLast20Frames.GetX() * averageVelocityLast20Frames.GetX();
            double ySquared = averageVelocityLast20Frames.GetY() * averageVelocityLast20Frames.GetY();
            double zSquared = averageVelocityLast20Frames.GetZ() * averageVelocityLast20Frames.GetZ();
            double magnitude = Math.Sqrt(xSquared + ySquared + zSquared);
            return magnitude;
        }

        public double Get2DMagnitude(Point3D point)
        {
            double xSquared = point.GetX() * point.GetX();
            double ySquared = point.GetY() * point.GetY();
            double magnitude = Math.Sqrt(xSquared + ySquared);
            return magnitude;
        }
        public double Get3DMagnitude(Point3D point)
        {
            double xSquared = point.GetX() * point.GetX();
            double ySquared = point.GetY() * point.GetY();
            double zSquared = point.GetZ() * point.GetZ();
            double magnitude = Math.Sqrt(xSquared + ySquared + zSquared);
            return magnitude;
        }

        public void FlushData()
        {

            GetPoints1().Clear();
            leftHandLast10Frames.Clear();
            leftHandVelocity.Clear();
            leftHandLast2FramesVelocity.Clear();
            leftHandLast10FramesVelocity.Clear();
            leftHandLast20FramesVelocity.Clear();
            leftHandLast30FramesVelocity.Clear();
            velocityLines.Clear();
            averageVelocity.Reset();
            sumVelocity.Reset();
            averageVelocityLast2Frames.Reset();
            averageVelocityLast10Frames.Reset();
            averageVelocityLast20Frames.Reset();
            averageVelocityLast30Frames.Reset();
            instantVelocityMagnitude = 0;
            sumVelocityLast2Frames.Reset();
            sumVelocityLast10Frames.Reset();
            sumVelocityLast20Frames.Reset();
            sumVelocityLast30Frames.Reset();

            instantVelocity.Reset();
            accelerationFlag = false;
            firstFrameAdded = false;
            secondFrameAdded = false;
            motionData = new List<Hashtable>();
            sumLeftHand.Reset();
            averageLeftHand.Reset();

            sumLeftHandLast10Frames.Reset();
            sumLeftHandLast20Frames.Reset();
            averageLeftHandLast20Frames.Reset();
            averageLeftHandLast10Frames.Reset();

            absoluteAverageVelocityLast2Frames.Reset();
            absoluteAverageVelocityLast20Frames.Reset();
            AbsoluteAverageVelocityLast30Frames.Reset();

            absoluteSumVelocityLast20Frames.Reset();
            absoluteSumVelocityLast2Frames.Reset();
            absoluteSumVelocityLast30Frames.Reset();
        }

        public Boolean ChangeInDirection()
        {
            //       double averageVelocityMagnitude = Math.Sqrt(averageVelocity.GetX() * averageVelocity.GetX() + averageVelocity.GetY() * averageVelocity.GetY());
            //       double instantVelocityMagnitude = Math.Sqrt(instantVelocity.GetX() * instantVelocity.GetX() + instantVelocity.GetY() * instantVelocity.GetY());
            double threshold = 16;
            Point3D averageVelocityUnitVector = new Point3D(averageVelocity.GetX(), averageVelocity.GetY(), 0);
            Point3D instantVelocityUnitVector = new Point3D(instantVelocity.GetX(), instantVelocity.GetY(), 0);

            double averageVelocityMagnitude = Get2DMagnitude(averageVelocity);

            Console.WriteLine("average velocity unit vector: " + averageVelocity.ToString());
            Console.WriteLine("instant velocity unit vector: " + instantVelocityUnitVector.ToString());

            Console.WriteLine("averageVelocityMagnitude:  " + averageVelocityMagnitude);

            double xDifference = Math.Abs(averageVelocityUnitVector.GetX() - instantVelocityUnitVector.GetX());
            double yDifference = Math.Abs(averageVelocityUnitVector.GetY() - instantVelocityUnitVector.GetY());

            Console.WriteLine("X Difference: " + xDifference);
            Console.WriteLine("not the same sign: " + !((averageVelocity.GetX() >= 0) ^ (instantVelocity.GetX() < 0)));
            Console.WriteLine("y Difference: " + yDifference);
            Console.WriteLine("not the same sign: " + !((averageVelocity.GetY() >= 0) ^ (instantVelocity.GetY() < 0)));

            Boolean zeroVelocity = false;

            if (Math.Abs(instantVelocity.GetX()) < averageVelocityMagnitude/1.5 && Math.Abs(instantVelocity.GetY()) < averageVelocityMagnitude / 1.5 && instantVelocity.GetX() != 0 && instantVelocity.GetY() != 0)
                zeroVelocity = true;


            if ((xDifference > threshold && !((averageVelocity.GetX() >= 0) ^ (instantVelocity.GetX() < 0))) || (yDifference > threshold && !((averageVelocity.GetY() >= 0) ^ (instantVelocity.GetY() < 0))) || zeroVelocity)
            {
                Console.WriteLine("trueeeeeeeeeee");
                return true;
            }
            return false;
        }

        public static Point3D UnitVector(Point3D point)
        {
            double xSquared = point.GetX() * point.GetX();
            double ySquared = point.GetY() * point.GetY();
            double magnitude = Math.Sqrt(xSquared + ySquared);
            Point3D newPoint = new Point3D(point.GetX() / magnitude, point.GetY() / magnitude, 0 / magnitude);



            return newPoint;
        }
    }


}
