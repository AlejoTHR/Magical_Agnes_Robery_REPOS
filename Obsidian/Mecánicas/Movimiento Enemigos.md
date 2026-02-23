***--ETAPA 1--*** *20/01/2026*
El sistema de  movimiento de enemigos funcionará de la siguiente manera dependiendo de el tipo de enemigo:

.- El enemigo de color verde funcionara mediante un script que alterara el estado del personaje siendo estos despierto y dormido, el campo de visión se activara o desactivara dependiendo del estado en el que este el enemigo, el tiempo en el que pasa cada estado se podra canviar mediante el inspector.

.- El enemigo de color blanco funcionara mediante un script con el que cada intervalo de tiempo el enemigo canviara la posicion en la que mira del ejeX, el campo de visión canviara con el enemigo, el tiempo en el que pasa mirando a cada lado se podra coanviar mediante el inspector.

.- El enemigo de color naranja funcionara mediante una animación en la que el enemigo se movera por el ejeX de manera constante con un camino fijo y se parara cada vez que llege al extremo del tramo, el enemigo movera el foco de vision con el.

.- El enemigo de color rojo funcionara mediante una animación en la que el enemigo se movera de manera por el ejeX de manera constante con un camino fijo, el enemigo movera el foco de visión con el.

***--ETAPA 2--*** *23/02/2026*
El sistema de movimiento de los enemigos funciona en base a un script.

El script pide tres variables públicas:
- Velocidad
- Límite derecho
- Límite izquierdo

El script tiene las siguientes variables privadas:
- Dirección
- Sprite Renderer

Una vez que lo tiene, el código funciona de la siguiente manera:
1) La dirección marca hacia donde se mueve el enemigo
2) El enemigo se desplaza hacia esa dirección
3) El enemigo llega al límite
4) Se invierte la dirección (1 <-> -1)
5) El sprite enemigo se invierte
6) Se reinicia el bucle