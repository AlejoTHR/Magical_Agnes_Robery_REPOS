La magia de viento es la primera con la que contara el jugador al empezar el juego, y tiene la función de permitir al jugador reducir significativamente su velocidad de caida, sin embargo tambien reduciendo su velocidad horizontal. 

Para esto, usaremos el input "ZR" (asumiendo un mando Nintendo Switch Pro). Al pulsar este boton, accedemos a las propiedades del jugador en el script ScriptableStats y modificamos la "MaxFallSpeed" y la "MaxSpeed", dandoles un valor de 0 hasta que el jugador vuelva a presionar el boton. 

Ademas, tiene asignada la variable "usingWindMagic" que permite reconocer si está activa o no.

# Interacciones:

Agua: Si se usa la magia de agua mientras la magia de viento esta activa, el jugador se movera en la direccion del dash durante la duracion, y parara en seco una vez termine. 

Electricidad: El jugador puede mantener pulsado el boton de la magia de viento para tenerla activa en el momento en el que se teletransporta al objeto. 

Fuego: La magia de viento toma prioridad sobre la de fuego, pausando la caida del jugador mientras esta activa y permitiendo que este se suspenda en el aire. 