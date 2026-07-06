using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

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

    private bool work = true;

    private ROSConnection ros;
    private string topicNameGyro;
    private string topicNameAcc;
    private string topicNameLightSensor;

    void Start()
    {
        
    }

    void Update()
    {
        if (turn_on)
        {
            topicNameGyro = topic_base_name + "/gyro/imu";
            topicNameAcc = topic_base_name + "/accel/imu"; 
            topicNameLightSensor = topic_base_name + "/light_sensor";

            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);
            InputSystem.EnableDevice(UnityEngine.InputSystem.LightSensor.current);

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<ImuMsg>(topicNameGyro);
            ros.RegisterPublisher<ImuMsg>(topicNameAcc);
            ros.RegisterPublisher<IlluminanceMsg>(topicNameLightSensor);
            ros.RegisterPublisher<StringMsg>(topic_base_name + "/log");

            if  (UnityEngine.InputSystem.Gyroscope.current.enabled)
            {
                msg = new StringMsg("gyro: Gyroscope  is  enabled.");
                ros.Publish(topic_base_name + "/log", msg);
            }
            if  (UnityEngine.InputSystem.Accelerometer.current.enabled)
            {
                msg = new StringMsg("accelerometer: Accelerometer is  enabled.");
                ros.Publish(topic_base_name + "/log", msg);
            }
            if  (UnityEngine.InputSystem.LightSensor.current.enabled)
            {
                msg = new StringMsg("light_sensor: LightSensor is  enabled.");
                ros.Publish(topic_base_name + "/log", msg);
            }

            work = true;
            turn_on = false;
        }

        if (work)
        {
            Vector3 angularVelocity = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
            

            DateTime utcNow = DateTime.UtcNow;
            long unixTicks = utcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            uint sec = (uint)(unixTicks / TimeSpan.TicksPerSecond);
            uint nsec = (uint)((unixTicks % TimeSpan.TicksPerSecond) * 100);

            ImuMsg gyroMsg = new ImuMsg
            {
                header = new RosMessageTypes.Std.HeaderMsg
                {
                    frame_id = "magicleap_gyro",
                    stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg((int)sec, nsec)
                },
                orientation = new QuaternionMsg(),
                angular_velocity = new Vector3Msg
                {
                    x = angularVelocity.y,
                    y = angularVelocity.x,
                    z = angularVelocity.z
                },
                linear_acceleration = new Vector3Msg { x = 0, y = 0, z = 0},
                orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                angular_velocity_covariance = new double[9],
                linear_acceleration_covariance = new double[9]
            };
            ros.Publish(topicNameGyro, gyroMsg);


            Vector3 acceleration = UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue();

            utcNow = DateTime.UtcNow;
            unixTicks = utcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            sec = (uint)(unixTicks / TimeSpan.TicksPerSecond);
            nsec = (uint)((unixTicks % TimeSpan.TicksPerSecond) * 100);
            
            ImuMsg accMsg = new ImuMsg
            {
                header = new RosMessageTypes.Std.HeaderMsg
                {
                    frame_id = "magicleap_accel",
                    stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg((int)sec, nsec)
                },
                orientation = new QuaternionMsg(),
                angular_velocity = new Vector3Msg { x = 0, y = 0, z = 0},
                linear_acceleration = new Vector3Msg
                {
                    x = acceleration.y,
                    y = acceleration.x,
                    z = acceleration.z
                },
                orientation_covariance = new double[] {-1.0, 0, 0, 0, 0, 0, 0, 0, 0},
                angular_velocity_covariance = new double[9],
                linear_acceleration_covariance = new double[9]
            };
            ros.Publish(topicNameAcc, accMsg);


            float lightLevel = UnityEngine.InputSystem.LightSensor.current.lightLevel.ReadValue();

            utcNow = DateTime.UtcNow;
            unixTicks = utcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            sec = (uint)(unixTicks / TimeSpan.TicksPerSecond);
            nsec = (uint)((unixTicks % TimeSpan.TicksPerSecond) * 100);

            IlluminanceMsg illuminanceMsg= new IlluminanceMsg
            {
                header = new RosMessageTypes.Std.HeaderMsg
                {
                    frame_id = "magic_leap_light_sensor",
                    stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg((int)sec, nsec)
                },
                illuminance = lightLevel,
                variance = 0
            };

            ros.Publish(topicNameLightSensor, illuminanceMsg);
        }
    }

    private void OnDisable()
    {
        InputSystem.DisableDevice(UnityEngine.InputSystem.Gyroscope.current);
        msg = new StringMsg("gyro: Gyroscope  is  disabled.");
        ros.Publish(topic_base_name + "/log", msg);

        InputSystem.DisableDevice(UnityEngine.InputSystem.Accelerometer.current);
        msg = new StringMsg("accelerometer: Accelerometer is  disabled.");
        ros.Publish(topic_base_name + "/log", msg);

        InputSystem.DisableDevice(UnityEngine.InputSystem.LightSensor.current);
        msg = new StringMsg("light_sensor: LightSensor is  disabled.");
        ros.Publish(topic_base_name + "/log", msg);
    }
}
