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
    private string topicNameGyro;
    private string topicNameAcc;
    private string topicNameLightSensor;

    long old_gyro_time = 0;
    long old_accel_time = 0;
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
            topicNameGyro = topic_base_name + "/gyro/imu";
            topicNameAcc = topic_base_name + "/accel/imu"; 
            topicNameLightSensor = topic_base_name + "/light_sensor";

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<ImuMsg>(topicNameGyro);
            ros.RegisterPublisher<ImuMsg>(topicNameAcc);
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
            

            // Accelerometer
            float[] accel = pluginInstance.Call<float[]>("getAccelValues");
            long accelTime = pluginInstance.Call<long>("getAccelTimestamp");

            if (old_accel_time != accelTime)
            {
                old_accel_time = accelTime;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(accelTime, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg accMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_accel",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg { x = 0, y = 0, z = 0},
                    linear_acceleration = new Vector3Msg
                    {
                        x = accel[1],
                        y = -accel[0],
                        z = accel[2]
                    },
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameAcc, accMsg);
            }

            // Gyro
            float[] gyro = pluginInstance.Call<float[]>("getGyroValues");
            long gyroTime = pluginInstance.Call<long>("getGyroTimestamp");

            if (old_gyro_time != gyroTime)
            {   
                old_gyro_time = gyroTime;
                MLResult result = MLTime.ConvertMLTimeToSystemTime(gyroTime, out long converted_time); // API are wrong; this method does exactly the opposite.
                long frameUnixNano = _bootTimeUnixNano + converted_time;

                sec = (int)(frameUnixNano / 1_000_000_000);
                nsec = (uint)(frameUnixNano % 1_000_000_000);

                ImuMsg gyroMsg = new ImuMsg
                {
                    header = new RosMessageTypes.Std.HeaderMsg
                    {
                        frame_id = "magicleap_gyro",
                        stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nsec)
                    },
                    orientation = new QuaternionMsg(),
                    angular_velocity = new Vector3Msg
                    {
                        x = gyro[1],
                        y = -gyro[0],
                        z = gyro[2]
                    },
                    linear_acceleration = new Vector3Msg { x = 0, y = 0, z = 0},
                    orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                    angular_velocity_covariance = new double[9],
                    linear_acceleration_covariance = new double[9]
                };
                ros.Publish(topicNameGyro, gyroMsg);
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
