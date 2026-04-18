
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
**Variables Públicas**
- Velocidad Máxima
	- Define la velocidad horizontal máxima a la que puede llegar el personaje. Se encarga de evitar que el personaje acelere indefinidamente.
- Aceleración
	- Controla la rapidez con la que el personaje alcanza la velocidad máxima.
- Desaceleración
	- Controla la rapidez con la que el personaje se detiene.
- Fuerza de salto
	- Controla la velocidad vertical que se aplica al personaje al saltar. Cuanto más alto el numero, más alto salta el personaje.
- Velocidad máxima de caída
	- Limita la velocidad a la que el personaje puede caer, evitando que caiga demasiado rápido.

**Variables Privadas**
- RigidBody2D
	- Componente que gestiona la física del personaje. Se encarga de aplicar velocidad, gravedad y colisiones automáticamente.
- Collider (CapsuleColider2D)
	- Define la forma física del personaje, lo que se usa para detectar colisiones con el entorno.
- Dirección
	- Indica hacia donde se mueve el personaje.
- Velocidad del personaje (_frameVelocity)
	- Vector que almacena la velocidad actual en X e Y antes de aplicarla al Rigidbody.  
		Permite modificar el movimiento de forma controlada cada frame.
- Grounded
	- Boolean que indica si el personaje esta tocando el suelo. Se usa para controlar el salto y controlar la gravedad.
- FrameInput
	- Es la estructura que guarda las acciones del jugador. Guarda el movimiento (Vector2), el salto (bool) y permite separar la lectura del input de la lógica del jugador.
- JumpToConsume
	- Variable que guardia si el jugador ha saltado, asegurando que no se pierde el input entre los frames.
- lastGroundedTime
	- Guarda el ultimo momento en que el jugador estaba en el suelo. Se usa para dar un pequeño margen al jugador a la hora de saltar.
- groundedGracePeriod
	- Tiempo extra que permite seguir considerando al jugador como que esta en el suelo tras perder el contacto, usado para que sea mas preciso el salto.

****
### Como funciona el código

- El script recoge el input del jugador
- Se comprueba si el personaje está en el suelo
- Si se pulsa salto y está en el suelo, el personaje salta
- Se calcula la dirección horizontal según el input
- Se aplica aceleración o desaceleración
- Se aplica la gravedad si está en el aire
- Se limita la velocidad de caída
- Se aplica la velocidad final al Rigidbody2D
- Se repite el proceso en cada frame