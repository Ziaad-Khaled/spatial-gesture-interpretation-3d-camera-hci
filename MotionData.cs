using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class MotionData
    {

        private LinkedList<Point3D> points = new LinkedList<Point3D>();

        private LinkedList<VelocityPoint> velocity = new LinkedList<VelocityPoint>();

        private LinkedList<VelocityPoint> velocityLast20Frames = new LinkedList<VelocityPoint>();


        private Point3D averageVelocity = new Point3D(0, 0, 0);

        private Point3D sumVelocity = new Point3D(0, 0, 0);

        private double instantVelocityMagnitude;

        private Point3D sumVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D averageVelocityLast20Frames = new Point3D(0, 0, 0);

        private Point3D instantVelocity = new Point3D(0, 0, 99);

        Boolean firstFrameAdded = false;

        Boolean secondFrameAdded = false;

        public void AddFrame(Hashtable hashtable, string joint)
        {
            Point3D lastFrame = new Point3D(0, 0, 0);

            if (firstFrameAdded)
            {
                lastFrame = this.points.Last();

            }

            this.points.AddLast(((Point3D)hashtable[joint]));
          

            if (firstFrameAdded)
            {
                this.UpdateVelocities(hashtable, lastFrame, joint);
            }
            if (!firstFrameAdded)
                firstFrameAdded = true;
        }

        private void UpdateVelocities(Hashtable hashtable, Point3D lastFrame, string joint)
        {
            double positionX = ((Point3D)hashtable[joint]).GetX();
            double positionY = ((Point3D)hashtable[joint]).GetY();
            double positionZ = ((Point3D)hashtable[joint]).GetZ();

            VelocityPoint velocityNewPoint = new VelocityPoint(positionX - lastFrame.GetX(), positionY - lastFrame.GetY(), positionZ - lastFrame.GetZ(), positionX, positionY);
            VelocityPoint point1 = null;
            VelocityPoint beforeLastVelocity = null;
            if (secondFrameAdded)
            {
                beforeLastVelocity = velocity.Last();
                point1 = velocity.Last();
            }


            velocity.AddLast(velocityNewPoint);

            sumVelocity.SetNewPoints(sumVelocity.GetX() + positionX - lastFrame.GetX(), sumVelocity.GetY() + positionY - lastFrame.GetY(), sumVelocity.GetZ() + positionZ - lastFrame.GetZ());
            averageVelocity.SetNewPoints(sumVelocity.GetX() / velocity.Count, sumVelocity.GetY() / velocity.Count, sumVelocity.GetZ() / velocity.Count);



            velocityLast20Frames.AddLast(velocityNewPoint);
            sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() + positionX - lastFrame.GetX(), sumVelocityLast20Frames.GetY() + positionY - lastFrame.GetY(), sumVelocityLast20Frames.GetZ() + positionZ - lastFrame.GetZ());

            if (velocityLast20Frames.Count > 20)
            {
                sumVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() - velocityLast20Frames.First().GetXVelocity(), sumVelocityLast20Frames.GetY() - velocityLast20Frames.First().GetYVelocity(), sumVelocityLast20Frames.GetZ() - velocityLast20Frames.First().GetZVelocity());
                velocityLast20Frames.RemoveFirst();
            }
            averageVelocityLast20Frames.SetNewPoints(sumVelocityLast20Frames.GetX() / velocityLast20Frames.Count, sumVelocityLast20Frames.GetY() / velocityLast20Frames.Count, sumVelocityLast20Frames.GetZ() / velocityLast20Frames.Count);



            if (secondFrameAdded)
            {
               // this.updateAcceleration(beforeLastVelocity, velocityNewPoint);
            }
            else
            {
                secondFrameAdded = true;
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
        }

        public void ComputeInstantVelocityMagnitude()
        {
            double total = instantVelocity.GetX() * instantVelocity.GetX() + instantVelocity.GetY() * instantVelocity.GetY();
            instantVelocityMagnitude = Math.Sqrt(total);
        }


    }
}
