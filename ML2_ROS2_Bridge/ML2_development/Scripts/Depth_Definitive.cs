using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Text;
using System.Linq;
using System;

using UnityEngine.XR.OpenXR;
using MagicLeap.OpenXR.Features.PixelSensors;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;

public class Depth_Definitive : MonoBehaviour
{
    [HideInInspector]
    public bool turn_on = false;

    [HideInInspector]
    public string topic_base_name = "/magic_leap2";

    StringMsg msg;

    private bool pose_on = false;

    private MagicLeapPixelSensorFeature pixelSensorFeature;
    private PixelSensorId sensorType;
    private ROSConnection ros;
    private long precedent_time;
    private uint[] streamIndexes = new uint[1];
    private Dictionary<uint, PixelSensorMetaDataType[]> metaDataTypesByStream = new();
    private string sensor_Name = "Depth Center";

    private string topicName;
    private string topicNameIntrinsecs;
    private string topicNameMetaData;
    private string topicNamePose;



    public enum stream : uint { LongRange_stream0 = 0,  ShortRange_stream1 = 1};
    public stream stream_Index;
    public enum possible_resolution : uint { res544x480 = 0};
    public possible_resolution resolution;
    private int ResolutionWidth;
    private int ResolutionHeight;
    public enum possible_update_rate_stream0 : int { fps1 = 1, fps5 = 5};
    public possible_update_rate_stream0 UpdateRate_stream0;
    public enum possible_update_rate_stream1 : int { fps5 = 5, fps30 = 30, fps60 = 60};
    public possible_update_rate_stream1 UpdateRate_stream1;

    public enum possible_format : int {Depth32 = 0, DepthRaw = 1};
    public possible_format format;

    [HideInInspector]
    public long _bootTimeUnixNano;
    
    void Start()
    {
    }

    
    void Update()
    {
        if (turn_on)
        {
            if ((int) format == 0)
            {
                topicName = topic_base_name + "/depth/image_raw";
                topicNameIntrinsecs = topic_base_name + "/depth/camera_info";
                topicNameMetaData = topic_base_name + "/depth/metadata";
                topicNamePose = "/tf";
            }
            else if ((int) format == 1)
            {
                topicName = topic_base_name + "/infra/image_raw";
                topicNameIntrinsecs = topic_base_name + "/infra/camera_info";
                topicNameMetaData = topic_base_name + "/infra/metadata";
                topicNamePose = "/tf";
            }

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<ImageMsg>(topicName);
            ros.RegisterPublisher<CameraInfoMsg>(topicNameIntrinsecs);
            ros.RegisterPublisher<StringMsg>(topicNameMetaData);
            ros.RegisterPublisher<TFMessageMsg>(topicNamePose);
            ros.RegisterPublisher<StringMsg>(topic_base_name + "/log");

            streamIndexes[0] = (uint) stream_Index;

            if (resolution == 0)
            {
                ResolutionWidth = 544;
                ResolutionHeight = 480;
            }

            // Attempt to retrieve the MagicLeapPixelSensorFeature from the OpenXR settings
            pixelSensorFeature = OpenXRSettings.Instance.GetFeature<MagicLeapPixelSensorFeature>();

            // Check if the feature is available and enabled
            if (pixelSensorFeature == null || !pixelSensorFeature.enabled)
            {
                msg = new StringMsg("depth/infra: Magic Leap Pixel Sensor Feature is not available or not enabled.");
                ros.Publish(topic_base_name + "/log", msg);
                enabled = false;
                return;
            }

            // Get the list of supported sensors and log their details
            List<PixelSensorId> supportedSensors = pixelSensorFeature.GetSupportedSensors();
            
            sensorType = supportedSensors.Find(s => s.SensorName == sensor_Name);

            // Cretion
            bool wasCreated = pixelSensorFeature.CreatePixelSensor(sensorType);
            msg = new StringMsg($"depth/infra: Sensor creation was successful: {wasCreated}");
            ros.Publish(topic_base_name + "/log", msg);

            
            uint numberOfStreams = pixelSensorFeature.GetStreamCount(sensorType);
            for (uint streamIndex = 0; streamIndex < numberOfStreams; streamIndex++)
            {
                if (pixelSensorFeature.GetPixelSensorCapabilities(sensorType, streamIndex, out PixelSensorCapability[] capabilities))
                {
                    foreach (var pixelSensorCapability in capabilities)
                    {
                        if (pixelSensorFeature.QueryPixelSensorCapability(sensorType, pixelSensorCapability.CapabilityType, streamIndex, out var range) && range.IsValid)
                        {}
                    }
                }
                else
                {
                    msg = new StringMsg($"depth/infra: Failed to get capabilities for stream index {streamIndex}");
                    ros.Publish(topic_base_name + "/log", msg);
                }
            }

            // Settings
            if ((int) format == 0)
            {
                pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Format, (uint) PixelSensorFrameFormat.Depth32 , (uint) stream_Index);
            }
            else if ((int) format == 1)
            {
                pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Format, (uint) PixelSensorFrameFormat.DepthRaw , (uint) stream_Index);
            }
            

            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Resolution, new Vector2Int(ResolutionWidth, ResolutionHeight), (uint) stream_Index);

            if ((uint) stream_Index == 0)
            {
                pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.UpdateRate, (uint) UpdateRate_stream0, (uint) stream_Index);
            }
            else if ((uint) stream_Index == 1)
            {
                pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.UpdateRate, (uint) UpdateRate_stream1, (uint) stream_Index);
            }

            // Start stream
            StartCoroutine(StartSensorStream());

            pose_on = true;
            turn_on = false;
        }
        if (pose_on)
        {
            long systemTimeNs = System.Diagnostics.Stopwatch.GetTimestamp() * (1000000000L / System.Diagnostics.Stopwatch.Frequency);
            //MLResult result = MLTime.ConvertMLTimeToSystemTime(systemTimeNs, out long converted_time); // API are wrong; this method does exactly the opposite.
            long frameUnixNano = _bootTimeUnixNano + systemTimeNs;
            uint sec = (uint)(frameUnixNano / 1_000_000_000);
            uint nsec = (uint)(frameUnixNano % 1_000_000_000);
            Pose sensorPose = pixelSensorFeature.GetSensorPose(sensorType);
            SendPoseROS(sensorPose, (int)sec, nsec);
        }
    }
    private IEnumerator StartSensorStream()
    {
        // Submit congiguration
        var pixelSensorOperation = pixelSensorFeature.ConfigureSensor(sensorType, streamIndexes);
        yield return pixelSensorOperation;
        if (pixelSensorOperation.DidOperationSucceed)
        {
            msg = new StringMsg("depth/infra: Sensor configuration successful.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("depth/infra: Sensor configuration failed.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }
        msg = new StringMsg("depth/infra: Sensor configured successfully.");
        ros.Publish(topic_base_name + "/log", msg);


        // Get metadata types
        if (pixelSensorFeature.EnumeratePixelSensorMetaDataTypes(sensorType, (uint) stream_Index, out var metaDataTypes))
        {
            metaDataTypesByStream[(uint) stream_Index] = metaDataTypes;
            msg = new StringMsg($"depth/infra: Metadata types for stream {(uint) stream_Index} retrieved successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg($"depth/infra: Failed to retrieve metadata types for stream {(uint) stream_Index}.");
            ros.Publish(topic_base_name + "/log", msg);
        }

        // Start the sensor with the configuration and specify that all of the meta data should be requested.
        var sensorStartAsyncResult = pixelSensorFeature.StartSensor(sensorType, streamIndexes, metaDataTypesByStream);

        yield return sensorStartAsyncResult;

        if (!sensorStartAsyncResult.DidOperationSucceed)
        {
            msg = new StringMsg("depth/infra: Stream could not be started.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }
        msg = new StringMsg("depth/infra: Stream started successfully.");
        ros.Publish(topic_base_name + "/log", msg);
        yield return ProcessSensorData();
    }

    private IEnumerator ProcessSensorData()
    {

        while ( pixelSensorFeature.GetSensorStatus(sensorType) == PixelSensorStatus.Started)
        {
            foreach (var stream in streamIndexes)
            {
                if (pixelSensorFeature.GetSensorData(
                        sensorType, stream,
                        out var frame,
                        out PixelSensorMetaData[] currentFrameMetaData,
                        Allocator.Temp,
                        shouldFlipTexture: false))
                {
                    float test1 = (float) (frame.CaptureTime - precedent_time);
                    float test2 = 1000000000/(float) 5.0;
                    
                    if ((uint) stream_Index == 0)
                    {
                        test2 = 1000000000/(float)UpdateRate_stream0;
                    }
                    else if ((uint) stream_Index == 1)
                    {
                        test2 = 1000000000/(float)UpdateRate_stream1;
                    }
                    float jitter = test2 * 0.1f;
                    
                    if( test1 > (test2-jitter) )
                    {
                        long frameUnixNano = _bootTimeUnixNano + (long)frame.CaptureTime;
                        int sec = (int)(frameUnixNano / 1_000_000_000);
                        uint nanosec = (uint)(frameUnixNano % 1_000_000_000);

                        SendFrameROS(frame, sec, nanosec);
                        SendIntrinsecsROS(currentFrameMetaData, sec, nanosec);

                        precedent_time = frame.CaptureTime;
                    }
                }
            }
            yield return null;
        }
    }

    private void SendFrameROS(in PixelSensorFrame frame, int sec, uint nanosec)
    {
        var message = new ImageMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg 
            { 
                frame_id = "depth_optical_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            height = (uint) ResolutionHeight,
            width = (uint) ResolutionWidth,
            encoding = "32FC1",
            is_bigendian = 0,
            step = (uint)(ResolutionWidth * 4),
            data = frame.Planes[0].ByteData.ToArray()
        };

        ros.Publish(topicName, message);
    }

    private void SendIntrinsecsROS(PixelSensorMetaData[] frameMetaData, int sec, uint nanosec)
    {
        double k1 = 0, k2 = 0, p1 = 0, p2 = 0, k3 = 0, k4 = 0, k5 = 0, fx = 0, cx = 0, fy = 0, cy = 0;
        var builder = new StringBuilder();
        for (int i = 0; i < frameMetaData.Length; i++)
        {
            var metaData = frameMetaData[i];
            switch (metaData)
            {
                case PixelSensorAnalogGain analogGain:
                    builder.AppendLine($"AnalogGain: {analogGain.AnalogGain}");
                    break;
                case PixelSensorDigitalGain digitalGain:
                    builder.AppendLine($"DigitalGain: {digitalGain.DigitalGain}");
                    break;
                case PixelSensorExposureTime exposureTime:
                    builder.AppendLine($"ExposureTime: {exposureTime.ExposureTime:F1}");
                    break;
                case PixelSensorDepthFrameIllumination illumination:
                    builder.AppendLine($"IlluminationType: {illumination.IlluminationType}");
                    break;
                case PixelSensorFisheyeIntrinsics fisheyeIntrinsics:
                {
                    builder.AppendLine($"FOV: {fisheyeIntrinsics.FOV}");
                    builder.AppendLine($"Focal Length: {fisheyeIntrinsics.FocalLength}");
                    builder.AppendLine($"Principal Point: {fisheyeIntrinsics.PrincipalPoint}");
                    builder.AppendLine($"Radial Distortion: [{string.Join(',', fisheyeIntrinsics.RadialDistortion.Select(val => val.ToString("F1")))}]");
                    builder.AppendLine($"Tangential Distortion: [{string.Join(',', fisheyeIntrinsics.TangentialDistortion.Select(val => val.ToString("F1")))}]");
                    fx = fisheyeIntrinsics.FocalLength[0];
                    fy = fisheyeIntrinsics.FocalLength[1];
                    cx = fisheyeIntrinsics.PrincipalPoint[0];
                    cy = fisheyeIntrinsics.PrincipalPoint[1];
                    k1 = fisheyeIntrinsics.RadialDistortion[0];
                    k2 = fisheyeIntrinsics.RadialDistortion[1];
                    p1 = fisheyeIntrinsics.TangentialDistortion[0];
                    p2 = fisheyeIntrinsics.TangentialDistortion[1];
                    k3 = fisheyeIntrinsics.RadialDistortion[2];
                    k4 = fisheyeIntrinsics.RadialDistortion[3];
                    k5 = fisheyeIntrinsics.RadialDistortion[4];
                    break;
                }
                case PixelSensorPinholeIntrinsics pinholeIntrinsics:
                {
                    builder.AppendLine($"FOV: {pinholeIntrinsics.FOV}");
                    builder.AppendLine($"Focal Length: {pinholeIntrinsics.FocalLength}");
                    builder.AppendLine($"Principal Point: {pinholeIntrinsics.PrincipalPoint}");
                    builder.AppendLine($"Distortion: [{string.Join(',', pinholeIntrinsics.Distortion.Select(val => val.ToString("F1")))}]");
                    fx = pinholeIntrinsics.FocalLength[0];
                    fy = pinholeIntrinsics.FocalLength[1];
                    cx = pinholeIntrinsics.PrincipalPoint[0];
                    cy = pinholeIntrinsics.PrincipalPoint[1];
                    k1 = pinholeIntrinsics.Distortion[0];
                    k2 = pinholeIntrinsics.Distortion[1];
                    p1 = pinholeIntrinsics.Distortion[2];
                    p2 = pinholeIntrinsics.Distortion[3];
                    k3 = pinholeIntrinsics.Distortion[4];
                    break;
                }
            }
        }
        StringMsg msg = new StringMsg(builder.ToString());
        ros.Publish(topicNameMetaData, msg);

        CameraInfoMsg cameraInfo = new CameraInfoMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg 
            { 
                frame_id = "depth_optical_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },

            width = (uint) ResolutionWidth,
            height = (uint) ResolutionHeight,
            
            distortion_model = "plumb_bob",
            d = new double[] { k1, k2, p1, p2, k3 },
            

            k = new double[] {
                fx,  0, cx,
                0, fy, cy,
                0,  0,  1
            },

            r = new double[] {
                1, 0, 0,
                0, 1, 0,
                0, 0, 1
            },

            p = new double[] {
                fx,  0, cx, 0,
                0, fy, cy, 0,
                0,  0,  1, 0
            }
        };

        ros.Publish(topicNameIntrinsecs, cameraInfo);
    }

    private void SendPoseROS(Pose sensorPose, int sec, uint nanosec)
    {
        TransformStampedMsg transform0 = new TransformStampedMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg 
            { 
                frame_id = "magicleap_world",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            child_frame_id = "depth_frame",
            transform = new TransformMsg
            {
                translation = new Vector3Msg
                {
                    x = sensorPose.position.z,
                    y = - sensorPose.position.x,
                    z = sensorPose.position.y 
                },
                rotation = new QuaternionMsg
                {
                    x = - sensorPose.rotation.z,
                    y = sensorPose.rotation.x,
                    z = - sensorPose.rotation.y,
                    w = sensorPose.rotation.w                   
                }
            }
        };

        TransformStampedMsg transform1 = new TransformStampedMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg 
            { 
                frame_id = "depth_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            child_frame_id = "depth_optical_frame",
            transform = new TransformMsg
            {
                translation = new Vector3Msg
                {
                    x = 0,
                    y = 0,
                    z = 0
                },
                rotation = new QuaternionMsg
                {
                    x = - 0.5,
                    y = 0.5,
                    z = - 0.5,
                    w = 0.5                   
                }
            }
        };

        TFMessageMsg poseMsg = new TFMessageMsg
        {
            transforms = new TransformStampedMsg[]{transform0, transform1}
        };

        ros.Publish(topicNamePose, poseMsg);
    }

    private void OnDisable()
    {
        var camMono = Camera.main.GetComponent<MonoBehaviour>();
        camMono.StartCoroutine(StopSensorCoroutine());
    }

    private IEnumerator StopSensorCoroutine()
    {
        var sensorStopAsyncResult = pixelSensorFeature.StopSensor(sensorType, streamIndexes);
        yield return sensorStopAsyncResult;

        if (sensorStopAsyncResult.DidOperationSucceed)
        {
            bool wasDestroyed = pixelSensorFeature.DestroyPixelSensor(sensorType);
            if (wasDestroyed)
            {
                msg = new StringMsg("depth/infra: Sensor destroyed.");
                ros.Publish(topic_base_name + "/log", msg);
            }
            msg = new StringMsg("depth/infra: Sensor streaming stopped successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("depth/infra: Failed to stop sensor streaming.");
            ros.Publish(topic_base_name + "/log", msg);
        }
    }
}
