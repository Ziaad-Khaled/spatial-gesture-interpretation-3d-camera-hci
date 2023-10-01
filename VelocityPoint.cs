using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class VelocityPoint
    {
        private double xVelocity;
        private double yVelocity;
        private double zVelocity;

        private Point position;

        private double velocityMagnitude;

        private int red;
        private int blue;

        public Point GetPosition()
        {
            return position;
        }

        public void SetPosition(Point value)
        {
            position = value;
        }

        public double GetXVelocity()
        {
            return xVelocity;
        }

        public void SetXVelocity(double value)
        {
            xVelocity = value;
        }

        public double GetYVelocity()
        {
            return yVelocity;
        }

        public void SetYVelocity(double value)
        {
            yVelocity = value;
        }

        public double GetZVelocity()
        {
            return zVelocity;
        }

        public void SetZVelocity(double value)
        {
            zVelocity = value;
        }

        public VelocityPoint(double xVelovity, double yVelocity, double zVelocity, double xPosition, double yPosition)
        {
            this.xVelocity = xVelovity;
            this.yVelocity = yVelocity;
            this.zVelocity = zVelocity;
            position = new Point(xPosition, yPosition);
            calculateVelocityMagnitude();
            calculateRedAndBlue();

        }
        

        public double GetRed()
        {
            return red;
        }

        public void SetRed(int value)
        {
            red = value;
        }

        public double GetBlue()
        {
            return blue;
        }

        public void SetBlue(int value)
        {
            blue = value;
        }


        public void setVelocityPoints(double x, double y)
        {
           this.xVelocity = x;
           this.yVelocity = y;
            calculateVelocityMagnitude();
            calculateRedAndBlue();
        }

        public void calculateRedAndBlue()
        {
            calculateVelocityMagnitude();
            this.red = (int)((velocityMagnitude/60) * 255);
            this.blue = (int) ((1 - velocityMagnitude/60) * 255);
        }

        public void calculateVelocityMagnitude()
        {
            double xSquared = this.xVelocity * this.xVelocity;
            double ySquared = this.yVelocity * this.yVelocity;
            this.velocityMagnitude = Math.Sqrt(xSquared + ySquared);
        }
    }
}
