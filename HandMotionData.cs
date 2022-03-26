using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class HandMotionData
    {
        LinkedList<double> rightHandX  = new LinkedList<double>();
        LinkedList<double> rightHandY = new LinkedList<double>();
        LinkedList<double> rightHandZ = new LinkedList<double>();

        LinkedList<double> rightHandXLast10Frames = new LinkedList<double>();
        LinkedList<double> rightHandYLast10Frames = new LinkedList<double>();
        LinkedList<double> rightHandZLast10Frames = new LinkedList<double>();

        LinkedList<double> rightHandXVelocity = new LinkedList<double>();
        LinkedList<double> rightHandYVelocity = new LinkedList<double>();
        LinkedList<double> rightHandZVelocity = new LinkedList<double>();

        LinkedList<double> rightHandXLast10FramesVelocity = new LinkedList<double>();
        LinkedList<double> rightHandYLast10FramesVelocity = new LinkedList<double>();
        LinkedList<double> rightHandZLast10FramesVelocity = new LinkedList<double>();

        double averageVelocityX = 0;
        double averageVelocityY = 0;
        double averageVelocityZ = 0;

        double sumVelocityX = 0;
        double sumVelocityY = 0;
        double sumVelocityZ = 0;

        double averageVelocityXLast10Frames = 0;
        double averageVelocityYLast10Frames = 0;
        double averageVelocityZLast10Frames = 0;

        double sumVelocityXLast10Frames = 0;
        double sumVelocityYLast10Frames = 0;
        double sumVelocityZLast10Frames = 0;

        Boolean firstFrameAdded = false;


        public double GetAverageVelocityX()
        {
            return averageVelocityX;
        }

        public void SetAverageVelocityX(double value)
        {
            averageVelocityX = value;
        }

        public double GetAverageVelocityY()
        {
            return averageVelocityY;
        }

        public void SetAverageVelocityY(double value)
        {
            averageVelocityY = value;
        }

        public double GetAverageVelocityZ()
        {
            return averageVelocityZ;
        }

        public void SetAverageVelocityZ(double value)
        {
            averageVelocityZ = value;
        }

        public double GetAverageVelocityXLast10Frames()
        {
            return averageVelocityXLast10Frames;
        }

        public void SetAverageVelocityXLast10Frames(double value)
        {
            averageVelocityXLast10Frames = value;
        }

        public double GetAverageVelocityYLast10Frames()
        {
            return averageVelocityYLast10Frames;
        }

        public void SetAverageVelocityYLast10Frames(double value)
        {
            averageVelocityYLast10Frames = value;
        }

        public double GetAverageVelocityZLast10Frames()
        {
            return averageVelocityZLast10Frames;
        }

        public void SetAverageVelocityZLast10Frames(double value)
        {
            averageVelocityZLast10Frames = value;
        }

        public LinkedList<double> GetRightHandY()
        {
            return rightHandY;
        }

        public void SetRightHandY(LinkedList<double> value)
        {
            rightHandY = value;
        }

        public LinkedList<double> GetRightHandZ()
        {
            return rightHandZ;
        }

        public void SetRightHandZ(LinkedList<double> value)
        {
            rightHandZ = value;
        }

        public LinkedList<double> GetRightHandX()
        {
            return rightHandX;
        }

        public void SetRightHandX(LinkedList<double> value)
        {
            rightHandX = value;
        }

        public void AddFrame(double handPositionX,double handPositionY,double handPositionZ)
        {
            double lastFrameX = 0;
            double lastFrameY = 0;
            double lastFrameZ = 0;
            double lastFrameXInLast10Frames = 0;
            double lastFrameYInLast10Frames = 0;
            double lastFrameZInLast10Frames = 0;
            if (firstFrameAdded)
            {
                lastFrameX = rightHandX.Last();
                lastFrameY = rightHandY.Last();
                lastFrameZ = rightHandZ.Last();
                lastFrameXInLast10Frames = rightHandXLast10Frames.Last();
                lastFrameYInLast10Frames = rightHandYLast10Frames.Last();
                lastFrameZInLast10Frames = rightHandZLast10Frames.Last();
            }
            
            rightHandX.AddLast(handPositionX);
            rightHandY.AddLast(handPositionY);
            rightHandZ.AddLast(handPositionZ);
            
            rightHandXLast10Frames.AddLast(handPositionX);
            rightHandYLast10Frames.AddLast(handPositionY);
            rightHandZLast10Frames.AddLast(handPositionZ);

            if(rightHandX.Count > 60)
            {
                averageVelocityX = averageVelocityX - rightHandX.First();
                rightHandX.RemoveFirst();

                averageVelocityY = averageVelocityY - rightHandY.First();
                rightHandY.RemoveFirst();

                averageVelocityZ = averageVelocityZ - rightHandZ.First();
                rightHandZ.RemoveFirst();
            }

            if (rightHandXLast10Frames.Count > 10)
            {
                averageVelocityXLast10Frames = averageVelocityXLast10Frames - rightHandXLast10Frames.First();
                rightHandXLast10Frames.RemoveFirst();
                
                averageVelocityYLast10Frames = averageVelocityYLast10Frames - rightHandYLast10Frames.First();
                rightHandYLast10Frames.RemoveFirst();

                averageVelocityZLast10Frames = averageVelocityZLast10Frames - rightHandZLast10Frames.First();
                rightHandZLast10Frames.RemoveFirst();
            }
            if (firstFrameAdded)
            {
                this.UpdateVelocities(handPositionX, handPositionY, handPositionZ, lastFrameX, lastFrameY, lastFrameZ);
            }
            if (!firstFrameAdded)
                firstFrameAdded = true;
        }

        private void UpdateVelocities(double handPositionX, double handPositionY, double handPositionZ, double lastFrameX, double lastFrameY, double lastFrameZ)
        {
            rightHandXVelocity.AddLast(handPositionX - lastFrameX);
            rightHandYVelocity.AddLast(handPositionY - lastFrameY);
            rightHandZVelocity.AddLast(handPositionZ - lastFrameZ);


            sumVelocityX = sumVelocityX + handPositionX - lastFrameX;
            sumVelocityY = sumVelocityY + handPositionY - lastFrameY;
            sumVelocityZ = sumVelocityZ + handPositionZ - lastFrameZ;



            if (rightHandXVelocity.Count > 60)
            {
                sumVelocityX = sumVelocityX - rightHandXVelocity.First();
                sumVelocityY = sumVelocityY - rightHandYVelocity.First();
                sumVelocityZ = sumVelocityZ - rightHandZVelocity.First();

                rightHandXVelocity.RemoveFirst();
                rightHandYVelocity.RemoveFirst();
                rightHandZVelocity.RemoveFirst();
            }

            averageVelocityX = sumVelocityX / rightHandXVelocity.Count;
            averageVelocityY = sumVelocityY / rightHandYVelocity.Count;
            averageVelocityZ = sumVelocityZ / rightHandZVelocity.Count;

            /*
            double ratio = (rightHandXVelocity.Count - 1) / rightHandXVelocity.Count;

            averageVelocityX = averageVelocityX*ratio + rightHandXVelocity.Last()/ rightHandXVelocity.Count;
            averageVelocityY = averageVelocityY*ratio + rightHandYVelocity.Last()/ rightHandYVelocity.Count;
            averageVelocityZ = averageVelocityZ*ratio + rightHandZVelocity.Last()/ rightHandZVelocity.Count;
            */

            rightHandXLast10FramesVelocity.AddLast(handPositionX - lastFrameX);
            rightHandYLast10FramesVelocity.AddLast(handPositionY - lastFrameY);
            rightHandZLast10FramesVelocity.AddLast(handPositionZ - lastFrameZ);

            sumVelocityXLast10Frames = sumVelocityXLast10Frames + handPositionX - lastFrameX;
            sumVelocityYLast10Frames = sumVelocityYLast10Frames + handPositionY - lastFrameY;
            sumVelocityZLast10Frames = sumVelocityZLast10Frames + handPositionZ - lastFrameZ;



            if (rightHandXLast10FramesVelocity.Count > 10)
            {
                sumVelocityXLast10Frames = sumVelocityXLast10Frames - rightHandXLast10FramesVelocity.First();
                sumVelocityYLast10Frames = sumVelocityYLast10Frames - rightHandYLast10FramesVelocity.First();
                sumVelocityZLast10Frames = sumVelocityZLast10Frames - rightHandZLast10FramesVelocity.First();

                rightHandXLast10FramesVelocity.RemoveFirst();
                rightHandYLast10FramesVelocity.RemoveFirst();
                rightHandZLast10FramesVelocity.RemoveFirst();
            }

            averageVelocityXLast10Frames = sumVelocityXLast10Frames / rightHandXLast10FramesVelocity.Count;
            averageVelocityYLast10Frames = sumVelocityYLast10Frames / rightHandYLast10FramesVelocity.Count;
            averageVelocityZLast10Frames = sumVelocityZLast10Frames / rightHandZLast10FramesVelocity.Count;

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

        }
    }
}
