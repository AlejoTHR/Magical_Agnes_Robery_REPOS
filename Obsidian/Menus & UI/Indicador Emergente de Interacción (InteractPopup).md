Este componente gestiona la retroalimentación visual del entorno bidimensional mediante la activación y desactivación dinámica de elementos de la interfaz de usuario (*Popups*). Su propósito principal es notificar al jugador cuándo un elemento del escenario (como palancas, cofres o puertas estructurales) es susceptible de recibir una orden de interacción, condicionando su despliegue al estado interno de los sistemas lógicos vinculados.

#### Inicialización y Acoplamiento de Dependencias
Durante el ciclo `Start()`, el componente inicializa la interfaz de usuario para prevenir la superposición de ventanas flotantes en el mapa y realiza un escaneo de componentes locales para determinar la naturaleza del objeto interactivo:
* **Ocultamiento de Seguridad:** Fuerza el apagado inmediato del contenedor gráfico (`popupUI.SetActive(false)`).
* **Detección de Interfaces de Puzle:** Intenta capturar las referencias de los scripts `PuzzleTrigger` (asociado a interruptores o palancas de activación) y `PuzzleReceiver` (asociado a receptores o puertas bloqueadas) coexistentes en el mismo objeto rígido mediante llamadas directas a `GetComponent`.

#### Matriz de Validación y Disparo Condicional (OnTriggerEnter2D)
Cuando el colisionador volumétrico del jugador (Agnes) intersecta el área de influencia del disparador físico bidimensional (`OnTriggerEnter2D`), el script evalúa las condiciones lógicas de los componentes detectados antes de conmutar la visibilidad del indicador:

```
  /---> [PuzzleTrigger Activo] ---> (Retorno sin acción)
[Player Enter] --> Evaluar Estados? 
  \---> [PuzzleReceiver Bloqueado] -> (Retorno sin acción)
   ||
   \/
[popupUI.SetActive(true)]
```

 **1. Filtro de Estado de Activación (`PuzzleTrigger`):** Si el objeto interactivo actúa como un disparador (por ejemplo, una palanca mecánica) y su estado interno ya ha sido conmutado a verdadero (`puzzleTrigger.IsActivated()`), el método interrumpe su ejecución de forma prematura mediante un retorno vacío (`return`), evitando mostrar un indicador redundante en un elemento ya resuelto.

**2. Filtro de Estado de Desbloqueo (`PuzzleReceiver`):** Si el objeto actúa como un receptor físico (por ejemplo, una compuerta o mecanismo cerrado) y la bandera de apertura permanece bloqueada (`!puzzleReceiver.IsUnlocked()`), el script aborta la operación. Esto asegura que la interfaz de interacción solo sea accesible si el puzle o la cerradura elemental previa han sido resueltos de forma satisfactoria.

**3. Despliegue Visual:** Si el objeto supera las salvaguardas estructurales previas y la referencia del panel de la interfaz de usuario es válida, el elemento gráfico se activa de forma instantánea (`popupUI.SetActive(true)`).

#### Clausura Automática por Desplazamiento (OnTriggerExit2D)

Como directiva de limpieza visual y gestión de proximidad, el método `OnTriggerExit2D` monitoriza la desvinculación física de las entidades en el escenario. En el momento exacto en que el colisionador con la etiqueta `"Player"` abandona el umbral del trigger, el script extingue la visibilidad del panel de interacción (`popupUI.SetActive(false)`), garantizando que el HUD de Agnes permanezca limpio de elementos flotantes mientras se desplaza por el nivel.