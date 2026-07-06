import os
from ament_index_python.packages import get_package_share_directory
from launch import LaunchDescription
from launch_ros.actions import Node

def generate_launch_description():
    # Recupera il percorso del file YAML installato nella share del pacchetto
    config_file = os.path.join(
        get_package_share_directory('config_ml2_stream'),
        'config',
        'config.yaml'
    )

    return LaunchDescription([
        Node(
            package='config_ml2_stream',
            executable='config_publisher',
            name='config_ML_node',
            output='screen',
            parameters=[config_file] # Passaggio automatico del file di parametri
        )
    ])