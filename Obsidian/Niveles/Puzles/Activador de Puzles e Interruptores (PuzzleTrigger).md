Este componente representa los mecanismos de activación utilizados dentro de los sistemas de puzles del escenario, tales como palancas, interruptores o dispositivos interactivos. Su función principal consiste en detectar la interacción del jugador, registrar una activación única y propagar señales hacia los distintos receptores e indicadores visuales asociados al puzle. De esta forma, actúa como el punto de origen de la cadena de eventos que permite desbloquear puertas, habilitar transiciones y actualizar el estado visual de los elementos conectados.

#### Inicialización y Acoplamiento de Dependencias

Durante el ciclo `Start()`, el componente prepara los sistemas audiovisuales responsables de representar el estado del activador.

- **Captura del Sistema de Animación:** Obtiene una referencia al componente `Animator` asociado al objeto para controlar la representación visual de la palanca o interruptor.
    
- **Configuración de Audio de Activación:** Vincula el efecto sonoro configurado (`_leverSFX`) al canal de reproducción correspondiente (`_leverSFXSource`), permitiendo emitir una confirmación auditiva cuando el mecanismo es accionado.
    

#### Sistema de Detección de Proximidad

El componente utiliza un volumen de colisión bidimensional para determinar cuándo el jugador se encuentra dentro del rango de interacción.

**1. Registro de Entrada (`OnTriggerEnter2D`)**

Cuando un objeto etiquetado como `"Player"` accede al área de influencia:

- Se registra la presencia del jugador.
    
- Se almacena una referencia al componente `PlayerInput` para consultar posteriormente las acciones disponibles.
    

**2. Registro de Salida (`OnTriggerExit2D`)**

Cuando el jugador abandona la zona de interacción, el sistema invalida la condición de proximidad, impidiendo nuevas activaciones hasta que el personaje vuelva a entrar en el área.

#### Matriz de Validación y Activación Condicional (`Update`)

Mientras el jugador permanezca dentro del área de influencia, el componente evalúa continuamente si existen las condiciones necesarias para ejecutar el mecanismo.

```
          /--> [Jugador Fuera] ------> (Sin acción)
[Update] -
          \--> [Jugador Dentro]
                       │
                       ▼
             ¿Palanca Activada?
                   │
             ┌─────┴─────┐
             │           │
            Sí          No
             │           │
             ▼           ▼
      (Sin acción)  ¿Interact?
                          │
                    ┌─────┴─────┐
                    │           │
                   No          Sí
                    │           │
                    ▼           ▼
              (Esperar) [ExecuteTrigger()]
```

**1. Validación de Proximidad**

La interacción únicamente puede producirse cuando el jugador permanece dentro del área de activación del mecanismo.

**2. Prevención de Activaciones Duplicadas**

Antes de aceptar una nueva orden, el sistema comprueba el estado de `isPulled`.

Si la palanca ya ha sido utilizada previamente, cualquier nueva interacción es descartada automáticamente.

**3. Confirmación de Interacción**

Cuando la acción `"Interact"` es detectada durante el fotograma actual, el componente inicia la secuencia de activación mediante `ExecuteTrigger()`.

#### Ejecución del Mecanismo (`ExecuteTrigger`)

Una vez validada la interacción, el sistema realiza una serie de operaciones destinadas a actualizar el estado interno del activador y comunicar el evento al resto de elementos del puzle.

```
[Interact]
      │
      ▼
[ExecuteTrigger]
      │
      ├──► Activar Estado Interno
      │
      ├──► Enviar Señales
      │
      ├──► Ejecutar Animación
      │
      ├──► Reproducir Sonido
      │
      └──► Actualizar Apariencia
```

**1. Registro Permanente de Activación**

La variable `isPulled` se establece a verdadero, marcando el mecanismo como utilizado y bloqueando futuras activaciones.

**2. Propagación de Señales**

El sistema invoca `SendSignals()`, notificando a todos los receptores e indicadores visuales vinculados al puzle.

**3. Actualización de Animación**

La animación asociada al interruptor se actualiza mediante el parámetro `IsPulled`, reflejando visualmente que la palanca ha sido accionada.

**4. Confirmación Auditiva**

El efecto sonoro configurado es reproducido inmediatamente, proporcionando retroalimentación acústica al jugador.

**5. Confirmación Visual Local**

Como medida adicional de retroalimentación, el componente modifica el color del `SpriteRenderer` local a gris, permitiendo identificar rápidamente los mecanismos ya utilizados.

#### Sistema de Propagación de Señales (`SendSignals`)

Tras ser activado, el componente distribuye la información del evento a todos los elementos conectados dentro del nivel.

```
[PuzzleTrigger]
        │
        ▼
   [SendSignals]
        │
        ├──────────────► PuzzleReceiver
        │                    │
        │                    ▼
        │            Registrar Activación
        │
        ▼
   PuzzleLightCue
        │
        ▼
 Activar Indicador
```

**1. Notificación a Receptores de Puzle**

El sistema localiza todas las instancias activas de `PuzzleReceiver` presentes en la escena.

Cada receptor recibe el identificador almacenado en `puzzleID`, permitiéndole determinar si la activación forma parte de su cadena lógica de desbloqueo.

**2. Notificación a Indicadores Luminosos**

Posteriormente, el componente localiza todos los objetos `PuzzleLightCue`.

Cada indicador recibe el valor contenido en `specificLeverID`, activando únicamente aquellos elementos visuales cuya identificación coincida con la señal enviada.

Esta separación entre `puzzleID` y `specificLeverID` permite que una misma palanca contribuya simultáneamente al progreso global del puzle y a la actualización de indicadores individuales de estado.

#### Consulta Externa del Estado de Activación

El método `IsActivated()` proporciona una interfaz pública para consultar el estado actual del activador.

Esta funcionalidad es utilizada habitualmente por sistemas auxiliares de interfaz, como indicadores contextuales de interacción, que necesitan determinar si un mecanismo continúa siendo utilizable por el jugador.

#### Garantía de Progresión Controlada

La combinación de validación por proximidad, activación única y propagación desacoplada de eventos convierte a este componente en el núcleo operativo de los sistemas de puzles basados en interruptores. Gracias a esta arquitectura, cada activador puede comunicar su estado a múltiples receptores e indicadores simultáneamente, manteniendo una estructura modular que facilita la creación de puzles complejos sin dependencias directas entre los distintos elementos del escenario.