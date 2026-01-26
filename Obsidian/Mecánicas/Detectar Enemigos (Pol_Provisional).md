###### ***--ETAPA 1--*** *20/01/2026*
El sistema de detectar enemigos funcionara de la siguiente manera:

Se dibujara un circulo con su centro en las coordenadas (X,Y,Z) exactas del objeto que lo requiera. Este contendra un script el cual dibujara un cono de vision ajustable para los requerimientos necesarios con las variables "Angle" y "Radius".

"Angle" dictara lo amplia que es la vision del enemigo, mientras que "Radius" dictara lo lejos que puede ver.

![[Cono_Detección.png]]

###### ***--ETAPA 2--*** *25/01/2026*
Se han divido en 2 scripts los controles del cono de vision de los enemigos: 

1. El script que controla el funcionamiento del cono y la deteccion del jugador.
2. El script que permite editar y observar toda la informacion relacionada al cono desde el mismo editor de unity, permitiendo que los desarolladores podamos editarlo a nuestra conveniencia segun la circunstancia (Cada instancia del script funcionara independientemente de las otras).

Ahora se procedera a explicar ambos script individualmente, detallando el codigo y la funcion que tiene este mismo.

***PARTE 1 - CONO DE VISION***

Antes de iniciar ninguna funcion, declaramos una serie de variables para poder editar las difrentes partes que conforman el cono:

- viewRadius (La distancia en la que puede ver el enemigo).
- viewAngle (El angulo de vision que tiene el enemigo respecto al circulo que conforma el objeto)
- fovRotation (En relacion con el angulo, permite que el enemigo pueda ver en diferentes direcciones sin tener que cambiar el angulo)

Ademas, se declara tambien el Mesh (y sus partes) con el que se pinta el cono de vision en el mapa de forma publica, lo cual nos permite cambiar su color, textura y transparencia.

Por ultimo, declaramos una ultima variable booleana que nos permitira ver si el jugador es detectado por el cono de vision.

La primera funcion, "void Start" consta de dos partes: Primero, inicializa el mesh, dandole un nombre y propiedades acordes. Segundo, inicia la segunda funcion del script.

La segunda funcion "Enumerator FindTargetsWithDelay(float delay)", simplemente pausa las funciones durante un cierto numero de segundos y llama la funcion "FindVisibleTargets()"

La tercera funcion, "void LateUpdate", simplemente llama a la funcion "DrawFieldOfView" cada frame.

La cuarta funcion, "void FindVisibleTargets()", llamada por "FindTargetsWithDelay", usa OverlapCircleAll de la libreria Physics2D para generar el circulo de vision de los enemigos, calcula el angulo en el que se tiene que detectar al jugador, y comprueba si colosiona la hitbox con este.  

