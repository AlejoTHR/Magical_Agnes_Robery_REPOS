Este componente administra la activación contextual de cámaras virtuales asociadas a las distintas habitaciones del nivel. Su función principal consiste en detectar la entrada del jugador en una sala y otorgar prioridad a la cámara correspondiente, permitiendo que el sistema de seguimiento visual se adapte automáticamente a la zona actualmente explorada. De esta forma, cada habitación puede disponer de una configuración de encuadre independiente sin necesidad de gestionar manualmente los cambios de cámara.

#### Inicialización y Vinculación del Objetivo de Seguimiento

Durante el ciclo `Start()`, el componente establece la relación entre la cámara virtual de la sala y el personaje controlado por el jugador.

- **Localización del Jugador:** El sistema busca en la escena el objeto identificado con la etiqueta `"Player"`.
    
- **Asignación de Objetivo de Seguimiento:** Si la búsqueda resulta satisfactoria, la referencia del jugador se asigna al parámetro `Follow` de la cámara virtual.
    
- **Preparación de la Cámara de Sala:** Esta configuración garantiza que la cámara disponga de un objetivo válido desde el momento en que sea activada por el sistema de prioridades.
    

```
[Inicio de Sala]
       │
       ▼
[Buscar Player]
       │
       ▼
¿Encontrado?
    │      │
   No      Sí
    │      │
    ▼      ▼
 (Fin) [Asignar Follow]
```

#### Sistema de Activación por Proximidad

El componente utiliza un `PolygonCollider2D` configurado como área de influencia para determinar cuándo una sala debe convertirse en la región activa del escenario.

Cuando el jugador atraviesa el volumen asociado al controlador, el sistema interpreta que dicha habitación pasa a ser la zona principal de exploración.

```
[Jugador Entra]
        │
        ▼
[OnTriggerEnter2D]
        │
        ▼
 ¿Tag = Player?
        │
    ┌───┴───┐
    │       │
   No      Sí
    │       │
    ▼       ▼
 (Ignorar) [ActivateRoom()]
```

#### Gestión de Prioridades Cinematográficas (`ActivateRoom`)

La activación de una sala se realiza mediante la modificación de la prioridad asignada a su cámara virtual.

**1. Validación de Cámara**

Antes de efectuar cualquier modificación, el componente verifica que exista una referencia válida a un objeto `CinemachineCamera`.

**2. Incremento de Prioridad**

Si la cámara está disponible, el sistema asigna el valor configurado en `activePriority`.

```
[Cámara de Sala]
        │
        ▼
[Asignar Prioridad]
        │
        ▼
[Cinemachine Selecciona Cámara Activa]
```

**3. Sustitución Automática de Vista**

El sistema Cinemachine evalúa continuamente las prioridades de todas las cámaras virtuales presentes en la escena. Como consecuencia, la cámara de la sala recientemente activada pasa a convertirse en la vista principal, desplazando automáticamente cualquier cámara previamente dominante.

#### Integración con la Arquitectura de Habitaciones

Este componente está diseñado para funcionar como parte de una estructura basada en salas independientes, donde cada habitación dispone de:

- Un área de detección propia.
    
- Una cámara virtual asociada.
    
- Parámetros de encuadre específicos para su geometría.
    

Gracias a este enfoque, cada zona del nivel puede controlar de forma autónoma su comportamiento visual, permitiendo crear espacios con diferentes dimensiones, márgenes de seguimiento o configuraciones de composición sin modificar la lógica global de cámara.

#### Garantía de Transición Visual Consistente

La combinación de detección espacial y gestión de prioridades permite que los cambios de encuadre se produzcan de forma automática y transparente para el jugador. Gracias a esta arquitectura, la cámara permanece siempre sincronizada con la sala activa, manteniendo una representación visual coherente de la exploración y facilitando la navegación entre las distintas áreas del escenario.