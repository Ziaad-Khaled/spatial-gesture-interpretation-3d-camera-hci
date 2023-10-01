using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class RightHandMotionData : HandMotionData
    {
        private LinkedList<Point3D> rightHand = new LinkedList<Point3D>();

        private LinkedList<Point3D> spineMid = new LinkedList<Point3D>();

        private LinkedList<Point3D> rightHandLast10Frames = new LinkedList<Point3D>();

        private LinkedList<Point3D> rightHandLast20Frames = new LinkedList<Point3D>();

        private LinkedList<VelocityPoint> rightHandVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> spineMidVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<AccelerationPoint> rightHandAcceleration = new LinkedList<AccelerationPoint>();

        private LinkedList<VelocityPoint> rightHandLast2FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> rightHandLast3FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> rightHandLast10FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> rightHandLast20FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> rightHandLast30FramesVelocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> spineMidLast20FramesVelocity = new LinkedList<VelocityPoint>();


        private LinkedList<VelocityLine> velocityLines = new LinkedList<VelocityLine>();

        private Point3D sumRightHand = new Point3D(0, 0, 0);

        private Point3D averageRightHand = new Point3D(0, 0, 0);

        private Point3D sumRightHandLast20Frames = new Point3D(0, 0, 0);

        private Point3D sumRightHandLast10Frames = new Point3D(0, 0, 0);

        private Point3D averageRightHandLast20Frames = new Point3D(0, 0, 0);

        private Point3D averageRightHandLast10Frames = new Point3D(0, 0, 0);



        private Point3D averageVelocity = new Point3D(0, 0, 0);

        private Point3D averageVelocitySpineMid = new Point3D(0, 0, 0);

        private Point3D sumVelocity = new Point3D(0, 0, 0);

        private Point3D sumVelocitySpineMid = new Point3D(0, 0, 0);


        private double instantVelocityMagnitude;

        private Point3D sumVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast3Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast10Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast3Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D absoluteSumVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D sumVelocityLast20FramesForSpineMid = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast3Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast10Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast30Frames = new Point3D(0, 0, 0);

        private Point3D absoluteAverageVelocityLast2Frames = new Point3D(0, 0, 0);

        private Point3D absoluteAverageVelocityLast3Frames = new Point3D(0, 0, 0);

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
        internal Point3D AverageRightHandLast20Frames { get => averageRightHandLast20Frames; set => averageRightHandLast20Frames = value; }
        internal Point3D AverageRightHandLast10Frames { get => averageRightHandLast10Frames; set => averageRightHandLast10Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast2Frames { get => absoluteAverageVelocityLast2Frames; set => absoluteAverageVelocityLast2Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast20Frames { get => absoluteAverageVelocityLast20Frames; set => absoluteAverageVelocityLast20Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast30Frames { get => absoluteAverageVelocityLast30Frames; set => absoluteAverageVelocityLast30Frames = value; }
        internal Point3D AbsoluteAverageVelocityLast3Frames { get => absoluteAverageVelocityLast3Frames; set => absoluteAverageVelocityLast3Frames = value; }

        internal LinkedList<Point3D> GetPoints()
        {
            return rightHand;
        }

        internal void SetPoints(LinkedList<Point3D> value)
        {
            rightHand = value;
        }

        internal Point3D GetAverageRightHand()
        {
            return averageRightHand;
        }

        internal void SetAverageRightHand(Point3D value)
        {
            averageRightHand = value;
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
                double xSquared = rightHandAcceleration.Last().GetXAcceleration() * rightHandAcceleration.Last().GetXAcceleration();
                double ySquared = rightHandAcceleration.Last().GetYAcceleration() * rightHandAcceleration.Last().GetYAcceleration();
                double zSquared = rightHandAcceleration.Last().GetZAcceleration() * rightHandAcceleration.Last().GetZAcceleration();
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

        public LinkedList<Point3D> GetRightHand()
        {
            return GetPoints();
        }

        public void SetRightHand(LinkedList<Point3D> value)
        {
            SetPoints(value);
        }

        public LinkedList<Point3D> GetRightHandLast10Frames()
        {
            return rightHandLast10Frames;
        }

        public void SetRightHandLast10Frames(LinkedList<Point3D> value)
        {
            rightHandLast10Frames = value;
        }

        public LinkedList<VelocityPoint> GetRightHandVelocity()
        {
            return rightHandVelocity;
        }

        public void SetRightHandVelocity(LinkedList<VelocityPoint> value)
        {
            rightHandVelocity = value;
        }

        public LinkedList<VelocityPoint> GetRightHandLast10FramesVelocity()
        {
            return rightHandLast10FramesVelocity;
        }

        public void SetRightHandLast10FramesVelocity(LinkedList<VelocityPoint> value)
        {
            rightHandLast10FramesVelocity = value;
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

            Point3D lastFrame = ((Point3D)hashtable["HandRight"]);
            Point3D lastFrameInLast10Frames = ((Point3D)hashtable["HandRight"]);
            Point3D lastFrameSpineMid = (Point3D)hashtable["SpineMid"];

            if (firstFrameAdded)
            {
                lastFrame = this.GetPoints().Last();

                lastFrameSpineMid = this.spineMid.Last();

                lastFrameInLast10Frames = this.rightHandLast10Frames.Last();
            }

            this.GetPoints().AddLast(((Point3D)hashtable["HandRight"]));

            this.spineMid.AddLast(((Point3D)hashtable["SpineMid"]));

            rightHandLast10Frames.AddLast((Point3D)hashtable["HandRight"]);

            rightHandLast20Frames.AddLast((Point3D)hashtable["HandRight"]);

            sumRightHand.SetNewPoints(sumRightHand.GetX() + ((Point3D)hashtable["HandRight"]).GetX(), sumRightHand.GetY() + ((Point3D)hashtable["HandRight"]).GetY(), sumRightHand.GetZ() + ((Point3D)hashtable["HandRight"]).GetZ());
            averageRightHand.SetNewPoints(sumRightHand.GetX() / GetPoints().Count, sumRightHand.GetY() / GetPoints().Count, sumRightHand.GetZ() / GetPoints().Count);


            sumRightHandLast20Frames.SetNewPoints(sumRightHandLast20Frames.GetX() + ((Point3D)hashtable["HandRight"]).GetX(), sumRightHandLast20Frames.GetY() + ((Point3D)hashtable["HandRight"]).GetY(), sumRightHandLast20Frames.GetZ() + ((Point3D)hashtable["HandRight"]).GetZ());

            if (rightHandLast20FramesVelocity.Count > 20)
            {
                sumRightHandLast20Frames.SetNewPoints(sumRightHandLast20Frames.GetX() - rightHandLast20Frames.First.Value.GetX(), sumRightHandLast20Frames.GetY() - rightHandLast20Frames.First.Value.GetY(), sumRightHandLast20Frames.GetZ() - rightHandLast20Frames.First.Value.GetZ());
                rightHandLast20Frames.RemoveFirst();
            }

            averageRightHandLast20Frames.SetNewPoints(sumRightHandLast20Frames.GetX() / rightHandLast20Frames.Count, sumRightHandLast20Frames.GetY() / rightHandLast20Frames.Count, sumRightHandLast20Frames.GetZ() / rightHandLast20Frames.Count);

            sumRightHandLast10Frames.SetNewPoints(sumRightHandLast10Frames.GetX() + ((Point3D)hashtable["HandRight"]).GetX(), sumRightHandLast10Frames.GetY() + ((Point3D)hashtable["HandRight"]).GetY(), sumRightHandLast10Frames.GetZ() + ((Point3D)hashtable["HandRight"]).GetZ());
            if (rightHandLast10Frames.Count > 10)
            {
                sumRightHandLast10Frames.SetNewPoints(sumRightHandLast10Frames.GetX() - rightHandLast10Frames.First.Value.GetX(), sumRightHandLast10Frames.GetY() - rightHandLast10Frames.First.Value.GetY(), sumRightHandLast10Frames.GetZ() - rightHandLast10Frames.First.Value.GetZ());
                rightHandLast10Frames.RemoveFirst();
            }
            averageRightHandLast10Frames.SetNewPoints(sumRightHandLast10Frames.GetX() / rightHandLast10Frames.Count, sumRightHandLast10Frames.GetY() / rightHandLast10Frames.Count, sumRightHandLast10Frames.GetZ() / rightHandLast10Frames.Count);


            if (firstFrameAdded)
            {
                this.UpdateVelocities(hashtable, lastFrame, lastFrameSpineMid);
            }
            if (!firstFrameAdded)
                firstFrameAdded = true;
        }

        private void UpdateVelocities(Hashtable hashtable, Point3D lastFrame, Point3D lastFrameSpineMid)
        {
            double handPositionXRightHand = ((Point3D)hashtable["HandRight"]).GetX();
            double handPositionYRightHand = ((Point3D)hashtable["HandRight"]).GetY();
            double handPositionZRightHand = ((Point3D)hashtable["HandRight"]).GetZ();

            Point3D spineMid = ((Point3D)hashtable["SpineMid"]);
            VelocityPoint rightHandVelocityNewPoint = new VelocityPoint(handPositionXRightHand - lastFrame.GetX(), handPositionYRightHand - lastFrame.GetY(), handPositionZRightHand - lastFrame.GetZ(), handPositionXRightHand, handPositionYRightHand);
            VelocityPoint spineMidVelocityNewPoint = new VelocityPoint(spineMid.GetX() - lastFrameSpineMid.GetX(), spineMid.GetY() - lastFrameSpineMid.GetY(), spineMid.GetZ() - lastFrameSpineMid.GetZ(), spineMid.GetX(), spineMid.GetY());
            VelocityPoint point1SpineMid = null;
            VelocityPoint point1RightHand = null;
            VelocityPoint spineMidBeforeLastVelocity = null;
            VelocityPoint rightHandBeforeLastVelocity = null;
            if (secondFrameAdded)
            {
                rightHandBeforeLastVelocity = rightHandVelocity.Last();
                spineMidBeforeLastVelocity = spineMidVelocity.Last();
                point1RightHand = rightHandVelocity.Last();
                point1SpineMid = spineMidVelocity.Last();
            }


            rightHandVelocity.AddLast(rightHandVelocityNewPoint);

            spineMidVelocity.AddLast(spineMidVelocityNewPoint);

            sumVelocity.SetNewPoints(sumVelocity.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocity.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocity.GetZ() + handPositionZRightHand - lastFrame.GetZ());
            averageVelocity.SetNewPoints(sumVelocity.GetX() / rightHandVelocity.Count, sumVelocity.GetY() / rightHandVelocity.Count, sumVelocity.GetZ() / rightHandVelocity.Count);

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


            rightHandLast20FramesVelocity.AddLast(rightHandVelocityNewPoint);
            sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocityLast20Frames.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocityLast20Frames.GetZ() + handPositionZRightHand - lastFrame.GetZ());
            absoluteSumVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() + Math.Abs(handPositionXRightHand - lastFrame.GetX()), absoluteSumVelocityLast20Frames.GetY() + Math.Abs(handPositionYRightHand - lastFrame.GetY()), absoluteSumVelocityLast20Frames.GetZ() + Math.Abs(handPositionZRightHand - lastFrame.GetZ()));

            if (rightHandLast20FramesVelocity.Count > 25)
            {
                sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() - rightHandLast20FramesVelocity.First().GetXVelocity(), sumVelocityLast20Frames.GetY() - rightHandLast20FramesVelocity.First().GetYVelocity(), sumVelocityLast20Frames.GetZ() - rightHandLast20FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() - Math.Abs(rightHandLast20FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast20Frames.GetY() - Math.Abs(rightHandLast20FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast20Frames.GetZ() - Math.Abs(rightHandLast20FramesVelocity.First().GetZVelocity()));
                rightHandLast20FramesVelocity.RemoveFirst();
            }
            averageVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() / rightHandLast20FramesVelocity.Count, sumVelocityLast20Frames.GetY() / rightHandLast20FramesVelocity.Count, sumVelocityLast20Frames.GetZ() / rightHandLast20FramesVelocity.Count);
            absoluteAverageVelocityLast20Frames.SetNewPoints(absoluteSumVelocityLast20Frames.GetX() / rightHandLast20FramesVelocity.Count, absoluteSumVelocityLast20Frames.GetY() / rightHandLast20FramesVelocity.Count, absoluteSumVelocityLast20Frames.GetZ() / rightHandLast20FramesVelocity.Count);



            rightHandLast30FramesVelocity.AddLast(rightHandVelocityNewPoint);
            sumVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocityLast30Frames.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocityLast30Frames.GetZ() + handPositionZRightHand - lastFrame.GetZ());
            absoluteSumVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() + Math.Abs(handPositionXRightHand - lastFrame.GetX()), absoluteSumVelocityLast30Frames.GetY() + Math.Abs(handPositionYRightHand - lastFrame.GetY()), absoluteSumVelocityLast30Frames.GetZ() + Math.Abs(handPositionZRightHand - lastFrame.GetZ()));

            if (rightHandLast30FramesVelocity.Count > 35)
            {
                sumVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() - rightHandLast30FramesVelocity.First().GetXVelocity(), sumVelocityLast30Frames.GetY() - rightHandLast30FramesVelocity.First().GetYVelocity(), sumVelocityLast30Frames.GetZ() - rightHandLast30FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() - Math.Abs(rightHandLast30FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast30Frames.GetY() - Math.Abs(rightHandLast30FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast30Frames.GetZ() - Math.Abs(rightHandLast30FramesVelocity.First().GetZVelocity()));
                rightHandLast30FramesVelocity.RemoveFirst();
            }
            averageVelocityLast30Frames.SetNewPoints(sumVelocityLast30Frames.GetX() / rightHandLast30FramesVelocity.Count, sumVelocityLast30Frames.GetY() / rightHandLast30FramesVelocity.Count, sumVelocityLast30Frames.GetZ() / rightHandLast30FramesVelocity.Count);
            AbsoluteAverageVelocityLast30Frames.SetNewPoints(absoluteSumVelocityLast30Frames.GetX() / rightHandLast30FramesVelocity.Count, absoluteSumVelocityLast30Frames.GetY() / rightHandLast30FramesVelocity.Count, absoluteSumVelocityLast30Frames.GetZ() / rightHandLast30FramesVelocity.Count);




            rightHandLast3FramesVelocity.AddLast(rightHandVelocityNewPoint);
            sumVelocityLast3Frames.SetNewPoints(sumVelocityLast3Frames.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocityLast3Frames.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocityLast3Frames.GetZ() + handPositionZRightHand - lastFrame.GetZ());
            absoluteSumVelocityLast3Frames.SetNewPoints(absoluteSumVelocityLast3Frames.GetX() + Math.Abs(handPositionXRightHand - lastFrame.GetX()), absoluteSumVelocityLast3Frames.GetY() + Math.Abs(handPositionYRightHand - lastFrame.GetY()), absoluteSumVelocityLast3Frames.GetZ() + Math.Abs(handPositionZRightHand - lastFrame.GetZ()));

            if (rightHandLast3FramesVelocity.Count > 3)
            {
                sumVelocityLast3Frames.SetNewPoints(sumVelocityLast3Frames.GetX() - rightHandLast3FramesVelocity.First().GetXVelocity(), sumVelocityLast3Frames.GetY() - rightHandLast3FramesVelocity.First().GetYVelocity(), sumVelocityLast3Frames.GetZ() - rightHandLast3FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast3Frames.SetNewPoints(absoluteSumVelocityLast3Frames.GetX() - Math.Abs(rightHandLast3FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast3Frames.GetY() - Math.Abs(rightHandLast3FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast3Frames.GetZ() - Math.Abs(rightHandLast3FramesVelocity.First().GetZVelocity()));
                rightHandLast3FramesVelocity.RemoveFirst();
            }
            averageVelocityLast3Frames.SetNewPoints(sumVelocityLast3Frames.GetX() / rightHandLast3FramesVelocity.Count, sumVelocityLast3Frames.GetY() / rightHandLast3FramesVelocity.Count, sumVelocityLast3Frames.GetZ() / rightHandLast3FramesVelocity.Count);
            AbsoluteAverageVelocityLast3Frames.SetNewPoints(absoluteSumVelocityLast3Frames.GetX() / rightHandLast3FramesVelocity.Count, absoluteSumVelocityLast3Frames.GetY() / rightHandLast3FramesVelocity.Count, absoluteSumVelocityLast3Frames.GetZ() / rightHandLast3FramesVelocity.Count);



            /*
            double ratio = (rightHandXVelocity.Count - 1) / rightHandXVelocity.Count;

            averageVelocityX = averageVelocityX*ratio + rightHandXVelocity.Last()/ rightHandXVelocity.Count;
            averageVelocityY = averageVelocityY*ratio + rightHandYVelocity.Last()/ rightHandYVelocity.Count;
            averageVelocityZ = averageVelocityZ*ratio + rightHandZVelocity.Last()/ rightHandZVelocity.Count;
            */

            rightHandLast2FramesVelocity.AddLast(rightHandVelocityNewPoint);
            sumVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocityLast2Frames.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocityLast2Frames.GetZ() + handPositionZRightHand - lastFrame.GetZ());
            absoluteSumVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() + Math.Abs(handPositionXRightHand - lastFrame.GetX()), absoluteSumVelocityLast2Frames.GetY() + Math.Abs(handPositionYRightHand - lastFrame.GetY()), absoluteSumVelocityLast2Frames.GetZ() + Math.Abs(handPositionZRightHand - lastFrame.GetZ()));


            if (rightHandLast2FramesVelocity.Count > 10)
            {
                sumVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() - rightHandLast2FramesVelocity.First().GetXVelocity(), sumVelocityLast2Frames.GetY() - rightHandLast2FramesVelocity.First().GetYVelocity(), sumVelocityLast2Frames.GetZ() - rightHandLast2FramesVelocity.First().GetZVelocity());
                absoluteSumVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() - Math.Abs(rightHandLast2FramesVelocity.First().GetXVelocity()), absoluteSumVelocityLast2Frames.GetY() - Math.Abs(rightHandLast2FramesVelocity.First().GetYVelocity()), absoluteSumVelocityLast2Frames.GetZ() - Math.Abs(rightHandLast2FramesVelocity.First().GetZVelocity()));

                rightHandLast2FramesVelocity.RemoveFirst();
            }
            averageVelocityLast2Frames.SetNewPoints(sumVelocityLast2Frames.GetX() / rightHandLast2FramesVelocity.Count, sumVelocityLast2Frames.GetY() / rightHandLast2FramesVelocity.Count, sumVelocityLast2Frames.GetZ() / rightHandLast2FramesVelocity.Count);
            absoluteAverageVelocityLast2Frames.SetNewPoints(absoluteSumVelocityLast2Frames.GetX() / rightHandLast2FramesVelocity.Count, absoluteSumVelocityLast2Frames.GetY() / rightHandLast2FramesVelocity.Count, absoluteSumVelocityLast2Frames.GetZ() / rightHandLast2FramesVelocity.Count);

            Console.WriteLine("absolute sum is: " + absoluteSumVelocityLast2Frames.ToString());
            Console.WriteLine("absolute average velocity is: " + absoluteAverageVelocityLast2Frames.ToString());



            rightHandVelocityNewPoint = new VelocityPoint(handPositionXRightHand - lastFrame.GetX(), handPositionYRightHand - lastFrame.GetY(), handPositionZRightHand - lastFrame.GetZ(), handPositionXRightHand, handPositionYRightHand);
            rightHandLast10FramesVelocity.AddLast(rightHandVelocityNewPoint);

            sumVelocityLast10Frames.SetNewPoints(sumVelocityLast10Frames.GetX() + handPositionXRightHand - lastFrame.GetX(), sumVelocityLast10Frames.GetY() + handPositionYRightHand - lastFrame.GetY(), sumVelocityLast10Frames.GetZ() + handPositionZRightHand - lastFrame.GetZ());


            if (rightHandLast10FramesVelocity.Count > 10)
            {
                sumVelocityLast10Frames.SetX(sumVelocityLast10Frames.GetX() - rightHandLast10FramesVelocity.First().GetXVelocity());
                sumVelocityLast10Frames.SetY(sumVelocityLast10Frames.GetY() - rightHandLast10FramesVelocity.First().GetYVelocity());
                sumVelocityLast10Frames.SetZ(sumVelocityLast10Frames.GetZ() - rightHandLast10FramesVelocity.First().GetZVelocity());

                rightHandLast10FramesVelocity.RemoveFirst();

            }

            averageVelocityLast10Frames.SetX(sumVelocityLast10Frames.GetX() / rightHandLast10FramesVelocity.Count);
            averageVelocityLast10Frames.SetY(sumVelocityLast10Frames.GetY() / rightHandLast10FramesVelocity.Count);
            averageVelocityLast10Frames.SetZ(sumVelocityLast10Frames.GetZ() / rightHandLast10FramesVelocity.Count);



            if (secondFrameAdded)
            {
                VelocityLine newVelocityLine = new VelocityLine(point1RightHand, rightHandVelocityNewPoint);
                velocityLines.AddLast(newVelocityLine);
                this.updateAcceleration(rightHandBeforeLastVelocity, rightHandVelocityNewPoint);
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
            SetInstantVelocity(rightHandVelocityNewPoint);
        }

        public void updateAcceleration(VelocityPoint beforeLastVelocity, VelocityPoint lastVelocity)
        {
            AccelerationPoint newPoint = new AccelerationPoint(lastVelocity.GetXVelocity() - beforeLastVelocity.GetXVelocity(), lastVelocity.GetYVelocity() - beforeLastVelocity.GetYVelocity(), lastVelocity.GetZVelocity() - beforeLastVelocity.GetZVelocity(), lastVelocity.GetPosition());
            sumAcceleration.SetNewPoints(sumAcceleration.GetX() + newPoint.GetXAcceleration(), sumAcceleration.GetY() + newPoint.GetYAcceleration(), sumAcceleration.GetZ() + newPoint.GetZAcceleration());

            rightHandAcceleration.AddLast(newPoint);

            GetAverageAccelration().SetX(sumAcceleration.GetX() / rightHandAcceleration.Count);
            GetAverageAccelration().SetY(GetAverageAccelration().GetY() / rightHandAcceleration.Count);
            GetAverageAccelration().SetZ(GetAverageAccelration().GetZ() / rightHandAcceleration.Count);
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
            if (Get2DMagnitude(AbsoluteAverageVelocityLast30Frames) < 2 && rightHandLast30FramesVelocity.Count > 24)
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

            GetPoints().Clear();
            rightHandLast10Frames.Clear();
            rightHandVelocity.Clear();
            rightHandLast2FramesVelocity.Clear();
            rightHandLast3FramesVelocity.Clear();
            rightHandLast10FramesVelocity.Clear();
            rightHandLast20FramesVelocity.Clear();
            rightHandLast30FramesVelocity.Clear();
            velocityLines.Clear();
            averageVelocity.Reset();
            sumVelocity.Reset();
            averageVelocityLast2Frames.Reset();
            averageVelocityLast3Frames.Reset();
            averageVelocityLast10Frames.Reset();
            averageVelocityLast20Frames.Reset();
            averageVelocityLast30Frames.Reset();
            instantVelocityMagnitude = 0;
            sumVelocityLast2Frames.Reset();
            sumVelocityLast3Frames.Reset();
            sumVelocityLast10Frames.Reset();
            sumVelocityLast20Frames.Reset();
            sumVelocityLast30Frames.Reset();

            instantVelocity.Reset();
            accelerationFlag = false;
            firstFrameAdded = false;
            secondFrameAdded = false;
            motionData = new List<Hashtable>();
            sumRightHand.Reset();
            averageRightHand.Reset();

            sumRightHandLast10Frames.Reset();
            sumRightHandLast20Frames.Reset();
            averageRightHandLast20Frames.Reset();
            averageRightHandLast10Frames.Reset();

            absoluteAverageVelocityLast2Frames.Reset();
            absoluteAverageVelocityLast3Frames.Reset();
            absoluteAverageVelocityLast20Frames.Reset();
            AbsoluteAverageVelocityLast30Frames.Reset();

            absoluteSumVelocityLast20Frames.Reset();
            absoluteSumVelocityLast3Frames.Reset();
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

            Console.WriteLine("average velocity magnitude: " + averageVelocityMagnitude);

            double xDifference = Math.Abs(averageVelocityUnitVector.GetX() - instantVelocityUnitVector.GetX());
            double yDifference = Math.Abs(averageVelocityUnitVector.GetY() - instantVelocityUnitVector.GetY());

            Console.WriteLine("X Difference: " + xDifference);
            Console.WriteLine("not the same sign: " + !((averageVelocity.GetX() >= 0) ^ (instantVelocity.GetX() < 0)));
            Console.WriteLine("y Difference: " + yDifference);
            Console.WriteLine("not the same sign: " + !((averageVelocity.GetY() >= 0) ^ (instantVelocity.GetY() < 0)));

            Boolean zeroVelocity = false;

            if (Math.Abs(instantVelocity.GetX()) < averageVelocityMagnitude/1.8 && Math.Abs(instantVelocity.GetY()) < averageVelocityMagnitude/1.8 && instantVelocity.GetX() != 0 && instantVelocity.GetY() != 0)
                zeroVelocity = true;

            if ((xDifference > threshold && !((averageVelocity.GetX() >= 0) ^ (instantVelocity.GetX() < 0))) || (yDifference > threshold && !((averageVelocity.GetY() >= 0) ^ (instantVelocity.GetY() < 0)))  || zeroVelocity)
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
