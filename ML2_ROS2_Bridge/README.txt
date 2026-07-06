This folder contains everything needed to interface the Magic Leap 2 headset with a ROS2 system.

Specifically, there are two approaches:
1) If you need the Magic Leap 2 to send camera and IMU data to ROS2, the application is already ready.
You simply need to follow the instructions in the `ROS2_development` folder to set up the ROS2-side interface and install the application provided in the `ML2_development` folder onto the headset.
2) If, however, you require additional features or need the headset to perform tasks other than just sending data, you can start with the Magic Leap 2 Unity project, using the scripts found in the `ML2_development` folder and following the instructions in the README located within that same folder.
Specifically, the provided code handles the functionality of the application we developed, serving as a starting point for you to add the features you need.
Before proceeding with this approach, however, you must still follow the instructions in the `ROS2_development` folder to set up the ROS2-side interface.