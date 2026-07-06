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
public class Eye_Temple_Right_Definitive : MonoBehaviour
{
    [HideInInspector]
    public bool turn_on = false;

    [HideInInspector]
    public string topic_base_name = "/magic_leap2";

    StringMsg msg;

    private MagicLeapPixelSensorFeature pixelSensorFeature;
    private PixelSensorId sensorType;
    private ROSConnection ros;
    private long precedent_time;
    private uint[] streamIndexes = new uint[1];
    private Dictionary<uint, PixelSensorMetaDataType[]> metaDataTypesByStream = new();
    private string sensor_Name = "Eye Temple Right";

    private string topicName;
    private string topicNameIntrinsecs;
    private string topicNameMetaData;
    private string topicNamePose;

    
    public enum stream : uint { stream0 = 0};
    public stream stream_Index;
    public enum possible_resolution : uint { res400x400 = 0};
    public possible_resolution resolution;
    private int ResolutionWidth;
    private int ResolutionHeight;
    public enum possible_update_rate : int { fps30 = 30};
    public possible_update_rate UpdateRate;
    public enum possible_format : int {Grayscale = 0};
    public possible_format Format;

    private long _bootTimeUnixNano;

    void Start()
    {
        long currentUnixNano = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000;
        long currentSystemNano = System.Diagnostics.Stopwatch.GetTimestamp() * (1_000_000_000 / System.Diagnostics.Stopwatch.Frequency);
        _bootTimeUnixNano = currentUnixNano - currentSystemNano;
    }

    void Update()
    {
        if (turn_on)
        {
            topicName = topic_base_name + "/eye_temple_right/image_raw";

            topicNameIntrinsecs = topic_base_name + "/eye_temple_right/camera_info";

            topicNameMetaData = topic_base_name + "/eye_temple_right/metatdata";

            topicNamePose = "/tf";

            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<ImageMsg>(topicName);
            ros.RegisterPublisher<CameraInfoMsg>(topicNameIntrinsecs);
            ros.RegisterPublisher<StringMsg>(topicNameMetaData);
            ros.RegisterPublisher<TFMessageMsg>(topicNamePose);
            ros.RegisterPublisher<StringMsg>(topic_base_name + "/log");

            streamIndexes[0] = (uint) stream_Index;

            if (resolution == 0)
            {
                ResolutionWidth = 400;
                ResolutionHeight = 400;
            }

            // Attempt to retrieve the MagicLeapPixelSensorFeature from the OpenXR settings
            pixelSensorFeature = OpenXRSettings.Instance.GetFeature<MagicLeapPixelSensorFeature>();

            // Check if the feature is available and enabled
            if (pixelSensorFeature == null || !pixelSensorFeature.enabled)
            {
                msg = new StringMsg("eye_temple_right: Magic Leap Pixel Sensor Feature is not available or not enabled.");
                ros.Publish(topic_base_name + "/log", msg);
                enabled = false;
                return;
            }

            // Get the list of supported sensors and log their details
            List<PixelSensorId> supportedSensors = pixelSensorFeature.GetSupportedSensors();
            
            sensorType = supportedSensors.Find(s => s.SensorName == sensor_Name);

            // Cretion
            bool wasCreated = pixelSensorFeature.CreatePixelSensor(sensorType);
            msg = new StringMsg($"eye_temple_right: Sensor creation was successful: {wasCreated}");
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
                    msg = new StringMsg($"eye_temple_right: Failed to get capabilities for stream index {streamIndex}");
                    ros.Publish(topic_base_name + "/log", msg);
                }
            }


            // Settings
            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Format, (uint) PixelSensorFrameFormat.Grayscale , (uint) stream_Index);
            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.Resolution, new Vector2Int(ResolutionWidth, ResolutionHeight), (uint) stream_Index);
            pixelSensorFeature.ApplySensorConfig(sensorType, PixelSensorCapabilityType.UpdateRate, (uint) UpdateRate, (uint) stream_Index);
            

            // Start stream
            StartCoroutine(StartSensorStream());

            turn_on = false;
        }
    }

        private IEnumerator StartSensorStream()
    {
        // Submit congiguration
        var pixelSensorOperation = pixelSensorFeature.ConfigureSensor(sensorType, streamIndexes);
        yield return pixelSensorOperation;
        if (pixelSensorOperation.DidOperationSucceed)
        {
            msg = new StringMsg("eye_temple_right: Sensor configuration successful.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("eye_temple_right: Sensor configured successfully.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }

        msg = new StringMsg("eye_temple_right: Sensor configured successfully.");
        ros.Publish(topic_base_name + "/log", msg);


        // Get metadata types
        if (pixelSensorFeature.EnumeratePixelSensorMetaDataTypes(sensorType, (uint) stream_Index, out var metaDataTypes))
        {
            metaDataTypesByStream[(uint) stream_Index] = metaDataTypes;
            msg = new StringMsg($"eye_temple_right: Metadata types for stream {(uint) stream_Index} retrieved successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg($"eye_temple_right: Failed to retrieve metadata types for stream {(uint) stream_Index}.");
            ros.Publish(topic_base_name + "/log", msg);
        }

        // Start the sensor with the configuration and specify that all of the meta data should be requested.
        var sensorStartAsyncResult = pixelSensorFeature.StartSensor(sensorType, streamIndexes, metaDataTypesByStream);

        yield return sensorStartAsyncResult;

        if (!sensorStartAsyncResult.DidOperationSucceed)
        {
            msg = new StringMsg("eye_temple_right: Stream could not be started.");
            ros.Publish(topic_base_name + "/log", msg);
            yield break;
        }

        msg = new StringMsg("eye_temple_right: Stream started successfully.");
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
                        shouldFlipTexture: true))
                {
                    float test1 = (float) (frame.CaptureTime - precedent_time);
                    float test2 = 1000000000/(float)UpdateRate;
                    float jitter = test2 * 0.1f;

                    if( test1 > (test2-jitter) )
                    {
                        long frameUnixNano = _bootTimeUnixNano + (long)frame.CaptureTime;
                        int sec = (int)(frameUnixNano / 1_000_000_000);
                        uint nanosec = (uint)(frameUnixNano % 1_000_000_000);

                        SendFrameROS(frame, sec, nanosec);
                        SendIntrinsecsROS(currentFrameMetaData);

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
                frame_id = "eye_temple_right_optical_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg(sec, nanosec)
            },
            height = (uint) ResolutionHeight,
            width = (uint) ResolutionWidth,
            encoding = "mono8",
            is_bigendian = 0,
            step = (uint)(ResolutionWidth * 1),
            data = frame.Planes[0].ByteData.ToArray()
        };
        ros.Publish(topicName, message);
    }

    private void SendIntrinsecsROS(PixelSensorMetaData[] frameMetaData)
    {
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
                    break;
                }
                case PixelSensorPinholeIntrinsics pinholeIntrinsics:
                {
                    builder.AppendLine($"FOV: {pinholeIntrinsics.FOV}");
                    builder.AppendLine($"Focal Length: {pinholeIntrinsics.FocalLength}");
                    builder.AppendLine($"Principal Point: {pinholeIntrinsics.PrincipalPoint}");
                    builder.AppendLine($"Distortion: [{string.Join(',', pinholeIntrinsics.Distortion.Select(val => val.ToString("F1")))}]");
                    break;
                }
            }
        }
        StringMsg msg = new StringMsg(builder.ToString());
        ros.Publish(topicNameMetaData, msg);
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
                msg = new StringMsg("eye_temple_right: Sensor destroyed.");
                ros.Publish(topic_base_name + "/log", msg);
            }
            msg = new StringMsg("eye_temple_right: Sensor streaming stopped successfully.");
            ros.Publish(topic_base_name + "/log", msg);
        }
        else
        {
            msg = new StringMsg("eye_temple_right: Failed to stop sensor streaming.");
            ros.Publish(topic_base_name + "/log", msg);
        }
    }
}
