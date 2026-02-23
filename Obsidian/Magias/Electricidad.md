La ultima habilidad que obtiene el jugador en el nivel de planta electrica, permite al jugador plantar un objeto en cualquier posicion del espacio jugable, permitiendole volver a esa posicion al pulsar el boton de nuevo. 

Para esto, usaremos el boton "ZL" (asumiendo un mando Nintendo Switch Pro), lo cual creara el objeto con "Instantiate Object" en caso de que no este creado. Al volver a pulsar el boton, destruiremos el objeto con "Destroy()" y pondremos al jugador en la posicion del objeto. 

# Interacciones:

Agua: Si el objeto esta creado, y el jugador se transporta a este mientras usa la magia de agua, no se cancelara la segunda, continuando el movimiento durante toda la duracion esperada.

Fuego: El jugador puede poner el objeto durante la caida, o puede volver a un punto mas alto incluso mientras cae con la magia activada. Activar la magia electrica no cancela la magia de fuego. 

Viento: El jugador puede mantener pulsado el boton de la magia de viento para tenerla activa en el momento en el que se teletransporta al objeto. 