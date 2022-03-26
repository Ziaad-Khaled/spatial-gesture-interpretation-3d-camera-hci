using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class GestureState
    {
        private bool gesture;

        private int state = 0;
        //1 : preperation
        //2 : nucleus
        //3 : retraction

        private int type = 0;


        //1 : swipe left
        //2 : swipe right
        //3 : rotate


        public int GetType()
        {
            return type;
        }

        public void SetType(int value)
        {
            type = value;
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
    }
}
