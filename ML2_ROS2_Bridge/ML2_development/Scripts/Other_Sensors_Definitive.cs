using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.MagicLeap;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class Other_Sensors_Definitive : MonoBehaviour
{
    [HideInInspector]
    public bool turn_on = false;

    [HideInInspector]
    public string topic_base_name = "/magic_leap2";

    StringMsg msg;

    private bool work = false;

    private ROSConnection ros;
    private string topicNameGyroRight;
    private string topicNameGyroLeft;
    private string topicNameGyroCompute;
    private string topicNameAccelRight;
    private string topicNameAccelLeft;
    private string topicNameAccelCompute;
    private string topicNameLightSensor;

    long old_gyro_right_time = 0;
    long old_gyro_left_time = 0;
    long old_gyro_compute_time = 0;
    long old_accel_right_time = 0;
    long old_accel_left_time = 0;
    long old_accel_compute_time = 0;
    long old_light_time = 0;

    private AndroidJavaObject pluginInstance = null;

    [HideInInspector]
    public long _bootTimeUnixNano;

    void Start()
    {
        Time.fixedDeltaTime = 0.01f; // 100 Hz

        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                pluginInstance = new AndroidJavaObject(
                    "com.federicocompagno_alessandrorubert.ml2_ros2_bridge.sensorlib.SensorManagerPlugin",
                    currentActivity
                );
            }
            Debug.Log("Plugin SensorManager loaded succesfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during the initialization of the plugin: {e.Message}");
        }
    }

    void Update()
    {
    }
    
    void FixedUpdate()
    {
        if (turn_on)
        {
            topicNameGyroRight = topic_base_name + "/gyro_right/imu";
            topicNameGyroLeft = topic_base_name + "/gyro_left/imu";
            topicNameGyroCompute = topic_base_name + "/gyro_compute_pack/imu";
            topicNameAccelRight = topic_base_name + "/accel_right/imu";
            topicNameAccelLeft = topic_base_name + "/accel_left/imu";
            topicNameAccelCompute = topic_base_name + "/accel_compute_pack/imu";
            topicNameLightSensor = topic_base_name + "/light_sensor";

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<ImuMsg>(topicNameGyroRight);
            ros.RegisterPublisher<ImuMsg>(topicNameGyroLeft);
            ros.RegisterPublisher<ImuMsg>(topicNameGyroCompute);
            ros.RegisterPublisher<ImuMsg>(topicNameAccelRight);
            ros.RegisterPublisher<ImuMsg>(topicNameAccelLeft);
            ros.RegisterPublisher<ImuMsg>(topicNameAccelCompute);
            ros.RegisterPublisher<IlluminanceMsg>(topicNameLightSensor);
            ros.RegisterPublisher<StringMsg>(topic_base_name + "/log");
            

            work = true;
            turn_on = false;
        }

        int sec = 0;
        uint nsec = 0;

        if (work)
        {
            
            if (pluginInstance == null)
            {
                return;
            }
            
            // Gyro Right
            float[] gyro_right = pluginInstance.Call<float[]>("getGyroRightValues");
            long gyroTime_right = pluginInstance.Call<long>("getGyroRightTimestamp");

            if (old_gyro_right_time != gyroTime_right)
            {   
                old_gyro_right_time = gyroTime_right;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(gyroTime_right, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg gyroMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_gyro_right",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg
                    {
                        x = gyro_right[1],
                        y = -gyro_right[0],
                        z = gyro_right[2]
                    },
                    linear_acceleration = new Vector3Msg { x = 0, y = 0, z = 0},
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameGyroRight, gyroMsg);
            }

            // Gyro Left
            float[] gyro_left = pluginInstance.Call<float[]>("getGyroLeftValues");
            long gyroTime_left = pluginInstance.Call<long>("getGyroLeftTimestamp");

            if (old_gyro_left_time != gyroTime_left)
            {   
                old_gyro_left_time = gyroTime_left;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(gyroTime_left, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg gyroMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_gyro_left",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg
                    {
                        x = gyro_left[1],
                        y = -gyro_left[0],
                        z = gyro_left[2]
                    },
                    linear_acceleration = new Vector3Msg { x = 0, y = 0, z = 0},
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameGyroLeft, gyroMsg);
            }

            // Gyro Compute Pack
            float[] gyro_compute = pluginInstance.Call<float[]>("getGyroComputeValues");
            long gyroTime_compute = pluginInstance.Call<long>("getGyroComputeTimestamp");

            if (old_gyro_compute_time != gyroTime_compute)
            {   
                old_gyro_compute_time = gyroTime_compute;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(gyroTime_compute, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg gyroMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_gyro_compute",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg
                    {
                        x = gyro_compute[1],
                        y = -gyro_compute[0],
                        z = gyro_compute[2]
                    },
                    linear_acceleration = new Vector3Msg { x = 0, y = 0, z = 0},
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameGyroCompute, gyroMsg);
            }

            // Accelerometer Right
            float[] accel_right = pluginInstance.Call<float[]>("getAccelRightValues");
            long accelTime_right = pluginInstance.Call<long>("getAccelRightTimestamp");

            if (old_accel_right_time != accelTime_right)
            {
                old_accel_right_time = accelTime_right;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(accelTime_right, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg accMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_accel_right",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg { x = 0, y = 0, z = 0},
                    linear_acceleration = new Vector3Msg
                    {
                        x = accel_right[1],
                        y = -accel_right[0],
                        z = accel_right[2]
                    },
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameAccelRight, accMsg);
            }

            // Accelerometer Left
            float[] accel_left = pluginInstance.Call<float[]>("getAccelLeftValues");
            long accelTime_left = pluginInstance.Call<long>("getAccelLeftTimestamp");

            if (old_accel_left_time != accelTime_left)
            {
                old_accel_left_time = accelTime_left;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(accelTime_left, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg accMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_accel_left",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg { x = 0, y = 0, z = 0},
                    linear_acceleration = new Vector3Msg
                    {
                        x = accel_left[1],
                        y = -accel_left[0],
                        z = accel_left[2]
                    },
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameAccelLeft, accMsg);
            }

            // Accelerometer Compute Pack
            float[] accel_compute = pluginInstance.Call<float[]>("getAccelComputeValues");
            long accelTime_compute = pluginInstance.Call<long>("getAccelComputeTimestamp");

            if (old_accel_compute_time != accelTime_compute)
            {
                old_accel_compute_time = accelTime_compute;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(accelTime_compute, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg accMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_accel_compute",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg { x = 0, y = 0, z = 0},
                    linear_acceleration = new Vector3Msg
                    {
                        x = accel_compute[1],
                        y = -accel_compute[0],
                        z = accel_compute[2]
                    },
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameAccelCompute, accMsg);
            }

            // light Sensor
            float light = pluginInstance.Call<float>("getLightValue");
            long lightTime = pluginInstance.Call<long>("getLightTimestamp");
            
            if (old_light_time != lightTime)
            {
                old_light_time = lightTime;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(lightTime, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                IlluminanceMsg illuminanceMsg= new IlluminanceMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magic_leap_light_sensor",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    illuminance = light,
                    variance = 0
                };

                ros.Publish(topicNameLightSensor, illuminanceMsg);
            }
        }
    
    }

    private void OnDisable()
    {
        if (pluginInstance != null)
        {
            pluginInstance.Call("stopListening");
        }
    }
}
