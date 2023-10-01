//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Newtonsoft.Json;

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

        private Point rightHandAnchor = new Point();
        private double rightHandAnchorZ= 0;

        private Point rightShoulderAnchor = new Point();

        private double shoulderReference = 0;

        private double anchorThreshold = 20;

        private Gesture gesture = new Gesture();

        private ColoredGestureImage rightHandColoredImage = new ColoredGestureImage("test", 0);

        private ColoredGestureImage leftHandColoredImage = new ColoredGestureImage("test", 0);


        private Point3D lastRightHand = new Point3D(0, 0, 0);

        double velocityMagnitude = 0;

        private RightHandMotionData rightHandMotionData = new RightHandMotionData();

        private LeftHandMotionData leftHandMotionData = new LeftHandMotionData();

        //0: rightHand
        //1: leftHand
        private Boolean chosenHand = false;

        private Point3D chosenHandLastPoint = new Point3D(0,0,0);

        private HandMotionData handMotionData = new HandMotionData();

        private Point[] velocityPoints = new Point[20];

        private string timestamp = DateTime.UtcNow.ToString("ddMMyyyy-HHmmss");

        private string pathToGestures;

        private string pathToSpecificGesture;

        private int gestureCounter = 1;

        private Boolean gestureCounterFlag = false;

        private LinkedList<GestureImage> gestureImages = new LinkedList<GestureImage>();

        private Point3D halfPointMidBase;

        private List<IReadOnlyDictionary<JointType, Joint>> rightHandJoints = new List<IReadOnlyDictionary<JointType, Joint>>();

        private List<Dictionary<JointType, Point>> rightHandJointPoints = new List<Dictionary<JointType, Point>>();

        private List<IReadOnlyDictionary<JointType, Joint>> leftHandJoints = new List<IReadOnlyDictionary<JointType, Joint>>();

        private List<Dictionary<JointType, Point>> leftHandJointPoints = new List<Dictionary<JointType, Point>>();



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

            //create the folder that contains gestures samples
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            Console.WriteLine(projectDirectory);
            pathToGestures = projectDirectory + "\\Gestures\\" + timestamp;
            Directory.CreateDirectory(pathToGestures);
            Directory.CreateDirectory(pathToGestures + "\\Human Images");
            Directory.CreateDirectory(pathToGestures + "\\CNN Images");



            // printing body joints

            List<IReadOnlyDictionary<JointType, Joint>> joints = WritingOnDisk.LoadFile<List<IReadOnlyDictionary<JointType, Joint>>>("C:\\Users\\Ziad\\Bachelor\\thesis-ziad\\Gestures\\18062022-121101\\1\\nucJoints");
            List<Dictionary<JointType, Point>> jointPoints = WritingOnDisk.LoadFile<List<Dictionary<JointType, Point>>>("C:\\Users\\Ziad\\Bachelor\\thesis-ziad\\Gestures\\18062022-121101\\1\\nucJointPoints");
            
            Console.WriteLine(joints.Count);
            for (int i = 0; i < joints.Count; i++)
            {
                foreach (KeyValuePair<JointType, Joint> entry in joints[i])
                {
                    Console.WriteLine(entry.Key.ToString() + ":  " + entry.Value.ToString());

                }
            }
            Console.WriteLine(jointPoints.Count);

            for (int i = 0; i < jointPoints.Count; i++)
            {
                foreach (KeyValuePair<JointType, Point> entry in jointPoints[i])
                {
                    Console.WriteLine(entry.Key.ToString() + ":U+0020" + entry.Value.ToString());

                }
            }

            /*
               List<Hashtable> n = WritingOnDisk.LoadFile<List<Hashtable>>("C:/Users/Ziad/Bachelor/thesis-ziad/Gestures/132949735972453966/4/nucleus");

               for (int i = 0; i < n.Count; i++)
               {
                   if(i == n.Count - 1)
                       foreach (string key in n[i].Keys)
                           System.Diagnostics.Debug.WriteLine(String.Format("{0}: {1}", key, n[i][key]));



                           foreach (string key in n[i].Keys)
                   {
                       try
                       {
                           System.Diagnostics.Debug.WriteLine(String.Format("{0}: {1}", key, ((Point3D)n[i][key]).ToString()));
                       }
                       catch (Exception e)
                       {

                       }

                   }
               }
            */

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

                    Hashtable pointsTable = new Hashtable();
                    int penIndex = 0;
                    Body chosenBody = null;
                    double chosenBodyZ = 99; ;
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

                                // setting the anchors 

                                Point3D jointCoordinates = new Point3D(depthSpacePoint.X, depthSpacePoint.Y, position.Z);

                                if(jointType.ToString() == "SpineMid")
                                {
                                    if(jointCoordinates.GetZ() < chosenBodyZ)
                                    {
                                        chosenBody = body;
                                    }
                                }
                            }
                           
                        }
                    }
                    penIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        if (chosenBody != body)
                        {
                            continue;
                        }

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

                                Point3D jointCoordinates = new Point3D(depthSpacePoint.X, depthSpacePoint.Y, position.Z);
                                try
                                {
                                    pointsTable.Add(jointType.ToString(), jointCoordinates);
                                }
                                catch (Exception ex)
                                {

                                }
                                if (jointType.ToString() == "HandRight")
                                {

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
                                    //converting to the depth space
                                    //Debug.WriteLine("position Right Shoulder " + position.Z);
                                    jointCoordinates.SetZ(position.Z * 1080 / 4);
                                }
                                if (jointType.ToString() == "ShoulderLeft" )
                                {
                                    jointCoordinates.SetZ(position.Z * 1080 / 4);
                                }
                            }

                            


                            double shoulderReferenceX = ((Point3D) pointsTable["ShoulderRight"]).GetX() - ((Point3D)pointsTable["ShoulderLeft"]).GetX();
                            double shoulderReferenceZ = ((Point3D)pointsTable["ShoulderRight"]).GetZ() - ((Point3D)pointsTable["ShoulderLeft"]).GetZ();
                            shoulderReference = Math.Sqrt(shoulderReferenceX*shoulderReferenceX + shoulderReferenceZ * shoulderReferenceZ);
                            try
                            {
                                pointsTable.Add("Reference", shoulderReference);
                            }
                            catch (Exception ex)
                            {

                            }

                            halfPointMidBase = new Point3D((((Point3D)pointsTable["SpineMid"]).GetX() + ((Point3D)pointsTable["SpineMid"]).GetX()) / 1.6, (((Point3D)pointsTable["SpineMid"]).GetY() + ((Point3D)pointsTable["SpineMid"]).GetY()) / 1.6, (((Point3D)pointsTable["SpineMid"]).GetZ() + ((Point3D)pointsTable["SpineMid"]).GetZ()) / 1.6);


                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                           // Debug.WriteLine("Right Hand: " + jointPoints[JointType.HandRight]);
                           // Debug.WriteLine("Right Shoulder: " + rightShoulderAnchor);
                    //        this.DrawShoulderAnchor(rightShoulderAnchor, dc);

                            rightHandMotionData.AddFrame(pointsTable);
                            leftHandMotionData.AddFrame(pointsTable);

                            rightHandJoints.Add(joints);
                            rightHandJointPoints.Add(jointPoints);

                            leftHandJoints.Add(joints);
                            leftHandJointPoints.Add(jointPoints);

                            
                            //  if(gesture.IsThereAGesture() == true)
                            //      checkGestureType(pointsTable);
                            UpdateRightHandState(pointsTable);
                            UpdateLeftHandState(pointsTable);



                            Print(dc, drawPen);

                            
                        }
                    }
       
                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }
        

        public void Print(DrawingContext dc, Pen drawPen)
        {
            Point point = new Point();
            String text = "Right hand state: " + gesture.GetRightHandState();
            PrintOnScreen(point, dc, text);

            point.Y = 20;
            text = "Left hand state: " + gesture.GetLeftHandState();
            PrintOnScreen(point, dc, text);

            point.Y = 40;
            text = "Swipe Right: " + gesture.GetGesturesDictonery()[1].ToString();
            //    PrintOnScreen(point, dc, text);

            PrintVelocityPoints(dc, drawPen);

            point.X = 340;
            point.Y = 0;

            text = "Last Made 3 Gestures: ";
            PrintOnScreen(point, dc, text);
            point.Y = 60;
            LinkedList<string>.Enumerator demoEnum = gesture.GetLastMadeGestures().GetEnumerator();
            while (demoEnum.MoveNext())
            {
                string res = demoEnum.Current;
                PrintOnScreen(point, dc, res);
                point.Y = point.Y - 20;
            }

            int[] totalNumberOfGestures = gesture.TotalNumberOfGestures;



            point.X = 430;
            point.Y = 80;

            point.Y = point.Y + 20;
            text = "SL: " + totalNumberOfGestures[0];
            PrintOnScreen(point, dc, text);

            point.Y = point.Y + 20;
            text = "SR: " + totalNumberOfGestures[1];
            PrintOnScreen(point, dc, text);

            point.Y = point.Y + 20;
            text = "Rot: " + totalNumberOfGestures[2];
            PrintOnScreen(point, dc, text);

            point.Y = point.Y + 20;
            text = "SU: " + totalNumberOfGestures[3];
            PrintOnScreen(point, dc, text);

            point.Y = point.Y + 20;
            text = "SD: " + totalNumberOfGestures[4];
            PrintOnScreen(point, dc, text);

            point.Y = point.Y + 20;
            text = "Rect: " + totalNumberOfGestures[5];
            PrintOnScreen(point, dc, text);

        }

        private void UpdateLeftHandState(Hashtable pointsTable)
        {

            double differenceInZ = Math.Abs(((Point3D)pointsTable["SpineMid"]).GetZ() - ((Point3D)pointsTable["HandLeft"]).GetZ());



            if (((leftHandMotionData.inActiveFor30Frames() && ((Point3D)pointsTable["HandLeft"]).GetZ() < ((Point3D)pointsTable["SpineMid"]).GetZ() + 0.8) || Gesture.IsBodyMovingWithTheHand(leftHandMotionData)))
            {
                if (gesture.GetLeftHandState().Equals(Gesture.State.Retraction))
                {
                    leftHandColoredImage.DrawLines(2, leftHandMotionData.GetPoints(), null, 0);

                    gestureCounterFlag = false;

                    SaveGestureToDisk(pathToSpecificGesture, 0, 2);

                    gestureCounter++;
                    leftHandMotionData.FlushData();
                }
                gesture.SetLeftHandNoGesture();
            }
            else
            {
                switch (gesture.GetLeftHandState())
                {
                    case Gesture.State.NoGesture:
                        if (gestureCounterFlag == true && leftHandColoredImage.NucleusLinkedList.Count == 0)
                        {
                            leftHandColoredImage.DrawLines(2, leftHandMotionData.GetPoints(), null, 0);

                            gestureCounterFlag = false;

                            SaveGestureToDisk(pathToSpecificGesture, 0, 2);

                            gestureCounter++;
                            leftHandMotionData.FlushData();
                            //                    File.WriteAllText(pathToGestures + "/json", gesture.GetLastMadeGesture());
                            /*
                            if(PrepareToSaveImages())
                            {
                                while(gestureImages.Count > 0)
                                {
                                    gestureImages.First.Value.SaveImage();
                                    gestureImages.RemoveFirst();
                                }
                            }
                            */

                        }

                        Console.WriteLine("instant velocity magnitude before preparation: " + leftHandMotionData.GetInstantVelocityMagnitude());
                        Console.WriteLine("hand left y : " + ((Point3D)pointsTable["HandLeft"]).GetY());
                        Console.WriteLine("spine mid y: " + halfPointMidBase.GetY());
                        if (((Point3D)pointsTable["HandLeft"]).GetY() < halfPointMidBase.GetY() && ((((Point3D)pointsTable["HandLeft"]).GetZ() + 0.1 < ((Point3D)pointsTable["SpineMid"]).GetZ() || ((Point3D)pointsTable["HandLeft"]).GetX() + 100 < ((Point3D)pointsTable["SpineMid"]).GetX() + 100) && leftHandMotionData.GetInstantVelocity().GetZ() < 0 && leftHandMotionData.GetInstantVelocityMagnitude() > 3))
                        {
                            pathToSpecificGesture = pathToGestures + "\\" + gestureCounter;
                            PrepareEnvironmentToSaveGesture(1);
                        }
                        break;
                    case Gesture.State.Preperation:

                        Console.WriteLine("Acceleration: " + leftHandMotionData.GetInstantAccelerationMagnitude());
                        //      Console.WriteLine("Gesture Image: " + this.gestureImage);


                        if (gesture.gestureNucleusIsReady(pointsTable, leftHandMotionData))
                        {
                            leftHandColoredImage.DrawLines(0, leftHandMotionData.GetPoints(), null, 0);
                            SaveGestureToDisk(pathToSpecificGesture, 0, 0);

                            Console.WriteLine("flusshhhhh");
                            leftHandMotionData.FlushData();
                            leftHandMotionData.AddFrame(pointsTable);
                            gesture.SetStateToNucleus(leftHandMotionData.GetPoints().First, 1);

                        }

                        break;
                    case Gesture.State.Nucleus:


                        //I should begin to detect the gesture
                        gesture.CheckGestureType(pointsTable, leftHandMotionData, pathToSpecificGesture);
                        //detectGesture()

                        if (gesture.gestureNucleusEnded(pointsTable, leftHandMotionData))
                        {

                            int closedShape = 0;
                            if (gesture.MadeGesturesWithinNucleus.Contains("Rotation"))
                            {
                                Console.WriteLine("yes containts rotation");
                                closedShape = 1;
                            }
                            if (gesture.MadeGesturesWithinNucleus.Contains("Rectangle"))
                            {
                                Console.WriteLine("yes containts rectangle");
                                closedShape = 2;
                            }


                            leftHandColoredImage.DrawLines(1, leftHandMotionData.GetPoints(), leftHandMotionData.GetAverageLeftHand(), closedShape);

                            Console.WriteLine("preperation linkedlist count: " + leftHandColoredImage.PreperationLinkedList.Count);
                            Console.WriteLine("nucleus linkedlist count: " + leftHandColoredImage.NucleusLinkedList.Count);
                            Console.WriteLine("retraction linkedlist count: " + leftHandColoredImage.RetractionLinkedList.Count);

                            gesture.SetStateToRetraction(leftHandColoredImage.NucleusLinkedList, 1);

                            //         List<Hashtable> motionData = handMotionData.GetMotionData();
                            //         Hashtable gestureType = new Hashtable();
                            //          gestureType.Add("type", gesture.GetLastMadeGesture());
                            //         motionData.Add(gestureType);
                            SaveGestureToDisk(pathToSpecificGesture, 0, 1);


                            leftHandMotionData.FlushData();

                        }
                        break;

                    case Gesture.State.Retraction:
                        Console.WriteLine("z instant velocity: " + leftHandMotionData.GetInstantVelocity().GetZ());
                        Console.WriteLine("hand left y : " + ((Point3D)pointsTable["HandLeft"]).GetY());
                        Console.WriteLine("spine Mid y: " + halfPointMidBase.GetY());
                        if (((Point3D)pointsTable["HandLeft"]).GetY() < halfPointMidBase.GetY() && ((((Point3D)pointsTable["HandLeft"]).GetZ() + 0.1 < ((Point3D)pointsTable["SpineMid"]).GetZ() || ((Point3D)pointsTable["HandLeft"]).GetX() + 100 < ((Point3D)pointsTable["SpineMid"]).GetX() + 100) && leftHandMotionData.GetInstantVelocity().GetZ() < 0 && leftHandMotionData.GetInstantVelocityMagnitude() > 3))
                        {
                            Console.WriteLine("retraaaaaaaaction");
                            Console.WriteLine("gesture: " + gesture.GetLastMadeGestures().Count);

                            leftHandColoredImage.DrawLines(2, leftHandMotionData.GetPoints(), null, 0);

                            gestureCounterFlag = false;
                            WritingOnDisk.WriteToFile(pathToGestures + "\\data.txt", gesture.GetLastMadeGesture());

                            SaveGestureToDisk(pathToSpecificGesture, 0, 2);


                            gestureCounter++;
                            leftHandMotionData.FlushData();
                            //                    File.WriteAllText(pathToGestures + "/json", gesture.GetLastMadeGesture());


                            pathToSpecificGesture = pathToGestures + "\\" + gestureCounter;
                            PrepareEnvironmentToSaveGesture(1);

                        }
                        break;

                    default:
                        break;
                }

            }
        }

        private void SaveGestureToDisk(string pathToSpecificGesture, int handType, int phase)
        {
            switch(handType)
            {
                case 0:
                    switch(phase)
                    {
                        case 0:
                //            WritingOnDisk.WriteFile(leftHandJoints, pathToSpecificGesture + "/prepJoints");
             //               WritingOnDisk.WriteFile(leftHandJointPoints, pathToSpecificGesture + "/prepJointPoints");
                            break;
                        case 1:
               //             WritingOnDisk.WriteFile(leftHandJoints, pathToSpecificGesture + "/nucJoints");
             //               WritingOnDisk.WriteFile(leftHandJointPoints, pathToSpecificGesture + "/nucJointPoints");
                            break;
                        case 2:
              //              WritingOnDisk.WriteFile(leftHandJoints, pathToSpecificGesture + "/retJoints");
               //             WritingOnDisk.WriteFile(leftHandJointPoints, pathToSpecificGesture + "/retJointPoints");
                            break;
                    }
                    leftHandJoints.Clear();
                    leftHandJointPoints.Clear();
                    break;
                case 1:
                    switch (phase)
                    {
                        case 0:
               //             WritingOnDisk.WriteFile(rightHandJoints, pathToSpecificGesture + "/prepJoints");
            //                WritingOnDisk.WriteFile(rightHandJointPoints, pathToSpecificGesture + "/prepJointPoints");
                            break;
                        case 1:
           //                 WritingOnDisk.WriteFile(rightHandJoints, pathToSpecificGesture + "/nucJoints");
            //                WritingOnDisk.WriteFile(rightHandJointPoints, pathToSpecificGesture + "/nucJointPoints");
                            break;
                        case 2:
           //                 WritingOnDisk.WriteFile(rightHandJoints, pathToSpecificGesture + "/retJoints");
            //                WritingOnDisk.WriteFile(rightHandJointPoints, pathToSpecificGesture + "/retJointPoints");
                            break;
                    }
                    rightHandJoints.Clear();
                    rightHandJointPoints.Clear();
                    break;
            }
        }

        private async Task UpdateRightHandState(Hashtable pointsTable)
        {

  

            double differenceInZ = Math.Abs(((Point3D)pointsTable["SpineMid"]).GetZ() - ((Point3D)pointsTable["HandRight"]).GetZ());


  
            if (((rightHandMotionData.inActiveFor30Frames() && ((Point3D)pointsTable["HandRight"]).GetZ() < ((Point3D)pointsTable["SpineMid"]).GetZ() + 0.8 )|| Gesture.IsBodyMovingWithTheHand(rightHandMotionData)))
            {
                if(gesture.GetRightHandState().Equals(Gesture.State.Retraction))
                {
                    rightHandColoredImage.DrawLines(2, rightHandMotionData.GetPoints(), null, 0);

                    gestureCounterFlag = false;
                    SaveGestureToDisk(pathToSpecificGesture, 1, 2);


                    gestureCounter++;
                    rightHandMotionData.FlushData();
                }
                gesture.SetRightHandNoGesture();
            }
            else
            {
                switch (gesture.GetRightHandState())
                {
                    case Gesture.State.NoGesture:
                        if (gestureCounterFlag == true && rightHandColoredImage.NucleusLinkedList.Count == 0)
                        {
                            rightHandColoredImage.DrawLines(2, rightHandMotionData.GetPoints(), null, 0);

                            gestureCounterFlag = false;
                            SaveGestureToDisk(pathToSpecificGesture, 1, 2);

                            gestureCounter++;
                            rightHandMotionData.FlushData();
                            //                    File.WriteAllText(pathToGestures + "/json", gesture.GetLastMadeGesture());
                            /*
                            if(PrepareToSaveImages())
                            {
                                while(gestureImages.Count > 0)
                                {
                                    gestureImages.First.Value.SaveImage();
                                    gestureImages.RemoveFirst();
                                }
                            }
                            */
                            
                        }

                        Console.WriteLine("instant velocity magnitude before preparation: " + rightHandMotionData.GetInstantVelocityMagnitude());
                        Console.WriteLine("hand Right y : " + ((Point3D)pointsTable["HandRight"]).GetY());
                        Console.WriteLine("spine mid y: " + halfPointMidBase.GetY());
                        if (((Point3D)pointsTable["HandRight"]).GetY() < halfPointMidBase.GetY() &&  !rightHandMotionData.inActiveFor30Frames() && rightHandMotionData.GetPoints().Count > 1 &&( ((Point3D)pointsTable["HandRight"]).GetZ() + 0.15 < ((Point3D)pointsTable["SpineMid"]).GetZ() || ((Point3D)pointsTable["HandRight"]).GetX()  > ((Point3D)pointsTable["SpineMid"]).GetX() + 100) && rightHandMotionData.GetInstantVelocityMagnitude() > 4) 
                        {
                            pathToSpecificGesture = pathToGestures + "\\" + gestureCounter;
                            PrepareEnvironmentToSaveGesture(0);
                        }
                        break;
                    case Gesture.State.Preperation:

                        Console.WriteLine("Acceleration: " + rightHandMotionData.GetInstantAccelerationMagnitude());
                  //      Console.WriteLine("Gesture Image: " + this.gestureImage);


                        if (gesture.gestureNucleusIsReady(pointsTable, rightHandMotionData))
                        {
                            rightHandColoredImage.DrawLines(0, rightHandMotionData.GetPoints(), null, 0);
                            SaveGestureToDisk(pathToSpecificGesture, 1, 0);

                            Console.WriteLine("flusshhhhh");
                            rightHandMotionData.FlushData();
                            rightHandMotionData.AddFrame(pointsTable);
                            gesture.SetStateToNucleus(rightHandMotionData.GetPoints().First, 0);

                        }

                        break;
                    case Gesture.State.Nucleus:

                        
                        //I should begin to detect the gesture
                        gesture.CheckGestureType(pointsTable, rightHandMotionData, pathToSpecificGesture);
                        //detectGesture()
                        
                        if (gesture.gestureNucleusEnded(pointsTable, rightHandMotionData))
                        {

                            int closedShape = 0;
                            if (gesture.MadeGesturesWithinNucleus.Contains("Rotation"))
                            {
                                Console.WriteLine("yes containts rotation");
                                closedShape = 1;
                            }
                            if (gesture.MadeGesturesWithinNucleus.Contains("Rectangle"))
                            {
                                Console.WriteLine("yes containts rectangle");
                                closedShape = 2;
                            }


                            rightHandColoredImage.DrawLines(1, rightHandMotionData.GetPoints(), rightHandMotionData.GetAverageRightHand(), closedShape);

                            Console.WriteLine("preperation linkedlist count: " + rightHandColoredImage.PreperationLinkedList.Count);
                            Console.WriteLine("nucleus linkedlist count: " + rightHandColoredImage.NucleusLinkedList.Count);
                            Console.WriteLine("retraction linkedlist count: " + rightHandColoredImage.RetractionLinkedList.Count);

                            gesture.SetStateToRetraction(rightHandColoredImage.NucleusLinkedList, 0);

                            //         List<Hashtable> motionData = handMotionData.GetMotionData();
                            //         Hashtable gestureType = new Hashtable();
                            //          gestureType.Add("type", gesture.GetLastMadeGesture());
                            //         motionData.Add(gestureType);

                            SaveGestureToDisk(pathToSpecificGesture, 1, 1);


                            //                        WritingOnDisk.WriteFile(motionData, pathToSpecificGesture + "/nucleus");
                            rightHandMotionData.FlushData();

                        }
                        break;

                    case Gesture.State.Retraction:
                        Console.WriteLine("z instant velocity: " + rightHandMotionData.GetInstantVelocity().GetZ());
                        Console.WriteLine("z instant velocity: " + rightHandMotionData.GetInstantVelocityMagnitude());
                        Console.WriteLine(((Point3D)pointsTable["HandRight"]).GetZ() < ((Point3D)pointsTable["SpineMid"]).GetZ());
                        Console.WriteLine("hand Right y : " + ((Point3D)pointsTable["HandRight"]).GetY());
                        Console.WriteLine("spine mid y: " + halfPointMidBase.GetY());
                        if (((Point3D)pointsTable["HandRight"]).GetY() < halfPointMidBase.GetY() &&  ((((Point3D)pointsTable["HandRight"]).GetZ() + 0.1 < ((Point3D)pointsTable["SpineMid"]).GetZ() || ((Point3D)pointsTable["HandRight"]).GetX() > ((Point3D)pointsTable["SpineMid"]).GetX() + 100) && rightHandMotionData.GetInstantVelocity().GetZ() < 0015 && rightHandMotionData.GetInstantVelocityMagnitude() > 5) )
                        {
                            Console.WriteLine("retraaaaaaaaction");
                            Console.WriteLine("gesture: " + gesture.GetLastMadeGestures().Count);

                            rightHandColoredImage.DrawLines(2, rightHandMotionData.GetPoints(), null, 0);

                            gestureCounterFlag = false;
                            SaveGestureToDisk(pathToSpecificGesture, 1, 2);


                            //                  WritingOnDisk.WriteFile(handMotionData.GetMotionData(), pathToSpecificGesture + "/retraction");
                            gestureCounter++;
                            rightHandMotionData.FlushData();
                            //                    File.WriteAllText(pathToGestures + "/json", gesture.GetLastMadeGesture());

                            WritingOnDisk.WriteToFile(pathToGestures + "\\data.txt", gesture.GetLastMadeGesture());

                            pathToSpecificGesture = pathToGestures + "\\" + gestureCounter;
                            PrepareEnvironmentToSaveGesture(0);

                        }
                        break;

                    default:
                        break;
                }

            }

            /*
            if ((differenceInZ > 0.2 && handMotionData.GetInstantVelocity().GetZ() < 0.03 && handMotionData.GetInstantVelocity().GetZ() > -0.03) || handMotionData.GetInstantVelocityMagnitude() > 2)
            {
                if(gesture.GetGesture() == false)
                {
                    gesture.resetCircle();
                }
                gesture.SetGesture(true);
            }
            
            if (handMotionData.GetInstantVelocityMagnitude() < 1.5 && handMotionData.GetInstantVelocityMagnitude() > -1.5 && ((Point3D)pointsTable["HandRight"]).GetX() < ((Point3D)pointsTable["ShoulderRight"]).GetX() + 40 && ((Point3D)pointsTable["HandRight"]).GetX() > ((Point3D)pointsTable["ShoulderRight"]).GetX() && ((Point3D)pointsTable["HandRight"]).GetY() > ((Point3D)pointsTable["SpineMid"]).GetY())
            {
                gesture.SetGesture(false);
                gesture.SetRotationFlag(true);
            }

            */

            lastRightHand.SetNewPoints(((Point3D)pointsTable["HandRight"]).GetX(), ((Point3D)pointsTable["HandRight"]).GetY(), ((Point3D)pointsTable["HandRight"]).GetZ());
        }

        private Boolean PrepareToSaveImages()
        {
            gestureImages.AddLast(rightHandColoredImage);
            if (gestureImages.Count >= 10)
                return true;
            return false;
        }

        private void PrepareEnvironmentToSaveGesture(int condition)
        {
            Directory.CreateDirectory(pathToGestures + "\\" + gestureCounter);
            switch(condition)
            {
                case 0:
                    this.rightHandColoredImage = gesture.SetStateToPreperation(rightHandMotionData, pathToGestures, gestureCounter);
                    Console.WriteLine("gesture image: " + this.rightHandColoredImage);
                    break;
                case 1:
                    this.leftHandColoredImage = gesture.SetStateToPreperation(leftHandMotionData, pathToGestures, gestureCounter);
                    Console.WriteLine("gesture image: " + this.leftHandColoredImage);
                    break;
            }


            
            gestureCounterFlag = true;

        }
        /*
        private void updateInstantVelocity(double rightHandX, double rightHandY, double rightHandZ)
        {
            if (lastRightHand.GetX() != 0 || lastRightHand.GetY() != 0 || lastRightHand.GetZ() != 0)
            {
                handMotionData.SetInstantVelocity(rightHandX - lastRightHand.GetX(), rightHandY - lastRightHand.GetY(), rightHandZ - lastRightHand.GetZ());
            }
        }
        */

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
        private void DrawShoulderAnchor(Point position, DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(this.ShoulderClosedBrush, null, position, 10, 10);
        }

        //print On Screen
        private void PrintOnScreen(Point position, DrawingContext drawingContext, String text)
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

        private void PrintVelocityPoints(DrawingContext drawingContext, Pen drawingPen)
        {
            foreach (VelocityLine velocityLine in rightHandMotionData.GetVelocityLines())
            {

                //Debug.WriteLine("Red in bytes : " + (byte)velocityLine.GetPoint1().GetRed());
               // Debug.WriteLine("blue in bytes  : " + (byte)velocityLine.GetPoint1().GetBlue());
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, (byte)((velocityLine.GetPoint1().GetRed()+ velocityLine.GetPoint2().GetRed())/2), 0 ,  (byte)((velocityLine.GetPoint1().GetBlue() + velocityLine.GetPoint2().GetBlue())/2)));
                Pen newPen = new Pen(brush, 6);
                drawingContext.DrawLine(newPen, velocityLine.GetPoint1().GetPosition(), velocityLine.GetPoint2().GetPosition());
            }
            foreach (VelocityLine velocityLine in leftHandMotionData.GetVelocityLines())
            {

                //Debug.WriteLine("Red in bytes : " + (byte)velocityLine.GetPoint1().GetRed());
                // Debug.WriteLine("blue in bytes  : " + (byte)velocityLine.GetPoint1().GetBlue());
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, (byte)((velocityLine.GetPoint1().GetRed() + velocityLine.GetPoint2().GetRed()) / 2), 0, (byte)((velocityLine.GetPoint1().GetBlue() + velocityLine.GetPoint2().GetBlue()) / 2)));
                Pen newPen = new Pen(brush, 6);
                drawingContext.DrawLine(newPen, velocityLine.GetPoint1().GetPosition(), velocityLine.GetPoint2().GetPosition());
            }
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
