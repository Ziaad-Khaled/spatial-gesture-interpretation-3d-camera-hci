# Gesture Segmentation

## Introduction
Gesture Segmentation is a project designed to track and analyze hand gestures using Kinect V2. This README provides an overview of the project's key components and functionality.

## Body Skeleton Data
- The Kinect V2 can track up to six bodies simultaneously.
- Each body contains temporal correlation data with the kinematic skeleton.
- The skeleton comprises 24 joints, including handRight, handLeft, spineMid, shoulderLeft, and shoulderRight.
- Joint coordinates are provided in the depth camera 3D coordinate system.

![Body Skeleton Visualization](![image](https://github.com/Ziaad-Khaled/gestureDetectionUsingKinectV2/assets/77291238/0fdffaaf-d530-4f63-af7b-f9381d6c18f8)
)

## Gesture States
- The system operates in four states: No-Gesture, Preparation, Stroke, and Retraction.
- The normal flow involves raising a hand to perform a gesture, retracting it, and waiting before the next gesture.
- Transitions between states can be interrupted due to specific events.

![Gesture States and Transitions](![image](https://github.com/Ziaad-Khaled/gestureDetectionUsingKinectV2/assets/77291238/526b8946-85f3-45a2-bd70-dd5b4bd3b458)
)

## Segmentation - First Phase
- The default state is No-Gesture.
- The system continuously receives body skeleton data.
- The compiler checks if the difference between spineMid Z-position and handRight Z-position exceeds a threshold (θp).
- Upon exceeding the threshold, the system transitions to the Preparation state.

Algorithm 1 describes the transition from Preparation to Stroke based on changes in hand velocity and direction.

## Segmentation - Second Phase
- A second segmentation phase improves segmentation accuracy.
- This phase analyzes saved gesture data in the Stroke phase.
- It identifies the last swipe down as a Retraction, correcting segmentation.

## Saving Gestures as an Image
- To evaluate gesture segmentation and detection, each gesture is represented as a gesture image.
- These images visually depict the hand motion data, including Preparation (blue lines), Stroke (green lines), and Retraction (red lines).
- The pen size varies based on velocity magnitude.

![Example Gesture Images](![image](https://github.com/Ziaad-Khaled/gestureDetectionUsingKinectV2/assets/77291238/3c51223d-0911-4de7-8eec-71d9fec7dfb3)
)

## Demo 




https://github.com/Ziaad-Khaled/gestureDetectionUsingKinectV2/assets/77291238/bac44081-02ad-4892-865d-e5a9c505f9ef



This project is part of my bachelor's degree. It is designed to work with Kinect V2 for gesture tracking and segmentation.

Feel free to explore the code and documentation for a more in-depth understanding of the project.
