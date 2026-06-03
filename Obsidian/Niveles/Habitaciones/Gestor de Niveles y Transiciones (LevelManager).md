Este componente constituye el núcleo de administración de progresión del juego, siendo responsable de la carga dinámica de salas, la gestión de transiciones visuales, el control de reinicios tras la muerte del jugador y la coordinación del avance entre niveles. Su propósito principal es garantizar que cada cambio de estado —ya sea una transición entre habitaciones, una reaparición tras una derrota o el acceso a una nueva escena— se produzca de forma controlada, manteniendo la consistencia de los sistemas de juego y evitando estados residuales no deseados.

#### Inicialización y Preparación del Entorno

Durante el ciclo `Awake()`, el componente establece las referencias fundamentales necesarias para el funcionamiento del sistema de progresión.

- **Registro de Instancia Global:** Almacena una referencia estática accesible mediante `Instance`, permitiendo que otros sistemas soliciten cambios de sala o reinicios desde cualquier punto de la ejecución.
    
- **Configuración de Sistemas de Transición:** Inicializa los animadores encargados de gestionar las transiciones estándar y las secuencias específicas de muerte.
    
- **Preparación de Canales de Audio:** Obtiene una referencia al sistema de reproducción utilizado para emitir efectos sonoros asociados a la reaparición del jugador.
    

Posteriormente, durante `Start()`, el gestor instancia la primera sala disponible de la lista configurada y evalúa si existe una transición pendiente procedente de una escena anterior.

#### Carga Inicial de Sala

Cuando la escena comienza, el sistema genera dinámicamente la primera habitación configurada dentro de `roomPrefabs`.

```
[Inicio de Escena]
        │
        ▼
 [LoadRoom(0)]
        │
        ▼
[Instanciar Sala]
        │
        ▼
[Posicionar Jugador]
```

Si la escena ha sido cargada desde otro nivel mediante una transición previa, el componente ejecuta adicionalmente una secuencia de entrada visual para suavizar la aparición del jugador.

#### Sistema de Transición entre Habitaciones

El método `LoadNextRoom()` constituye el mecanismo principal utilizado por puertas, receptores de puzle y otros sistemas de progresión para solicitar el avance a la siguiente sección del nivel.

```
[Solicitud de Avance]
           │
           ▼
 ¿Existen Más Salas?
      │
 ┌────┴─────┐
 │          │
Sí         No
 │          │
 ▼          ▼
[Cambiar] [Cambiar Escena]
```

**1. Validación de Estado**

Antes de iniciar cualquier transición, el sistema comprueba que no exista otra operación en curso mediante la variable `isTransitioning`.

**2. Incremento de Índice**

Si la solicitud es válida, el gestor avanza al siguiente elemento de la lista de habitaciones configuradas.

**3. Determinación de Destino**

Dependiendo de si aún quedan salas disponibles:

- Se inicia una transición interna dentro del mismo nivel.
    
- Se ejecuta una transferencia completa hacia una nueva escena.
    

#### Secuencia de Cambio de Habitación (`InternalRoomTransition`)

Cuando el jugador progresa entre habitaciones pertenecientes al mismo nivel, el sistema ejecuta una secuencia controlada destinada a ocultar la transición física entre espacios.

```
[Iniciar Transición]
          │
          ▼
[Desactivar Control]
          │
          ▼
[Ocultar Interfaz]
          │
          ▼
[Fade In]
          │
          ▼
[Recargar Sala]
          │
          ▼
[Fade Out]
          │
          ▼
[Restaurar Control]
```

**1. Bloqueo de Entrada del Jugador**

El componente deshabilita temporalmente el sistema de movimiento para impedir acciones durante el proceso.

**2. Ocultamiento de Interfaz**

Todos los elementos de interacción visual son ocultados para evitar inconsistencias durante la carga.

**3. Transición Visual de Salida**

El animador estándar ejecuta la animación configurada en `_hideScreenAnim`, oscureciendo progresivamente la pantalla.

**4. Reinicialización del Estado del Jugador**

Antes de generar la nueva sala, el sistema elimina cualquier estado residual asociado a habilidades, animaciones o desplazamientos.

**5. Instanciación de la Nueva Habitación**

La sala actual es destruida y sustituida por la nueva instancia correspondiente.

**6. Transición Visual de Entrada**

Una vez completada la carga, el animador reproduce `_showScreenAnim`, devolviendo progresivamente la visibilidad al jugador.

**7. Restauración Operativa**

Finalmente, la interfaz vuelve a mostrarse y el control del personaje es restablecido.

#### Secuencia de Reaparición tras la Muerte (`DeathSequence`)

Cuando el jugador pierde una vida o entra en un estado de derrota, el sistema ejecuta una transición especializada diseñada para diferenciar visualmente la reaparición de una transición convencional.

```
[Muerte]
    │
    ▼
[Desactivar Control]
    │
    ▼
[Animación DeathEntry]
    │
    ▼
[Reproducir SFX]
    │
    ▼
[Reiniciar Sala]
    │
    ▼
[Animación DeathExit]
    │
    ▼
[Recuperar Control]
```

**1. Sustitución de Sistemas Visuales**

El animador estándar es temporalmente desactivado para ceder el control al animador especializado de muerte.

**2. Ejecución de Secuencia de Derrota**

Se reproduce la animación de entrada correspondiente al estado de muerte.

**3. Retroalimentación Sonora**

Durante el proceso se reproduce el efecto sonoro definido en `_deathSfx`, reforzando la percepción del evento.

**4. Restauración de Escenario**

La sala actual es reconstruida desde cero mediante una nueva instancia.

**5. Secuencia de Reaparición**

Tras completar la carga, se ejecuta la animación de salida asociada a la recuperación del jugador.

**6. Retorno al Estado Normal**

El sistema restaura el animador estándar y devuelve el control completo al usuario.

#### Transición entre Escenas (`SceneTransitionSequence`)

Cuando el jugador alcanza la última habitación disponible del nivel actual, el gestor inicia una transición hacia la siguiente escena configurada en el proyecto.

```
[Última Sala]
      │
      ▼
[Fade In]
      │
      ▼
[Guardar Estado de Entrada]
      │
      ▼
[LoadScene()]
```

**1. Bloqueo de Control**

El jugador pierde temporalmente la capacidad de movimiento para garantizar la estabilidad de la transición.

**2. Oscurecimiento de Pantalla**

El animador estándar ejecuta la secuencia de cierre visual.

**3. Persistencia de Estado**

La variable estática `_shouldFadeOutOnArrival` es activada para informar a la siguiente escena de que debe reproducir una animación de entrada al cargarse.

**4. Carga de Nueva Escena**

Finalmente se ejecuta la transferencia hacia la siguiente escena registrada en el orden de compilación.

#### Sistema de Generación de Habitaciones (`LoadRoom`)

Cada vez que se solicita una nueva sala, el gestor realiza una sustitución completa del entorno actual.

**1. Eliminación de la Sala Anterior**

La instancia previamente activa es destruida para liberar recursos y evitar duplicidades.

**2. Instanciación del Nuevo Entorno**

Se genera la habitación correspondiente utilizando el prefab indicado por el índice actual.

**3. Reposicionamiento del Jugador**

El personaje es desplazado automáticamente al punto denominado `EntranceSpawnPoint`, garantizando una entrada coherente en cada nueva zona.

#### Reinicialización Integral del Jugador (`ResetPlayerState`)

Como medida de seguridad, el componente ejecuta una restauración exhaustiva del estado interno del personaje antes de cada recarga.

Esta operación incluye:

- Reinicio de las magias activas.
    
- Cancelación de velocidades residuales.
    
- Restauración de animaciones al estado de reposo.
    
- Reinicialización de habilidades especiales.
    
- Recuperación de estadísticas modificadas temporalmente.
    
- Restablecimiento de variables internas utilizadas por los distintos sistemas de movimiento.
    

De esta forma se evita que efectos temporales o estados excepcionales persistan entre habitaciones o tras una reaparición.

#### Gestión de Control e Interfaz

Como soporte auxiliar, el gestor incorpora mecanismos destinados a controlar la interacción del jugador durante las transiciones.

**1. Control de Movimiento (`TogglePlayerControl`)**

Permite habilitar o deshabilitar temporalmente el componente de movimiento del personaje.

**2. Gestión Global de Interfaz (`SetGlobalUIAlpha`)**

Localiza todos los elementos etiquetados como `"InteractionUI"` y modifica su opacidad y capacidad de interacción.

Esta funcionalidad evita que indicadores contextuales o elementos flotantes permanezcan visibles durante las secuencias de transición.

#### Garantía de Consistencia de Progresión

La combinación de gestión dinámica de habitaciones, reinicialización integral de estados, control de transiciones audiovisuales y supervisión de la progresión convierte a este componente en el eje central de navegación del juego. Gracias a esta arquitectura, el avance entre salas, la recuperación tras la muerte y el acceso a nuevos niveles se realizan de forma uniforme, garantizando que cada transición preserve la coherencia visual, lógica y mecánica de la experiencia de juego.