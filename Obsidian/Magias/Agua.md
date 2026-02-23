La magia de agua, que obtendra el jugador al superar el nivel del teatro, permite al jugador hacer un impulso lateral (con un limite de uno aereo) sin perder altura, abriendo caminos diferentes asi para superar a los enemigos de formas distintas.

Para esto, usaremos el input "L" (asumiendo un mando Nintendo Switch Pro). Al pulsar este boton, añadimos al jugador un valor de 40 velocidad en su posicion X, restringiendo su posicion Y durante la duracion del dash a la vez. Al finalizar el dash reducimos la velocidad del jugador para evitar que vuelen hacia la pared y quitamos la restriccion en su posicion Y. 

Ademas tiene la variable "UsingWaterMagic" que registra si se esta usando la habilidad. 

# Interacciones: 

Viento: Si se usa la magia de agua mientras la magia de viento esta activa, el jugador se movera en la direccion del dash durante la duracion, y parara en seco una vez termine. 

Electricidad: Si el objeto esta creado, y el jugador se transporta a este mientras usa la magia de agua, no se cancelara la segunda, continuando el movimiento durante toda la duracion esperada.

Fuego: Se congela la posicion Y correctamente, lo cual permite al jugador reposicionarse rapidamente en su caida. 


