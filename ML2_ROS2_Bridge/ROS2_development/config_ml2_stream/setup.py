import os
from glob import glob
from setuptools import find_packages, setup

package_name = 'config_ml2_stream'

setup(
    name=package_name,
    version='0.0.0',
    packages=find_packages(exclude=['test']),
    data_files=[
        ('share/ament_index/resource_index/packages',
            ['resource/' + package_name]),
        ('share/' + package_name, ['package.xml']),
        # Installa tutti i file .yaml della cartella config
        (os.path.join('share', package_name, 'config'), glob('config/*.yaml')),
        # Installa tutti i launch file nella cartella launch
        (os.path.join('share', package_name, 'launch'), glob('launch/*.launch.py')),
    ],
    install_requires=['setuptools'],
    zip_safe=True,
    maintainer='iaslab',
    maintainer_email='iaslab@todo.todo',
    description='TODO: Package description',
    license='TODO: License declaration',
    extras_require={
        'test': [
            'pytest',
        ],
    },
    entry_points={
        'console_scripts': [
            'config_publisher = config_ml2_stream.config_publisher:main'
        ],
    },
)
