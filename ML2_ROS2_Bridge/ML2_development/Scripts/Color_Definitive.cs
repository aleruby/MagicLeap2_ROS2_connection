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

public class Color_Definitive : MonoBehaviour
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
    private string sensor_Name = "Picture Center";


    private string topicName;
    private string topicNameIntrinsecs;
    private string topicNameMetaData;
    private string topicNamePose;


    public enum stream : uint { Color_stream0 = 0,  MixedReality_Camera_stream1 = 1};
    public stream stream_Index;
    public enum possible_resolution_0 : uint
    {
        res640x480 = 0,
        res1280x720 = 1,
        res1920x1080 = 2,
        res3840x2160 = 3,
        res2048x1536 = 4,
        res1280x960 = 5,
        res1440x1080 = 6,
        res2880x2160 = 7,
        res4096x3072 = 8
    };
    public possible_resolution_0 resolution_Stream_0;
    public enum possible_resolution_1 : uint 
    {
        res648x720 = 0,
        res972x1080 = 1,
        res1944x2160 = 2,
        res960x720 = 3,
        res1440x1080 = 4,
        res2880x2160 = 5,
    };
    public possible_resolution_1 resolution_Stream_1;
    private int ResolutionWidth;
    private int ResolutionHeight;

    [Range(30, 60)]
    public int UpdateRate;

    public enum possible_format : int {Jpeg = 0};
    public possible_format Format;

    private long _bootTimeUnixNano;

    // Start is called before the first frame update
    void Start()
    {
        long currentUnixNano = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000;
        long currentSystemNano = System.Diagnostics.Stopwatch.GetTimestamp() * (1_000_000_000 / System.Diagnostics.Stopwatch.Frequency);
        _bootTimeUnixNano = currentUnixNano - currentSystemNano;
    }

    // Update is called once per frame
    void Update()
    {
        if (turn_on)
        {
            if ((int) stream_Index == 0)
            {
                topicName = topic_base_name + "/color/image_raw/compressed";

                topicNameIntrinsecs = topic_base_name + "/color/camera_info";

                topicNameMetaData = topic_base_name + "/color/metadata";

                topicNamePose = "/tf";
            }
            else if ((int) stream_Index == 1)
            {
                topicName = topic_base_name + "/mixed_reality/image_raw/compressed";

                topicNameIntrinsecs = topic_base_name + "/mixed_reality/camera_info";

                topicNameMetaData = topic_base_name + "/mixed_reality/metadata";

                topicNamePose = "/tf";
            }

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<CompressedImageMsg>(topicName);
            ros.RegisterPublisher<CameraInfoMsg>(topicNameIntrinsecs);
            ros.RegisterPublisher<StringMsg>(topicNameMetaData);
            ros.RegisterPublisher<TFMessageMsg>(topicNamePose);
            ros.RegisterPublisher<StringMsg>(topic_base_name + "/log");

            streamIndexes[0] = (uint) stream_Index;
            
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 0)
            {
                ResolutionWidth = 640;
                ResolutionHeight = 480;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 1)
            {
                ResolutionWidth = 1280;
                ResolutionHeight = 720;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 2)
            {
                ResolutionWidth = 1920;
                ResolutionHeight = 1080;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 3)
            {
                ResolutionWidth = 3840;
                ResolutionHeight = 2160;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 4)
            {
                ResolutionWidth = 2048;
                ResolutionHeight = 1536;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 5)
            {
                ResolutionWidth = 1280;
                ResolutionHeight = 960;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 6)
            {
                ResolutionWidth = 1440;
                ResolutionHeight = 1080;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 7)
            {
                ResolutionWidth = 2880;
                ResolutionHeight = 2160;
            }
            if ((uint) stream_Index == 0 && (int) resolution_Stream_0 == 8)
            {
                ResolutionWidth = 4096;
                ResolutionHeight = 3072;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 0)
            {
                ResolutionWidth = 648;
                ResolutionHeight = 720;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 1)
            {
                ResolutionWidth = 972;
                ResolutionHeight = 1080;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 2)
            {
                ResolutionWidth = 1944;
                ResolutionHeight = 2160;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 3)
            {
                ResolutionWidth = 960;
                ResolutionHeight = 720;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 4)
            {
                ResolutionWidth = 1440;
                ResolutionHeight = 1080;
            }
            if ((uint) stream_Index == 1 && (int) resolution_Stream_1 == 5)
            {
                ResolutionWidth = 2880;
                ResolutionHeight = 2160;
            }
            
            // Attempt to retrieve the MagicLeapPixelSensorFeature from the OpenXR settings
            pixelSensorFeature = OpenXRSettings.Instance.GetFeature<MagicLeapPixelSensorFeature>();

            // Check if the feature is available and enabled
            if (pixelSensorFeature == null || !pixelSensorFeature.enabled)
            {
                msg = new StringMsg("color/mixed_reality: Magic Leap Pixel Sensor Feature is not available or not enabled.");
                ros.Publish(topic_base_name + "/log", msg);
                enabled = false;
                return;
            }

            // Get the list of supported sensors and log their details
            List<PixelSensorId> supportedSensors = pixelSensorFeature.GetSupportedSensors();
            
            sensorType = supportedSensors.Find(s => s.SensorName == sensor_Name);

            // Creation
            bool wasCreated = pixelSensorFeature.CreatePixelSensor(sensorType);
            msg = new StringMsg($"color/mixed_reality: Sensor creation was successful: {wasCreated}");
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
                    msg = new StringMsg($"color/mixed_reality: Failed to get capabilities for stream index {streamIndex}");
                    ros.Publish(topic_base_name + "/log", msg);
                }
            }


            // Settings
            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Format, (uint) PixelSensorFrameFormat.Jpeg , (uint) stream_Index);

            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Resolution, new Vector2Int(ResolutionWidth, ResolutionHeight), (uint) stream_Index);

            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.UpdateRate, (uint) UpdateRate, (uint) stream_Index);

            // Starting stream
            StartCoroutine(StartSensorStream());

            pose_on = true;
            turn_on = false;
        }
        
        if (pose_on)
        {
            DateTime utcNow = DateTime.UtcNow;
            long unixTicks = utcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            uint sec = (uint)(unixTicks / TimeSpan.TicksPerSecond);
            uint nanosec = (uint)((unixTicks % TimeSpan.TicksPerSecond) * 100);
            Pose sensorPose = pixelSensorFeature.GetSensorPose(sensorType);
            SendPoseROS(sensorPose, (int)sec, nanosec);
        }
        
    }

    private IEnumerator StartSensorStream()
    {
        // Submit congiguration
        var pixelSensorOperation = pixelSensorFeature.ConfigureSensor(sensorType, streamIndexes);
        yield return pixelSensorOperation;
        if (pixelSensorOperation.DidOperationSucceed)
        {
            msg = new StringMsg("color/mixed_reality: Sensor configuration successful.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("color/mixed_reality: Sensor configuration failed.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }
        msg = new StringMsg("color/mixed_reality: Sensor configured successfully.");
        ros.Publish(topic_base_name + "/log", msg);


        // Get metadata types
        if (pixelSensorFeature.EnumeratePixelSensorMetaDataTypes(sensorType, (uint) stream_Index, out var metaDataTypes))
        {
            metaDataTypesByStream[(uint) stream_Index] = metaDataTypes;
            msg = new StringMsg($"color/mixed_reality: Metadata types for stream {(uint) stream_Index} retrieved successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg($"color/mixed_reality: Failed to retrieve metadata types for stream {(uint) stream_Index}.");
            ros.Publish(topic_base_name + "/log", msg);
        }

        // Start the sensor with the configuration and specify that all of the meta data should be requested.
        var sensorStartAsyncResult = pixelSensorFeature.StartSensor(sensorType, streamIndexes, metaDataTypesByStream);

        yield return sensorStartAsyncResult;

        if (!sensorStartAsyncResult.DidOperationSucceed)
        {
            msg = new StringMsg("color/mixed_reality: Stream could not be started.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }
        msg = new StringMsg("color/mixed_reality: Stream started successfully.");
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
                    float test2 = 1000000000/(float)UpdateRate;
                    float jitter = test2 * 0.1f;

                    if( test1 > (test2 - jitter) )
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
        var message = new CompressedImageMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = "color_optical_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            format = "jpeg",
            data = frame.Planes[0].ByteData.ToArray(),
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
                frame_id = "color_optical_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },

            width = (uint) ResolutionWidth,
            height = (uint) ResolutionHeight,
            
            distortion_model = "kannala_brandt",
            // d = new double[] { k1, k2, k3, k4},
            d = new double[] { 0, 0, 0, 0},

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
            child_frame_id = "color_frame",
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
                frame_id = "color_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            child_frame_id = "color_optical_frame",
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
                msg = new StringMsg("color/mixed_reality: Sensor destroyed.");
                ros.Publish(topic_base_name + "/log", msg);
            }
            msg = new StringMsg("color/mixed_reality: Sensor streaming stopped successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("color/mixed_reality: Failed to stop sensor streaming.");
            ros.Publish(topic_base_name + "/log", msg);
        }
    }
}
