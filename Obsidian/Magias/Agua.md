La magia de agua, que obtendra el jugador al superar el nivel del teatro, permite al jugador hacer un impulso lateral (con un limite de uno aereo) sin perder altura, abriendo caminos diferentes asi para superar a los enemigos de formas distintas.

Para esto, usaremos el input "L" (asumiendo un mando Nintendo Switch Pro). Al pulsar este boton, añadimos al jugador un valor de 40 velocidad en su posicion X, restringiendo su posicion Y durante la duracion del dash a la vez. Al finalizar el dash reducimos la velocidad del jugador para evitar que vuelen hacia la pared y quitamos la restriccion en su posicion Y. 

Ademas tiene la variable "UsingWaterMagic" que registra si se esta usando la habilidad. 

# Interacciones: 

Viento: Si se usa la magia de agua mientras la magia de viento esta activa, el jugador se movera en la direccion del dash durante la duracion, y parara en seco una vez termine. 

Electricidad: Si el objeto esta creado, y el jugador se transporta a este mientras usa la magia de agua, no se cancelara la segunda, continuando el movimiento durante toda la duracion esperada.

Fuego: Se congela la posicion Y correctamente, lo cual permite al jugador reposicionarse rapidamente en su caida. 


****
### Variables
**Variables Públicas**
- dashPower
	- Define la fuerza del dash. A mayor numero, mas rápido se desplaza.
- dashUsed
	- Boolean que controla si ya se ha gastado el dash, evitando que se realizen varios sin parar.

**Variables Privadas**
- PlayerInput
- Movement

****
### Como funciona el código
1. Al iniciar, el script obtiene:
	- El componente Movement
	- El PlayerInput
2. En cada frame (Update):
	- Se comprueba si el jugador pulsa el botón de dash (Water)
	- Se comprueba que no haya usado el dash previamente
3. Si se cumplen las condiciones:
	- Se marca **DashUsed = true**
	- Se reproduce el sonido del dash
	- Se inicia la corrutina **DashRoutine()**
4. También en Update:
	- Si el personaje toca el suelo → **DashUsed = false**  (permite volver a usar el dash)

### Como funciona la corrutina DashRoutine()
1. Se obtiene la direccion del personaje:
	- -1 --> Izquierda.
	- 1 --> Derecha.
2. Se activa el estado usingWaterMagic.
3. Se bloquea el movimiento vertical.
	- Freeze position Y.
	- Freeze Rotation.
4. Se aplica una velocidad horizontal.
	- Velocidad = dashPower * direction.
5. Se mantiene el dash durante un tiempo determinado (0.2 segundos).
6. Se restauran las físicas normales.
7. Se desactiva el usingWaterMagic.