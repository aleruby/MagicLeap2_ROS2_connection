This README explains how to configure the ROS2 side to make the bridge between ROS2 and Magic Leap 2 work.

Add the `config_ml2_stream` package and the package available for download at the following link to your workspace.

https://github.com/Unity-Technologies/ROS-TCP-Endpoint/releases/tag/ROS2v0.7.0

Once this is done, simply build it.

To configure the streams provided by the headset, you can modify the `config.yaml` file in the `config_ml2_stream` package and build it.

Next, you can launch the node that collects data from the headset and publishes it to topics using `ros2 launch ros_tcp_endpoint endpoint.py`, and the node that loads the settings from the `config.yaml` file onto the headset using `ros2 launch config_ml2_stream magic_leap_config.launch.py`.