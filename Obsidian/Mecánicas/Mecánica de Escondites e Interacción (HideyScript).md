Este componente gestiona las zonas de interacción físicas del entorno (como rejillas de ventilación o armarios) que permiten a Agnes ocultarse de las amenazas. Actúa modificando directamente el comportamiento del controlador de movimiento base, alterando las propiedades visuales del jugador y conmutando los estados de colisión del motor físico.

#### Inicialización y Localización de Dependencias
Durante el ciclo `Start()`, el script implementa un sistema automatizado de asignación por si las referencias no se han configurado manualmente en el Inspector de Unity:
* **Búsqueda Dinámica del Jugador:** Si el cuerpo rígido de Agnes (`agnes`) es nulo, el script localiza el objeto en la escena mediante la etiqueta `"Player"` y extrae en cascada los componentes esenciales: `Rigidbody2D`, `Movement`, `SpriteRenderer` y `PlayerInput`.
* **Inicialización Visual:** Comprueba el renderizador de la entidad del escondite (`spotSpriteRenderer`) y fuerza la asignación del asset gráfico neutro (`emptySprite`) para inicializar el objeto con su apariencia vacía de forma predeterminada.

#### Bucle de Conmutación y Forzado de Posición
El ciclo de actualización lógica (`Update`) monitoriza el estado de la entrada del jugador y ancla la posición del personaje si se encuentra oculto:
1. **Validación de Interacción:** Si el personaje está en rango (`playerInRange`) y presiona la acción `"Interact"` registrada por el mapa de entrada (`InputAction.WasPressedThisFrame()`), el sistema evalúa el booleano global de locomoción `_move.isHiding` para alternar entre los métodos `Hide()` y `Unhide()`.
2. **Restricción Cinemática Absoluta:** Mientras Agnes permanezca en el estado de escondido, el script sobreescribe de forma continua su posición para igualarla al centro geométrico del escondite (`hidespot.transform.position`). Además, se inyecta una máscara de restricciones físicas combinada al `Rigidbody2D`:
   $$\text{Constraints} = \text{FreezePosition} \mid \text{FreezeRotation}$$
   Esto anula por completo la velocidad residual y evita que fuerzas externas o simulaciones de colisión desplacen al jugador del punto de ocultamiento.

#### Lógica de Entrada al Escondite (Hide)
Al activarse la acción de ocultarse, el componente realiza una serie de modificaciones estructurales sobre la entidad del jugador y del propio entorno:
* **Invisibilidad y Permisividad Física:** Establece la bandera global `_move.isHiding = true` (lo que frena la velocidad horizontal en el script de locomoción base). Desactiva el renderizador del sprite de Agnes (`playerSprite.enabled = false`) y transforma su colisionador capsular en un disparador volumétrico (`disableCollision.isTrigger = true`) para evitar colisiones físicas con enemigos o proyectiles circundantes.
* **Feedback Visual y Acústico:** Sustituye el sprite del escondite por el asset correspondiente a ocupado (`occupiedSprite`) y ejecuta un efecto sonoro instantáneo (`interactSound`) mediante el uso del método `PlayOneShot` en el componente `AudioSource` local.

#### Salida del Escondite y Restauración (Unhide)
Cuando el jugador decide abandonar el escondite (o si es forzado a salir), el sistema revierte todas las alteraciones del motor gráfico y físico:
* **Restablecimiento del Estado:** Devuelve la bandera `_move.isHiding` a `false`, restaura la tangibilidad de la cápsula física (`isTrigger = false`) y vuelve a activar el renderizador visual del personaje (`playerSprite.enabled = true`).
* **Liberación Cinética:** Se limpian las restricciones de posición del `Rigidbody2D` del personaje, dejando únicamente activo el bloqueo estándar de rotación de la locomoción (`FreezeRotation`). Finalmente, el renderizador del escondite regresa a la apariencia de vacío (`emptySprite`).

#### Gestión del Rango de Proximidad y Salida Forzada
El script delimita la zona interactiva mediante eventos de disparo bidimensionales (`Collider2D`):
* **Detección de Entrada:** Al interceptar el volumen con la etiqueta `"Player"`, se activa la bandera `playerInRange = true`.
* **Detección de Salida:** Si el jugador abandona el volumen físico de detección del disparador, la bandera cambia a falso (`playerInRange = false`). Si por algún desfase en las físicas el personaje sale del volumen manteniendo el estado oculto, el script invoca de manera automática el método `Unhide()` como medida de seguridad para evitar bloqueos lógicos en el bucle del juego.