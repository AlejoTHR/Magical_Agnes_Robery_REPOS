La magia de fuego, la segunda que consigue el jugador tras pasar el nivel de fabrica, consiste en una forma de "caida rapida", que permite al jugador traversar espacios verticales de manera mas agil. 

Para esto, usaremos el input "R" (asumiendo un mando Nintendo Switch Pro). Al pulsar este boton, accedemos a las propiedades del jugador en el script ScriptableStats y modificamos la "MaxFallSpeed", dandole un valor de 200 hasta que el jugador vuelva a pulsar el boton. 

Ademas, tiene asignada la variable "usingFireMagic" que permite reconocer si está activa o no.

# Interacciones:

Agua: Se congela la posicion Y correctamente, lo cual permite al jugador reposicionarse rapidamente en su caida. 

Electricidad: El jugador puede poner el objeto durante la caida, o puede volver a un punto mas alto incluso mientras cae con la magia activada. Activar la magia electrica no cancela la magia de fuego. 

Viento: La magia de viento toma prioridad sobre la de fuego, pausando la caida del jugador mientras esta activa y permitiendo que este se suspenda en el aire. 