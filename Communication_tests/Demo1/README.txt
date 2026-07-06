Versione utilizzata di Unity: Unity 2022.3.62f3 LTS

Lato Unity è stato necessario installare e configurare la libreria
Dalla barra in alto cliccare "Window".
Cliccare "Package Manager" dal menu che è comparso.
Cliccare sulla + in alto a sinistra.
Cliccare "add package from git URL".
Incollare il seguente url e premere add:
https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector
Se tutto è stato fatto correttamente nel menu in alto dovrebbe essere comparsa la voce "Robotics".
Cliccandoci sopra è possibile modificare l'ip del computer a cui ci si intende connettere.
In questa finestra è anche necessario selezionare "ROS2" siccome stiamo lavorando con ROS2.




Lato ROS2(jazzy Jalisco)
Se necessario:
	$ source /opt/ros/jazzy/setup.bash

Creare un workspace per la demo:
	$ mkdir -p ~/_nome_ws_/src

Entrare in _nome_ws_:
	$ cd _nome_ws_

installare la cartella reperibile al seguente link e copiarla in src:
	https://github.com/Unity-Technologies/ROS-TCP-Endpoint/releases/tag/ROS2v0.7.0

Compilare. Posizionarsi nella cartella _nome_ws_ se non si è già e lanciare il commando:
	$  colcon build

Fare il source del ws:
	$ source install/setup.bash

Lanciare il nodo che lancerà i topic per conto di unity:
	$ ros2 launch ros_tcp_endpoint endpoint.py


Adesso il pc con ros attende che unity si colleghi, pertanto è possibile eseguire la demo in unity.