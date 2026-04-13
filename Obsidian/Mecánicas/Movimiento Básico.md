
El movimiento básico consiste en dos partes principales: Caminar y Saltar. La intención es que el personaje del protagonista sea afectado por la gravedad, por las colisiones pero manteniendo cierto control del personaje. El moveset es el básico de un juego de plataformas.

Intención inicial: Utilizar addForce


## _***--Etapa 1--***_ 

La manera en la que se plantea el movimientto del jugador es siguiendo estas leyes:
- El personaje solo se puede mover horizontalmente mediante el imput del jugador (Se usa para ello el InputSystem).
- El personaje tiene la capacidad de saltar, pero únicamente cuando esta en el suelo.
- La gravedad siempre afecta al jugador mientras esté en el aire.
- El jugador detecta el suelo a través de las propias colisiones.

Todo el control del movimiento se encuentra recogido en un script llamado "Movement.cs".

### Variables
****

