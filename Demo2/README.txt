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





Per visualizzre le immagini pubblicate su ROS è possibile usare rviz2
Aprire un terminale, posizionarsi nella cartella _nome_ws_, fare i seguenti source (qualora sia necessario):
	$ source /opt/ros/jazzy/setup.bash
	$ source install/setup.bash
Avviare rviz2:
	$ rviz2
Si apre la finestra, cliccare su Add.
Cliccare By topic.
Selezionare il topic da visualizzare.
Premere Image.
Ora dovrebbe essere possibile visualizzare le immagini pubblicate dal topic.
E' normale siano storte poichè la demo formatta così le immagini trasmesse.
E' sempre possibile girarle con un altro nodo ros che legga le immagini ricevute, le giri e le ripubblichi girate su un altro topic.


	

Ulteriori comandi utili
Visualizzare i nodi attualmente attivi:
	$ ros2 node list
Visualizzare i topic attualmente attivi:
	$ ros2 topic list
Fermare i topic e i nodi attivi:
	$ pkill -f nome_topic_o_nodo




All'esecuzione della Demo i comandi sono:
w e s per muoversi avanti e indietro.
a e d per muovere la telecamera a destra e a sinistra.
c per cambiare camera
v per salvare in screenshot una singola immagine (Funziona se la cartella Demo1 è dentro una cartella project sul Desktop).
tenendo premuto b il programma salva nella cartella screenshot una immagine per ogni update in cui il tasto è premuto (Funziona se la cartella Demo1 è dentro una cartella project sul Desktop).
Premere n il programma cattura immagini ad ogni update e li invia al computer con ROS2 nella rete locale.
Ripremendo n il programma smette di catturare e inviare immagini a ros
