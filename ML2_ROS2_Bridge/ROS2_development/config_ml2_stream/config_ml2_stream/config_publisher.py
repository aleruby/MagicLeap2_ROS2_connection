import rclpy
from rclpy.node import Node
from std_msgs.msg import String
from rclpy.qos import QoSProfile, DurabilityPolicy
import yaml
import json
import os

class ConfigPublisher(Node):
    def __init__(self):
        super().__init__('config_ML_node')
        
        # Configura la QoS in modo che il messaggio "aspetti" la connessione di Unity
        # qos_profile = QoSProfile(depth=1)
        # qos_profile.durability = DurabilityPolicy.TRANSIENT_LOCAL
        
        self.publisher1_ = self.create_publisher(String, '/MagicLeap/config', 10)

        self.publisher2_ = self.create_publisher(String, '/request', 10)

        #1. Dichiarazione dei parametri con valori di default (puoi sovrascriverli con il file YAML)      
        self.declare_parameter('general.topic_name', '/magic_leap2')

        self.declare_parameter('picture_center.enabled', False)
        self.declare_parameter('picture_center.stream', 0)
        self.declare_parameter('picture_center.resolution_width', 640)
        self.declare_parameter('picture_center.resolution_height', 480)
        self.declare_parameter('picture_center.update_rate', 30)
        self.declare_parameter('picture_center.format', 'jpeg')

        self.declare_parameter('depth_center.enabled', False)
        self.declare_parameter('depth_center.stream', 0)
        self.declare_parameter('depth_center.resolution_width', 544)
        self.declare_parameter('depth_center.resolution_height', 480)
        self.declare_parameter('depth_center.update_rate', 5)
        self.declare_parameter('depth_center.format', 'Depth32')

        self.declare_parameter('world_center.enabled', False)
        self.declare_parameter('world_center.stream', 0)
        self.declare_parameter('world_center.resolution_width', 1016)
        self.declare_parameter('world_center.resolution_height', 1016)
        self.declare_parameter('world_center.update_rate', 30)
        self.declare_parameter('world_center.format', 'Grayscale')

        self.declare_parameter('world_right.enabled', False)
        self.declare_parameter('world_right.stream', 0)
        self.declare_parameter('world_right.resolution_width', 1016)
        self.declare_parameter('world_right.resolution_height', 1016)
        self.declare_parameter('world_right.update_rate', 30)
        self.declare_parameter('world_right.format', 'Grayscale')

        self.declare_parameter('world_left.enabled', False)
        self.declare_parameter('world_left.stream', 0)
        self.declare_parameter('world_left.resolution_width', 1016)
        self.declare_parameter('world_left.resolution_height', 1016)
        self.declare_parameter('world_left.update_rate', 30)
        self.declare_parameter('world_left.format', 'Grayscale')

        self.declare_parameter('eye_temple_right.enabled', False)
        self.declare_parameter('eye_temple_right.stream', 0)
        self.declare_parameter('eye_temple_right.resolution_width', 400)
        self.declare_parameter('eye_temple_right.resolution_height', 400)
        self.declare_parameter('eye_temple_right.update_rate', 30)
        self.declare_parameter('eye_temple_right.format', 'Grayscale')

        self.declare_parameter('eye_temple_left.enabled', False)
        self.declare_parameter('eye_temple_left.stream', 0)
        self.declare_parameter('eye_temple_left.resolution_width', 400)
        self.declare_parameter('eye_temple_left.resolution_height', 400)
        self.declare_parameter('eye_temple_left.update_rate', 30)
        self.declare_parameter('eye_temple_left.format', 'Grayscale')

        self.declare_parameter('eye_nasal_right.enabled', False)
        self.declare_parameter('eye_nasal_right.stream', 0)
        self.declare_parameter('eye_nasal_right.resolution_width', 400)
        self.declare_parameter('eye_nasal_right.resolution_height', 400)
        self.declare_parameter('eye_nasal_right.update_rate', 30)
        self.declare_parameter('eye_nasal_right.format', 'Grayscale')

        self.declare_parameter('eye_nasal_left.enabled', False)
        self.declare_parameter('eye_nasal_left.stream', 0)
        self.declare_parameter('eye_nasal_left.resolution_width', 400)
        self.declare_parameter('eye_nasal_left.resolution_height', 400)
        self.declare_parameter('eye_nasal_left.update_rate', 30)
        self.declare_parameter('eye_nasal_left.format', 'Grayscale')

        self.declare_parameter('other_sensors.enabled', False)

        config_dict = {
            'general': {
                'topic_name': self.get_parameter('general.topic_name').get_parameter_value().string_value
            },
            'picture_center': {
                'enabled': self.get_parameter('picture_center.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('picture_center.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('picture_center.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('picture_center.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('picture_center.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('picture_center.format').get_parameter_value().string_value
            },
            'depth_center': {
                'enabled': self.get_parameter('depth_center.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('depth_center.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('depth_center.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('depth_center.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('depth_center.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('depth_center.format').get_parameter_value().string_value
            },
            'world_center': {
                'enabled': self.get_parameter('world_center.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('world_center.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('world_center.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('world_center.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('world_center.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('world_center.format').get_parameter_value().string_value
            },
            'world_right': {
                'enabled': self.get_parameter('world_right.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('world_right.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('world_right.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('world_right.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('world_right.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('world_right.format').get_parameter_value().string_value
            },
            'world_left': {
                'enabled': self.get_parameter('world_left.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('world_left.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('world_left.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('world_left.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('world_left.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('world_left.format').get_parameter_value().string_value
            },
            'eye_temple_right': {
                'enabled': self.get_parameter('eye_temple_right.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('eye_temple_right.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('eye_temple_right.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('eye_temple_right.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('eye_temple_right.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('eye_temple_right.format').get_parameter_value().string_value
            },
            'eye_temple_left': {
                'enabled': self.get_parameter('eye_temple_left.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('eye_temple_left.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('eye_temple_left.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('eye_temple_left.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('eye_temple_left.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('eye_temple_left.format').get_parameter_value().string_value
            },
            'eye_nasal_right': {
                'enabled': self.get_parameter('eye_nasal_right.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('eye_nasal_right.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('eye_nasal_right.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('eye_nasal_right.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('eye_nasal_right.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('eye_nasal_right.format').get_parameter_value().string_value
            },
            'eye_nasal_left': {
                'enabled': self.get_parameter('eye_nasal_left.enabled').get_parameter_value().bool_value,
                'stream': self.get_parameter('eye_nasal_left.stream').get_parameter_value().integer_value,
                'resolution_width': self.get_parameter('eye_nasal_left.resolution_width').get_parameter_value().integer_value,
                'resolution_height': self.get_parameter('eye_nasal_left.resolution_height').get_parameter_value().integer_value,
                'update_rate': self.get_parameter('eye_nasal_left.update_rate').get_parameter_value().integer_value,
                'format': self.get_parameter('eye_nasal_left.format').get_parameter_value().string_value
            },
            'other_sensors': {
                'enabled': self.get_parameter('other_sensors.enabled').get_parameter_value().bool_value
            }
        }
         
        # Converte il dizionario YAML in una stringa JSON
        self.json_string = json.dumps(config_dict)
        
        self.subscription = self.create_subscription(
            String,
            '/request',
            self.publish_config,
            10
        )
        # self.get_logger().info("Subscriber registrato sul topic /request")
        

    def publish_config(self, request):
        msg = String()
        msg.data = self.json_string

        self.publisher1_.publish(msg)
        # self.get_logger().info('Configurazione inviata a Unity!')

def main(args=None):
    rclpy.init(args=args)
    node = ConfigPublisher()
    rclpy.spin(node)
    node.destroy_node()
    rclpy.shutdown()
