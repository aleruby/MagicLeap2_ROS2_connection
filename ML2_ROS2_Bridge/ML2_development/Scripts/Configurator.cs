using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

using Newtonsoft.Json;

public class Configurator : MonoBehaviour
{
    [System.Serializable]
    public class AppConfig
    {
        [System.Serializable]
        public class General
        {
            public string topic_name;
        }

        [System.Serializable]
        public class Picture_center
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Depth_center
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class World_center
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class World_right
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class World_left
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Eye_temple_right
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Eye_temple_left
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Eye_nasal_right
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Eye_nasal_left
        {
            public bool enabled;
            public int stream;
            public int resolution_width;
            public int resolution_height;
            public int update_rate;
            public string format;
        }

        [System.Serializable]
        public class Other_sensors
        {
            public bool enabled;
        }

        public General general;
        public Picture_center picture_center;
        public Depth_center depth_center;
        public World_center world_center;
        public World_right world_right;
        public World_left world_left;
        public Eye_temple_right eye_temple_right;
        public Eye_temple_left eye_temple_left;
        public Eye_nasal_right eye_nasal_right;
        public Eye_nasal_left eye_nasal_left;
        public Other_sensors other_sensors;
    }

    public AppConfig conf;

    private ROSConnection ros;

    private Other_Sensors_Definitive _Other_Sensors_Definitive;

    private Color_Definitive _Color_Definitive;

    private Depth_Definitive _Depth_Definitive;

    private World_Center_Definitive _World_Center_Definitive;

    private World_Right_Definitive _World_Right_Definitive;

    private World_Left_Definitive _World_Left_Definitive;

    private Eye_Nasal_Right_Definitive _Eye_Nasal_Right_Definitive;

    private Eye_Nasal_Left_Definitive _Eye_Nasal_Left_Definitive;

    private Eye_Temple_Right_Definitive _Eye_Temple_Right_Definitive;

    private Eye_Temple_Left_Definitive _Eye_Temple_Left_Definitive;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>("request");
        ros.Subscribe<StringMsg>("/MagicLeap/config", ReceiveConfig);
        StringMsg requestmsg = new StringMsg("REQUEST");
        ros.Publish("request", requestmsg);
    }

    void Update()
    {
        
    }

    void ReceiveConfig(StringMsg jsonMessage)
    {
        conf = JsonConvert.DeserializeObject<AppConfig>(jsonMessage.data);

        // Log building with the sensor configuration
        string log = "Configuration received\n";
        log += "general \n";
        log += conf.general.topic_name + "\n";
        log += "\n";

        log += "other_sensors /n";
        log += "enabled: "+conf.other_sensors.enabled + "\n";
        log += "/n";

        log += "picture_center /n";
        log += "enabled: "+conf.picture_center.enabled + "\n";
        log += "stream: " + conf.picture_center.stream + "\n";
        log += "resolution_width: "+conf.picture_center.resolution_width + "\n";
        log += "resolution_height: " + conf.picture_center.resolution_height + "\n";
        log += "update_rate: " + conf.picture_center.update_rate + "\n";
        log += "format: "+conf.picture_center.format + "\n";
        log += "\n";

        log += "depth_center /n";
        log += "enabled: "+conf.depth_center.enabled + "\n";
        log += "stream: " + conf.depth_center.stream + "\n";
        log += "resolution_width: "+conf.depth_center.resolution_width + "\n";
        log += "resolution_height: " + conf.depth_center.resolution_height + "\n";
        log += "update_rate: " + conf.depth_center.update_rate + "\n";
        log += "format: "+conf.depth_center.format + "\n";
        log += "\n";

        log += "world_center /n";
        log += "enabled: "+conf.world_center.enabled + "\n";
        log += "stream: " + conf.world_center.stream + "\n";
        log += "resolution_width: "+conf.world_center.resolution_width + "\n";
        log += "resolution_height: " + conf.world_center.resolution_height + "\n";
        log += "update_rate: " + conf.world_center.update_rate + "\n";
        log += "format: "+conf.world_center.format + "\n";
        log += "\n";

        log += "world_right /n";
        log += "enabled: "+conf.world_right.enabled + "\n";
        log += "stream: " + conf.world_right.stream + "\n";
        log += "resolution_width: "+conf.world_right.resolution_width + "\n";
        log += "resolution_height: " + conf.world_right.resolution_height + "\n";
        log += "update_rate: " + conf.world_right.update_rate + "\n";
        log += "format: "+conf.world_right.format + "\n";
        log += "\n";

        log += "world_left /n";
        log += "enabled: "+conf.world_left.enabled + "\n";
        log += "stream: " + conf.world_left.stream + "\n";
        log += "resolution_width: "+conf.world_left.resolution_width + "\n";
        log += "resolution_height: " + conf.world_left.resolution_height + "\n";
        log += "update_rate: " + conf.world_left.update_rate + "\n";
        log += "format: "+conf.world_left.format + "\n";
        log += "\n";

        log += "eye_temple_right /n";
        log += "enabled: "+conf.eye_temple_right.enabled + "\n";
        log += "stream: " + conf.eye_temple_right.stream + "\n";
        log += "resolution_width: "+conf.eye_temple_right.resolution_width + "\n";
        log += "resolution_height: " + conf.eye_temple_right.resolution_height + "\n";
        log += "update_rate: " + conf.eye_temple_right.update_rate + "\n";
        log += "format: "+conf.eye_temple_right.format + "\n";
        log += "\n";

        log += "eye_temple_left /n";
        log += "enabled: "+conf.eye_temple_left.enabled + "\n";
        log += "stream: " + conf.eye_temple_left.stream + "\n";
        log += "resolution_width: "+conf.eye_temple_left.resolution_width + "\n";
        log += "resolution_height: " + conf.eye_temple_left.resolution_height + "\n";
        log += "update_rate: " + conf.eye_temple_left.update_rate + "\n";
        log += "format: "+conf.eye_temple_left.format + "\n";
        log += "\n";

        log += "eye_nasal_right /n";
        log += "enabled: "+conf.eye_nasal_right.enabled + "\n";
        log += "stream: " + conf.eye_nasal_right.stream + "\n";
        log += "resolution_width: "+conf.eye_nasal_right.resolution_width + "\n";
        log += "resolution_height: " + conf.eye_nasal_right.resolution_height + "\n";
        log += "update_rate: " + conf.eye_nasal_right.update_rate + "\n";
        log += "format: "+conf.eye_nasal_right.format + "\n";
        log += "\n";

        log += "eye_nasal_left /n";
        log += "enabled: "+conf.eye_nasal_left.enabled + "\n";
        log += "stream: " + conf.eye_nasal_left.stream + "\n";
        log += "resolution_width: "+conf.eye_nasal_left.resolution_width + "\n";
        log += "resolution_height: " + conf.eye_nasal_left.resolution_height + "\n";
        log += "update_rate: " + conf.eye_nasal_left.update_rate + "\n";
        log += "format: "+conf.eye_nasal_left.format + "\n";
        log += "\n";

        ros.RegisterPublisher<StringMsg>(conf.general.topic_name + "/log");

        StringMsg logconfigurazione = new StringMsg(log);
        ros.Publish(conf.general.topic_name + "/log", logconfigurazione);

        // Other Sensrors configuration
        _Other_Sensors_Definitive = GetComponent<Other_Sensors_Definitive>();
        _Other_Sensors_Definitive.topic_base_name = conf.general.topic_name;
        _Other_Sensors_Definitive.turn_on = conf.other_sensors.enabled;

        // Color configuration
        _Color_Definitive = GetComponent<Color_Definitive>();
        _Color_Definitive.topic_base_name = conf.general.topic_name;
        _Color_Definitive.stream_Index = (Color_Definitive.stream) conf.picture_center.stream;
        _Color_Definitive.UpdateRate = conf.picture_center.update_rate;
        
        if (conf.picture_center.resolution_width == 640 && conf.picture_center.resolution_height == 480 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 0;
        }
        else if (conf.picture_center.resolution_width == 1280 && conf.picture_center.resolution_height == 720 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 1;
        }
        else if (conf.picture_center.resolution_width == 1920 && conf.picture_center.resolution_height == 1080 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 2;
        }
        else if (conf.picture_center.resolution_width == 3840 && conf.picture_center.resolution_height == 2160 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 3;
        }
        else if (conf.picture_center.resolution_width == 2048 && conf.picture_center.resolution_height == 1536 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 4;
        }
        else if (conf.picture_center.resolution_width == 1280 && conf.picture_center.resolution_height == 960 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 5;
        }
        else if (conf.picture_center.resolution_width == 1440 && conf.picture_center.resolution_height == 1080 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 6;
        }
        else if (conf.picture_center.resolution_width == 2880 && conf.picture_center.resolution_height == 2160 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 7;
        }
        else if (conf.picture_center.resolution_width == 4096 && conf.picture_center.resolution_height == 3072 && conf.picture_center.stream == 0)
        {
            _Color_Definitive.resolution_Stream_0 = (Color_Definitive.possible_resolution_0) 8;
        }
        
        else if (conf.picture_center.resolution_width == 648 && conf.picture_center.resolution_height == 720 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 0;
        }
        else if (conf.picture_center.resolution_width == 972 && conf.picture_center.resolution_height == 1080 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 1;
        }
        else if (conf.picture_center.resolution_width == 1944 && conf.picture_center.resolution_height == 2160 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 2;
        }
        else if (conf.picture_center.resolution_width == 960 && conf.picture_center.resolution_height == 720 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 3;
        }
        else if (conf.picture_center.resolution_width == 1440 && conf.picture_center.resolution_height == 1080 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 4;
        }
        else if (conf.picture_center.resolution_width == 2880 && conf.picture_center.resolution_height == 2160 && conf.picture_center.stream == 1)
        {
            _Color_Definitive.resolution_Stream_1 = (Color_Definitive.possible_resolution_1) 5;
        }

        if (conf.picture_center.format == "jpeg")
        {
            _Color_Definitive.Format = 0;
        }
        _Color_Definitive.turn_on = conf.picture_center.enabled;

        // Depth configuration
        _Depth_Definitive = GetComponent<Depth_Definitive>();
        _Depth_Definitive.topic_base_name = conf.general.topic_name;
        _Depth_Definitive.stream_Index = (Depth_Definitive.stream) conf.depth_center.stream;
        if (conf.depth_center.resolution_width == 544 && conf.depth_center.resolution_height == 480)
        {
            _Depth_Definitive.resolution = 0;
        }
        
        if (conf.depth_center.update_rate == 1 && conf.depth_center.stream == 0)
        {
            _Depth_Definitive.UpdateRate_stream0 = (Depth_Definitive.possible_update_rate_stream0) 1;
        }
        else if (conf.depth_center.update_rate == 5 && conf.depth_center.stream == 0)
        {
            _Depth_Definitive.UpdateRate_stream0 = (Depth_Definitive.possible_update_rate_stream0) 5;
        }
        
        if (conf.depth_center.update_rate == 5 && conf.depth_center.stream == 1)
        {
            _Depth_Definitive.UpdateRate_stream1 = (Depth_Definitive.possible_update_rate_stream1) 5;
        }
        else if (conf.depth_center.update_rate == 30 && conf.depth_center.stream == 1)
        {
            _Depth_Definitive.UpdateRate_stream1 = (Depth_Definitive.possible_update_rate_stream1) 30;
        }
        else if (conf.depth_center.update_rate == 60 && conf.depth_center.stream == 1)
        {
            _Depth_Definitive.UpdateRate_stream1 = (Depth_Definitive.possible_update_rate_stream1) 60;
        }

        if (conf.depth_center.format == "Depth32")
        {
            _Depth_Definitive.format = (Depth_Definitive.possible_format) 0;
        }
        else if (conf.depth_center.format == "DepthRaw")
        {
            _Depth_Definitive.format = (Depth_Definitive.possible_format) 1;
        }
        _Depth_Definitive.turn_on = conf.depth_center.enabled;

        // World Center configuration
        _World_Center_Definitive = GetComponent<World_Center_Definitive>();
        _World_Center_Definitive.topic_base_name = conf.general.topic_name;
        _World_Center_Definitive.stream_Index = (World_Center_Definitive.stream) conf.world_center.stream;
        if (conf.world_center.resolution_width == 1016 && conf.world_center.resolution_height == 1016)
        {
            _World_Center_Definitive.resolution = 0;
        }
        if (conf.world_center.format == "Grayscale")
        {
            _World_Center_Definitive.Format = 0;
        }
        _World_Center_Definitive.turn_on = conf.world_center.enabled;

        // Word Right configuration
        _World_Right_Definitive = GetComponent<World_Right_Definitive>();
        _World_Right_Definitive.topic_base_name = conf.general.topic_name;
        _World_Right_Definitive.stream_Index = (World_Right_Definitive.stream) conf.world_right.stream;
        if (conf.world_right.resolution_width == 1016 && conf.world_right.resolution_height == 1016)
        {
            _World_Right_Definitive.resolution = 0;
        }
        if (conf.world_right.format == "Grayscale")
        {
            _World_Right_Definitive.Format = 0;
        }
        _World_Right_Definitive.turn_on = conf.world_right.enabled;

        // Word Left configuration
        _World_Left_Definitive = GetComponent<World_Left_Definitive>();
        _World_Left_Definitive.topic_base_name = conf.general.topic_name;
        _World_Left_Definitive.stream_Index = (World_Left_Definitive.stream) conf.world_center.stream;
        if (conf.world_left.resolution_width == 1016 && conf.world_left.resolution_height == 1016)
        {
            _World_Left_Definitive.resolution = 0;
        }
        if (conf.world_left.format == "Grayscale")
        {
            _World_Left_Definitive.Format = 0;
        }
        _World_Left_Definitive.turn_on = conf.world_left.enabled;

        // Eye Nasal Right configuration
        _Eye_Nasal_Right_Definitive = GetComponent<Eye_Nasal_Right_Definitive>();
        _Eye_Nasal_Right_Definitive.topic_base_name = conf.general.topic_name;
        _Eye_Nasal_Right_Definitive.stream_Index = (Eye_Nasal_Right_Definitive.stream) conf.eye_nasal_right.stream;
        if (conf.eye_nasal_right.resolution_width == 400 && conf.eye_nasal_right.resolution_height == 400)
        {
            _Eye_Nasal_Right_Definitive.resolution = 0;
        }
        if (conf.eye_nasal_right.format == "Grayscale")
        {
            _Eye_Nasal_Right_Definitive.Format = 0;
        }
        _Eye_Nasal_Right_Definitive.turn_on = conf.eye_nasal_right.enabled;

        // Eye Nasal Left configuration
        _Eye_Nasal_Left_Definitive = GetComponent<Eye_Nasal_Left_Definitive>();
        _Eye_Nasal_Left_Definitive.topic_base_name = conf.general.topic_name;
        _Eye_Nasal_Left_Definitive.stream_Index = (Eye_Nasal_Left_Definitive.stream) conf.eye_nasal_left.stream;
        if (conf.eye_nasal_left.resolution_width == 400 && conf.eye_nasal_left.resolution_height == 400)
        {
            _Eye_Nasal_Left_Definitive.resolution = 0;
        }
        if (conf.eye_nasal_left.format == "Grayscale")
        {
            _Eye_Nasal_Left_Definitive.Format = 0;
        }
        _Eye_Nasal_Left_Definitive.turn_on = conf.eye_nasal_left.enabled;

        // Eye Temple Right configuration
        _Eye_Temple_Right_Definitive = GetComponent<Eye_Temple_Right_Definitive>();
        _Eye_Temple_Right_Definitive.topic_base_name = conf.general.topic_name;
        _Eye_Temple_Right_Definitive.stream_Index = (Eye_Temple_Right_Definitive.stream) conf.eye_temple_right.stream;
        if (conf.eye_temple_right.resolution_width == 400 && conf.eye_temple_right.resolution_height == 400)
        {
            _Eye_Temple_Right_Definitive.resolution = 0;
        }
        if (conf.eye_temple_right.format == "Grayscale")
        {
            _Eye_Temple_Right_Definitive.Format = 0;
        }
        _Eye_Temple_Right_Definitive.turn_on = conf.eye_temple_right.enabled;

        // Eye Temple Left configuration
        _Eye_Temple_Left_Definitive = GetComponent<Eye_Temple_Left_Definitive>();
        _Eye_Temple_Left_Definitive.topic_base_name = conf.general.topic_name;
        _Eye_Temple_Left_Definitive.stream_Index = (Eye_Temple_Left_Definitive.stream) conf.eye_temple_left.stream;
        if (conf.eye_temple_left.resolution_width == 400 && conf.eye_temple_left.resolution_height == 400)
        {
            _Eye_Temple_Left_Definitive.resolution = 0;
        }
        if (conf.eye_temple_left.format == "Grayscale")
        {
            _Eye_Temple_Left_Definitive.Format = 0;
        }
        _Eye_Temple_Left_Definitive.turn_on = conf.eye_temple_left.enabled;
    }
}