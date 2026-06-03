Este componente coordina la operativa de la pantalla de finalización del juego, gestionando la navegación de la interfaz de usuario, la reproducción de efectos sonoros asociados a la interacción del jugador y la ejecución de transiciones visuales previas al abandono de la secuencia final. Su propósito principal es proporcionar una salida controlada desde la pantalla de victoria, sincronizando los elementos audiovisuales de la interfaz con la carga de la siguiente escena o retorno al menú principal.

#### Inicialización y Preparación del Entorno

Durante los ciclos `Awake()` y `Start()`, el componente configura los subsistemas necesarios para el funcionamiento de la interfaz final y establece el estado visual inicial de la pantalla.

- **Registro de Instancia Global:** Almacena una referencia estática accesible desde otros elementos de la interfaz mediante la asignación de `Instance`.
    
- **Creación de Canales de Audio Dedicados:** Genera dos fuentes de audio independientes destinadas a la reproducción de efectos de navegación (_Hover_) y confirmación (_Click_), evitando interferencias entre ambos tipos de eventos sonoros.
    
- **Asignación de Foco Inicial:** Si existe un `EventSystem` activo, el sistema selecciona automáticamente el botón principal configurado (`singularButton`), garantizando la compatibilidad inmediata con dispositivos de entrada alternativos como teclado o mando.
    
- **Transición de Entrada:** Ejecuta la animación definida en `_showScreenAnim`, utilizada para revelar progresivamente la pantalla final una vez concluida la experiencia de juego.
    

#### Gestión Centralizada de Retroalimentación Sonora

El componente actúa como un concentrador de eventos acústicos para la interfaz de cierre, coordinando la reproducción de sonidos asociados a la navegación y confirmación de opciones.

**1. Reproducción de Sonidos de Navegación (`PlayHoverSound`)**

Cuando el usuario desplaza el foco entre los elementos interactivos de la pantalla, el sistema verifica que no exista una transición activa ni una reproducción de confirmación en curso. Si las condiciones son válidas, el sonido solicitado se reproduce inmediatamente mediante el canal dedicado a la navegación.

**2. Reproducción de Sonidos de Confirmación (`UI_PlayClick`)**

Al seleccionar una opción disponible, el sistema interrumpe cualquier reproducción secundaria y ejecuta el efecto sonoro global de confirmación configurado para la interfaz.

**3. Corte Temporal de Seguridad**

Con el objetivo de evitar que reproducciones prolongadas interfieran con la secuencia de salida, el sonido de confirmación es detenido automáticamente tras un intervalo de 0,4 segundos mediante una llamada programada a `StopClickAudio()`.

#### Secuencia de Clausura y Abandono de la Pantalla Final

Cuando el jugador confirma una acción desde la pantalla de victoria, el sistema inicia una secuencia controlada destinada a impedir nuevas entradas y preparar visualmente la transición hacia la siguiente escena.

```text
[Selección de Opción]
          │
          ▼
 [Reproducir Click]
          │
          ▼
 [Bloquear Entradas]
          │
          ▼
 [Ejecutar Fade In]
          │
          ▼
 [Esperar Duración]
          │
          ▼
 [LoadScene()]
```

**1. Bloqueo de Interacciones Concurrentes**

La variable interna `isTransitioning` actúa como mecanismo de protección frente a múltiples pulsaciones consecutivas. Si la transición ya ha comenzado, cualquier nueva solicitud es descartada inmediatamente.

**2. Activación de la Transición de Salida**

Una vez validada la interacción, el componente reproduce la animación especificada en `_hideScreenAnim`, generalmente utilizada para oscurecer la pantalla antes del cambio de escena.

**3. Sincronización con la Animación**

La corrutina `ExitSequence()` mantiene suspendida la carga durante el intervalo definido por `_animDuration`, garantizando que la transición visual finalice completamente antes de abandonar la pantalla.

**4. Transferencia de Escena**

Tras completarse la espera, el sistema ejecuta `SceneManager.LoadScene(sceneToLoad)`, finalizando la secuencia de cierre y trasladando al jugador al destino configurado, normalmente el menú principal o una nueva sección de la aplicación.

#### Garantía de Consistencia Visual y Operativa

La combinación de control de entradas, sincronización audiovisual y gestión centralizada de eventos permite que la pantalla final mantenga un comportamiento estable durante toda su ejecución. Gracias a esta arquitectura, la conclusión de la experiencia de juego se presenta de forma ordenada y libre de interrupciones, evitando cargas duplicadas, solapamientos sonoros y transiciones abruptas que puedan afectar a la percepción del cierre de la partida.