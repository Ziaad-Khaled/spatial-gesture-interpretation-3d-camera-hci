//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing right shoulder Anchor
        /// </summary>
        private readonly Brush ShoulderClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));



        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Anchors
        /// </summary>
        private jointPoint leftHandAnchor = new jointPoint();

        private Point rightHandAnchor = new Point();
        private double rightHandAnchorZ= 0;

        private jointPoint leftShoulderAnchor = new jointPoint();
        private Point rightShoulderAnchor = new Point();

        private double shoulderReference = 0;

        private double anchorThreshold = 20;

        private Gesture gesture = new Gesture();

        private double lastRightHandZ = 0;
        private double lastRightHandX = 0;
        private double lastRightHandY = 0;

        double instantVelocityZ = 99;
        double instantVelocityX = 0;
        double instantVelocityY = 0;

        double velocityMagnitude = 0;

        private HandMotionData handMotionData = new HandMotionData();


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    double spineMidZ = 0;
                    double spineMidY = 0;

                    double handRightX = 0;
                    double handRightY = 0;
                    double handRightZ = 0;

                    double rightShoulderX = 0;
                    double rightShoulderY = 0;
                    double rightShoulderZ = 0;

                    double leftShoulderX = 0;
                    double leftShoulderZ = 0;
                    double thumbRightY = 0;
                    int penIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                            double distance;
                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // sometimes the depth(Z) of an inferred joint may show as negative
                                // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                               

                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);


                                //Debug.WriteLine(jointType + ": X: " + position.X + " - Y: " + position.Y + "- Z: " + position.Z);


                                // setting the anchors 
                                if (jointType.ToString() == "HandLeft")
                                {
                                    //Debug.WriteLine("Anchor" + ": X: " + leftHandAnchor.GetX() +" - Y: " + leftHandAnchor.GetY() + "- Z: " + leftHandAnchor.GetZ()) ;


                                    if (leftHandAnchor.GetX() == 0.0)
                                    {
                                        setNewAnchorPoints(0, depthSpacePoint.X, depthSpacePoint.Y);

                                    }
                                    else
                                    {
                                        distance = distanceBetweenTwoPoints(leftHandAnchor.GetX(), leftHandAnchor.GetY(), depthSpacePoint.X, depthSpacePoint.Y);
                                        //Debug.WriteLine(distance) ;
                                        if (Math.Abs(distance) > anchorThreshold)
                                            setNewAnchorPoints(0, depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                }
                                if (jointType.ToString() == "HandRight")
                                {
                                    handRightZ = position.Z;

                                    if (rightHandAnchor.X == 0.0)
                                    {
                                        setNewAnchorPoints(1, depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                    else
                                    {
                                        distance = distanceBetweenTwoPoints(rightHandAnchor.X, rightHandAnchor.Y, depthSpacePoint.X, depthSpacePoint.Y);
                                        if (Math.Abs(distance) > anchorThreshold)
                                            setNewAnchorPoints(1, depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                    handRightX = depthSpacePoint.X;
                                    handRightY = depthSpacePoint.Y;

                                }
                                if (jointType.ToString() == "ShoulderRight")
                                {
                                    if (rightShoulderAnchor.X == 0.0)
                                    {
                                        setNewAnchorPoints(2, depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                    else
                                    {
                                        distance = distanceBetweenTwoPoints(rightShoulderAnchor.X, rightShoulderAnchor.Y, depthSpacePoint.X, depthSpacePoint.Y);
                                        if (Math.Abs(distance) > anchorThreshold)
                                            setNewAnchorPoints(2, depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                    rightShoulderX = depthSpacePoint.X;
                                    rightShoulderY = depthSpacePoint.Y;
                                    //converting to the depth space
                                    //Debug.WriteLine("position Right Shoulder " + position.Z);
                                    rightShoulderZ = position.Z *1080 / 4;
                                }
                                if (jointType.ToString() == "ShoulderLeft" )
                                {
                                   
                                    leftShoulderX = depthSpacePoint.X;
                                    //Debug.WriteLine("position left Shoulder " + position.Z);

                                    leftShoulderZ = position.Z * 1080/4;
                                }
                                if (jointType.ToString() == "SpineMid")
                                {
                                    spineMidZ = position.Z;
                                    spineMidY = depthSpacePoint.Y;
                                }
                                if (jointType.ToString() == "ThumbRight")
                                {
                                    thumbRightY = depthSpacePoint.Y;
                                }
                            }

                            double shoulderReferenceX = rightShoulderX - leftShoulderX;
                            double shoulderReferenceZ = rightShoulderZ - leftShoulderZ;
                            shoulderReference = Math.Sqrt(shoulderReferenceX*shoulderReferenceX + shoulderReferenceZ * shoulderReferenceZ);

                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                           // Debug.WriteLine("Right Hand: " + jointPoints[JointType.HandRight]);
                           // Debug.WriteLine("Right Shoulder: " + rightShoulderAnchor);
                            this.drawShoulderAnchor(rightShoulderAnchor, dc);

                            updateGestureState(handRightX, handRightY, handRightZ,spineMidY, spineMidZ, rightShoulderX, rightShoulderY);
                            handMotionData.AddFrame(handRightX,handRightY,handRightZ);

                            if(gesture.GetGesture() == true)
                                checkGestureType(thumbRightY, handRightX, handRightY, rightShoulderX, rightShoulderY);

                            Point point = new Point();
                            String text = "Doing a gesture: " + gesture.GetGesture().ToString();
                            printOnScreen(point, dc, text);

                            point.Y = 20;
                            text = "Swipe Left: " + gesture.GetGesturesDictonery()[0].ToString();
                            printOnScreen(point, dc, text);

                            point.Y = 40;
                            text = "Swipe Right: " + gesture.GetGesturesDictonery()[1].ToString();
                            printOnScreen(point, dc, text);

                            point.Y = 60;
                            text = "RotationFlag: " + gesture.GetRotationFlag();
                            printOnScreen(point, dc, text);
                        }
                    }
       
                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }

        private void checkGestureType(double thumbRightY, double handRightX, double handRightY, double rightShoulderX, double rightShoulderY)
        {
            Boolean[] gesturesDictionery = gesture.GetGesturesDictonery();
            if(gesturesDictionery[0] == false)
            {
                checkForSwipeLeft(thumbRightY, handRightY);
            }
            else
            {
               checkIfSwipeLeftHasEnded(thumbRightY, handRightY);
            }

            if (gesturesDictionery[1] == false)
            {
                checkForSwipeRight(thumbRightY,handRightY);
            }
            else
            {
                checkIfSwipeRightHasEnded(thumbRightY,handRightY);
            }

            if (gesturesDictionery[2] == false)
            {
                if (gesture.GetRotationFlag())
                    checkForRotation(handRightX, handRightY, rightShoulderX, rightShoulderY);
            }
            else
            {
                //checkIfRotationHasEnded();
            }
        }

        private void checkForRotation(double handRightX, double handRightY, double rightShoulderX, double rightShoulderY)
        {
            double radius = calculateRadius(handRightX, handRightY, rightShoulderX, rightShoulderY);
            if(gesture.GetRotationFirstPointX() == -99)
            {
                gesture.SetRotationFirstPointX(handRightX);
                gesture.SetRotationFirstPointY(handRightY);
                gesture.SetStandardRadius(calculateRadius(handRightX, handRightY, rightShoulderX, rightShoulderY));

                
            }
            Debug.WriteLine("rotationFirstPointX: " + gesture.GetRotationFirstPointX());
            Debug.WriteLine("rotationFirstPointY: " + gesture.GetRotationFirstPointY());
            Debug.WriteLine("standardRadius: " + gesture.GetStandardRadius());
            Debug.WriteLine("handRightX: " + handRightX);
            Debug.WriteLine("handRightY: " + handRightY);
            Debug.WriteLine("radius: " + radius);

            if (gesture.GetStandardRadius()> radius - 10 || gesture.GetStandardRadius() > radius + 10)
            {
                gesture.SetRotationFlag(false);
                return;
            }
            /*
            Debug.WriteLine("radius: " + radius);
            Debug.WriteLine("instant Velocity X: " + instantVelocityX);
            Debug.WriteLine("instant Velocity Y: " + instantVelocityY);
            Debug.WriteLine("hand X: " + handRightX);
            Debug.WriteLine("hand Y: " + handRightY);
            Debug.WriteLine("shoulder X: " + rightShoulderAnchor.X);
            Debug.WriteLine("shoulder Y: " + rightShoulderAnchor.Y);
            */
        }

        private double calculateRadius(double handRightX, double handRightY, double rightShoulderX, double rightShoulderY)
        {
            double xSquared = (handRightX - rightShoulderX) * (handRightX - rightShoulderX);
            double ySquared = (handRightY - rightShoulderY) * (handRightY - rightShoulderY);
            return Math.Sqrt(xSquared + ySquared);
        }

        private void checkForSwipeLeft(double thumbRightY, double handRightY)
        {
            if (thumbRightY > handRightY)
                return;

            if (handMotionData.GetAverageVelocityXLast10Frames() < -5 && handMotionData.GetAverageVelocityYLast10Frames() > -2 && handMotionData.GetAverageVelocityYLast10Frames() < 2)
            {
                gesture.AddActiveGesture(0);
            }
                

        }

        private void checkIfSwipeLeftHasEnded(double thumbRightY, double handRightY)
        {
            
            if (thumbRightY > handRightY || handMotionData.GetAverageVelocityXLast10Frames() > -2 || handMotionData.GetAverageVelocityYLast10Frames() < -2 || handMotionData.GetAverageVelocityYLast10Frames() > 2)
                gesture.KillActiveGesture(0);
        }

        private void checkForSwipeRight(double thumbRightY, double handRightY)
        {
            
            if (thumbRightY < handRightY)
                return;

            if (handMotionData.GetAverageVelocityXLast10Frames() > 4 && handMotionData.GetAverageVelocityYLast10Frames() > -2 && handMotionData.GetAverageVelocityYLast10Frames() < 2)
                gesture.AddActiveGesture(1);

        }

        private void checkIfSwipeRightHasEnded(double thumbRightY, double handRightY)
        {
            if (thumbRightY < handRightY || handMotionData.GetAverageVelocityXLast10Frames() < 2 || handMotionData.GetAverageVelocityYLast10Frames() < -2 || handMotionData.GetAverageVelocityYLast10Frames() > 2)
                gesture.KillActiveGesture(1);
        }




        private void updateGestureState(double rightHandX, double rightHandY, double rightHandZ, double spineMidY, double spineMidZ, double rightShoulderX, double rightShoulderY)
        {

            double differenceInZ = spineMidZ - rightHandZ;
            // Debug.WriteLine("Velocity " + velocityZ);
            // Debug.WriteLine("lastRightHandZ " + lastRightHandZ);
            updateInstantVelocity(rightHandX, rightHandY, rightHandZ);
            
            if ((differenceInZ > 0.2 && instantVelocityZ < 0.03 && instantVelocityZ > -0.03) || handMotionData.GetAverageVelocityMagnitude() > 2)
            {
                gesture.SetGesture(true);
                gesture.resetCircle();
            }
                
            if(velocityMagnitude< 1 && velocityMagnitude > -1 && rightHandX < rightShoulderX + 20 && rightHandX > rightShoulderX  && rightHandY > spineMidY)
            {
                gesture.SetGesture(false);
                gesture.SetRotationFlag(true);
            }
                

            lastRightHandX = rightHandX;
            lastRightHandY = rightHandY;
            lastRightHandZ = rightHandZ;
        }

        private void updateInstantVelocity(double rightHandX, double rightHandY, double rightHandZ)
        {
            if (lastRightHandZ != 0 || lastRightHandX != 0 || lastRightHandY != 0)
            {
                instantVelocityX = rightHandX - lastRightHandX;
                instantVelocityY = rightHandY - lastRightHandY;
                instantVelocityZ = rightHandZ - lastRightHandZ;
            }
            velocityMagnitude = Math.Sqrt(instantVelocityX*instantVelocityX + instantVelocityY*instantVelocityY + instantVelocityZ*instantVelocityZ);
        }

        private double distanceBetweenTwoPoints(double v1, double v2, double x, double y)
        {
            v1 = Math.Pow(x - v1, 2);
            v2 = Math.Pow(y - v2, 2);
            return Math.Sqrt(v1 + v2 );
        }

        private void setNewAnchorPoints(float i, double x, double y)
        {
            switch (i)
            {
                case 0:
                    leftHandAnchor.SetX(x);
                    leftHandAnchor.SetY(y);
                    break;
                case 1:
                    rightHandAnchor.X = x;
                    rightHandAnchor.Y = y;
                    break;
                case 2:
                    rightShoulderAnchor.X = x;
                    rightShoulderAnchor.Y = y;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }
        private void drawShoulderAnchor(Point position, DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(this.ShoulderClosedBrush, null, position, 10, 10);
        }

        //print On Screen
        private void printOnScreen(Point position, DrawingContext drawingContext, String text)
        {
            FormattedText formattedText = new FormattedText(
        text,
        CultureInfo.GetCultureInfo("en-us"),
        FlowDirection.LeftToRight,
        new Typeface("Verdana"),
        15,
        Brushes.White);
            drawingContext.DrawText(formattedText, position);
        }

            /// <summary>
            /// Draws indicators to show which edges are clipping body data
            /// </summary>
            /// <param name="body">body to draw clipping information for</param>
            /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }




        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }
    }
}
