
Este componente actúa como receptor lógico dentro de un sistema de puzles basado en activadores distribuidos por el escenario. Su función principal consiste en supervisar el progreso de resolución asociado a un identificador específico, desbloqueando una puerta o mecanismo cuando se alcanza el número requerido de activaciones. Una vez completadas las condiciones del puzle, el sistema habilita además la transición del jugador hacia la siguiente sala o sección del nivel.

#### Inicialización y Configuración del Estado

Durante el ciclo `Start()`, el componente prepara el estado inicial del receptor y evalúa si la estructura asociada debe comenzar la partida bloqueada o disponible.

- **Captura del Sistema de Animación:** Obtiene una referencia al componente `Animator` local encargado de representar visualmente el estado de apertura de la puerta o mecanismo asociado.
    
- **Validación de Requisitos de Activación:** Si el valor configurado en `leversNeeded` es igual o inferior a cero, el sistema interpreta que no existen condiciones previas para el desbloqueo.
    
- **Desbloqueo Automático:** En ausencia de requisitos, la estructura pasa inmediatamente a estado operativo y se ejecuta la animación de apertura correspondiente.
    

```
[Inicio]
    │
    ▼
¿Levers Needed > 0?
     │
 ┌───┴───┐
 │       │
Sí      No
 │       │
 ▼       ▼
[Bloqueada] [Desbloquear]
```

#### Sistema de Registro de Activaciones

El método `RegisterLeverActivation()` constituye el punto de entrada principal para las señales emitidas por los distintos activadores del puzle.

Cada vez que un mecanismo externo comunica una activación, el componente verifica que el identificador recibido corresponda al identificador configurado en `puzzleID`.

```
[Recibir Activación]
          │
          ▼
 ¿ID Coincide?
      │
 ┌────┴────┐
 │         │
No        Sí
 │         │
 ▼         ▼
[Ignorar] [Contabilizar]
```

**1. Validación de Identificador**

La activación únicamente es aceptada si el valor recibido coincide con el identificador lógico asociado al receptor.

Este mecanismo permite que múltiples puzles coexistentes dentro del mismo nivel funcionen de forma independiente sin interferencias entre ellos.

**2. Registro de Progreso**

Si la validación es satisfactoria, el sistema incrementa internamente el contador `currentLeversActivated`, registrando el avance del jugador dentro de la secuencia de resolución.

**3. Verificación de Objetivo Completado**

Tras cada incremento, el componente compara el número de activaciones registradas con el requisito establecido en `leversNeeded`.

Cuando ambas cantidades coinciden o son superadas, el receptor inicia el procedimiento de desbloqueo.

#### Proceso de Desbloqueo (`UnlockDoor`)

Una vez alcanzado el número requerido de activaciones, el sistema modifica permanentemente el estado del receptor.

```
[Activaciones Alcanzadas]
            │
            ▼
     [UnlockDoor]
            │
            ▼
 [isLocked = false]
            │
            ▼
 [Animación Apertura]
```

**1. Liberación del Mecanismo**

La variable interna `isLocked` se establece a `false`, indicando que las condiciones del puzle han sido satisfechas correctamente.

**2. Actualización Visual**

Si existe un componente `Animator` asociado, el parámetro encargado de controlar la apertura es activado inmediatamente.

Esta transición proporciona una confirmación visual directa del éxito del jugador al completar el puzle.

#### Sistema de Interacción con el Jugador

Una vez desbloqueada la estructura, el componente habilita una segunda fase de interacción destinada a permitir el avance hacia la siguiente zona del nivel.

Durante los eventos `OnTriggerEnter2D` y `OnTriggerExit2D`, el sistema monitoriza la presencia física del jugador dentro del área de influencia del receptor.

- **Detección de Entrada:** Al ingresar en la zona de interacción, se registra la presencia del jugador y se almacenan referencias a los componentes `PlayerInput` y `Movement`.
    
- **Detección de Salida:** Cuando el jugador abandona el área, la interacción queda automáticamente deshabilitada.
    

#### Validación y Disparo de la Transición (`Update`)

Mientras el jugador permanezca dentro del área de interacción, el componente evalúa continuamente si existen condiciones válidas para efectuar el cambio de sala.

```
          /--> [Puerta Bloqueada] --> (Sin acción)
[Jugador Dentro]
          |
          +--> [Interacción No Pulsada] --> (Esperar)
          |
          \--> [Puerta Desbloqueada + Interact]
                           │
                           ▼
                 [Load Next Room]
```

**1. Validación de Estado del Receptor**

La interacción únicamente puede ejecutarse cuando el receptor ha sido desbloqueado previamente.

Si la estructura continúa cerrada, cualquier intento de interacción es ignorado.

**2. Detección de Entrada del Jugador**

El sistema requiere que el jugador permanezca físicamente dentro del área de activación definida por el colisionador del objeto.

**3. Confirmación de Interacción**

Cuando se detecta una pulsación de la acción `"Interact"` durante el mismo fotograma, el componente interpreta la orden como una solicitud de avance.

**4. Inmovilización Preventiva**

Antes de iniciar la transición, la velocidad lineal del personaje es anulada para evitar desplazamientos residuales durante el cambio de sala.

**5. Transferencia de Zona**

Finalmente, el sistema invoca el método `LoadNextRoom()` del gestor de niveles, completando la transición hacia la siguiente sección del escenario.

#### Consulta Externa del Estado de Desbloqueo

El método `IsUnlocked()` proporciona una interfaz pública para que otros componentes puedan consultar el estado actual del receptor.

Esta funcionalidad es utilizada habitualmente por sistemas auxiliares de interfaz o indicadores de interacción que necesitan determinar si una puerta o mecanismo puede ser utilizado por el jugador.

#### Garantía de Progresión Controlada

La combinación de validación por identificadores, conteo acumulativo de activaciones y control explícito de interacción permite que el receptor funcione como un punto de progresión seguro dentro del flujo del nivel. Gracias a esta arquitectura, el acceso a nuevas salas queda estrictamente condicionado a la resolución previa de los puzles asociados, garantizando una progresión coherente y evitando accesos prematuros a contenido no desbloqueado.