

# 03/02/2026:

La magia de viento es la primera con la que contara el jugador al empezar el juego, y tendra la función de permitir al jugador reducir significativamente su velocidad de caida. 

Para esto, usaremos el input "ZR" (asumiendo un mando Nintendo Switch Pro). Al pulsar este boton, accederemos a las propiedades del jugador en el script ScriptableStats y modificaremos la "MaxFallSpeed", dandole un valor de 0 hasta que el jugador vuelva a presionar el boton. 

Ademas, tendra asignada la variable "usingWindMagic" que permitira reconocer si está activa o no.