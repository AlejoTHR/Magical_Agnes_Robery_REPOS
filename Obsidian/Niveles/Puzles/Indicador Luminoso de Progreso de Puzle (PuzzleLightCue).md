Este componente proporciona una señalización visual persistente para comunicar el estado de activación de mecanismos asociados a puzles del entorno. Su función principal consiste en modificar la representación gráfica de un indicador luminoso cuando recibe una señal procedente de un elemento activador compatible, permitiendo al jugador identificar de forma inmediata qué interruptores, palancas o mecanismos han sido accionados correctamente.

#### Inicialización y Vinculación Visual

Durante el ciclo `Awake()`, el componente prepara el sistema gráfico responsable de representar el estado del indicador luminoso.

- **Captura del Renderizador Local:** Obtiene una referencia al componente `SpriteRenderer` asociado al objeto mediante una llamada a `GetComponent`.
    
- **Estado Inicial Apagado:** Como medida de consistencia visual, el sistema asigna automáticamente el sprite definido en `spriteOff`, garantizando que todos los indicadores comiencen la partida en estado inactivo.
    

```
[Inicio de Escena]
        │
        ▼
[Obtener SpriteRenderer]
        │
        ▼
[Asignar spriteOff]
        │
        ▼
[Indicador Apagado]
```

#### Sistema de Identificación y Emparejamiento

El componente utiliza una cadena de texto almacenada en `lightID` como mecanismo de asociación lógica entre el indicador visual y los distintos elementos interactivos del escenario.

Esta identificación permite que múltiples indicadores coexistentes dentro del mismo nivel respondan únicamente a las señales que les corresponden, evitando activaciones cruzadas entre puzles independientes.

Por ejemplo:

```
Lever_01  →  Light_01
Lever_02  →  Light_02
Lever_03  →  Light_03
```

Cada indicador reaccionará exclusivamente a los eventos que compartan su mismo identificador.

#### Recepción de Señales de Activación (`ActivateLight`)

Cuando un sistema externo, normalmente un `PuzzleTrigger`, notifica una activación, el método `ActivateLight()` recibe el identificador asociado al evento y evalúa su correspondencia con el identificador local del indicador.

```
[Recibir ID]
      │
      ▼
	¿ID Coincide?
	   │       │
	  No       Sí
	   │       │
	   ▼       ▼
[Sin Acción] [Activar Luz]
```

**1. Validación de Identificador**

El sistema compara el valor recibido (`incomingID`) con el identificador configurado en `lightID`.

Si ambos valores son diferentes, el método finaliza inmediatamente sin modificar el estado visual del objeto.

**2. Activación del Indicador**

Si la coincidencia es satisfactoria, el componente sustituye el sprite actual por la representación definida en `spriteOn`.

```
sr.sprite = spriteOn;
```

Esta operación produce un cambio visual inmediato que informa al jugador de que el mecanismo asociado ha sido activado correctamente.

#### Función como Elemento de Retroalimentación Ambiental

A diferencia de otros sistemas de interfaz temporal, este componente no genera ventanas emergentes ni indicadores contextuales. Su comportamiento está orientado a proporcionar una referencia visual permanente dentro del escenario, permitiendo al jugador consultar el progreso de resolución del puzle incluso cuando se encuentra alejado del mecanismo que originó la activación.

Esta estrategia resulta especialmente útil en estructuras con múltiples interruptores, puertas remotas o secuencias de activación distribuidas por distintas áreas del nivel.

#### Garantía de Consistencia Visual

La utilización de identificadores únicos y la validación previa de todas las señales recibidas aseguran que cada indicador responda únicamente a los eventos para los que fue diseñado. Gracias a esta arquitectura desacoplada, el sistema puede escalar fácilmente a múltiples puzles simultáneos sin generar dependencias directas entre los elementos activadores y los elementos de señalización visual presentes en el escenario.