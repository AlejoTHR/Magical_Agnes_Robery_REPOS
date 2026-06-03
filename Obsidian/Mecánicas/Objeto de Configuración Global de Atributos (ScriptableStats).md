Este componente hereda de `ScriptableObject` y actúa como un contenedor de datos persistente en disco. Su propósito dentro de la arquitectura del juego es centralizar, desacoplar y estandarizar todas las variables físicas de fricción, zonas muertas de control, parámetros de salto y umbrales de detección para el personaje jugador (Agnes).

#### Arquitectura de Datos Desacoplada
Al almacenar los atributos fuera de los scripts lógicos de movimiento, el sistema permite modificar el comportamiento dinámico y el balance del videojuego en tiempo de ejecución desde el Inspector de Unity sin alterar el estado de las variables en los componentes del objeto. El script de locomoción lee estos campos como valores de configuración de solo lectura, garantizando la integridad de los datos de diseño preconfigurados durante las sesiones de juego.

#### Configuración de Capas y Control de Zonas Muertas (Input)
El asset define los parámetros de tolerancia para homogeneizar el comportamiento entre teclados y mandos (ej: Nintendo Switch Pro Controller):
* **Normalización de Entrada (`SnapInput`):** Configurado en `true`. Fuerza el redondeo de los vectores flotantes hacia valores enteros discretos ($-1, 0, 1$). Esto anula las aceleraciones residuales de los joysticks analógicos y garantiza la paridad de respuesta con un teclado.
* **Umbral Muerto Vertical (`VerticalDeadZoneThreshold = 0.3f`):** Define el recorrido mínimo que debe realizar el stick analógico en el eje vertical antes de validar acciones de escalado o colgado, previniendo activaciones accidentales por holgura (*drift*) del mando.
* **Umbral Muerto Horizontal (`HorizontalDeadZoneThreshold = 0.1f`):** Delimita la tolerancia del movimiento horizontal residual para evitar desplazamientos fantasma causados por mandos desgastados.

#### Parámetros Dinámicos de Locomoción (Movement)
Modula el comportamiento inercial y las fuerzas de traslación del controlador físico basándose en el perfil activo del Inspector de Unity:
* **Velocidad Máxima Horizontal (`MaxSpeed = 14`):** Límite superior del vector de velocidad horizontal en condiciones de carrera neutra.
* **Capacidad de Aceleración (`Acceleration = 120`):** Ritmo de ganancia de velocidad frame a frame en el eje de traslación horizontal.
* **Desaceleración en Tierra (`GroundDeceleration = 120`):** Fuerza de fricción aplicada para frenar la inercia del jugador cuando no hay comandos de movimiento activos sobre una superficie.
* **Desaceleración en Aire (`AirDeceleration = 60`):** Coeficiente de arrastre que frena la inercia horizontal de forma más suave cuando el jugador suelta los controles en el aire.
* **Fuerza de Anclaje (`GroundingForce = -1.5f`):** Fuerza descendente constante aplicada mientras se interactúa con el suelo para asegurar el contacto y mitigar el rebote en pendientes o rampas.
* **Distancia de Suelo (`GrounderDistance = 0.2f`):** Longitud de desplazamiento del volumen de proyección (*CapsuleCast*) dedicado a validar la proximidad física del suelo o del techo.

#### Parámetros Avanzados de Salto y Gravedad Aérea (Jump)
Controla la curva parabólica del salto y las mecánicas permisivas de diseño de niveles:
* **Fuerza de Impulso (`JumpPower = 25`):** Velocidad vertical instantánea inyectada de forma ascendente en el momento de procesar un salto válido.
* **Velocidad Límite de Caída (`MaxFallSpeed = 40`):** Umbral máximo absoluto que puede alcanzar el vector de aceleración descendente por gravedad en caída libre.
* **Gravedad en Aire (`FallAcceleration = 80`):** Ritmo de aceleración vertical descendente aplicada frame a frame en el ciclo físico mientras el personaje permanezca suspendido de forma neutra.
* **Modificador de Salto Cortado (`JumpEndEarlyGravityModifier = 3`):** Multiplicador de gravedad que entra en funcionamiento si el jugador suelta el botón de salto antes de alcanzar el ápice de la parábola, permitiendo controlar la altura del salto según el tiempo de pulsación.
* **Tiempo Coyote (`CoyoteTime = 0.15f`):** Ventana de gracia en segundos que determina cuánto tiempo puede transcurrir tras abandonar físicamente el borde de una plataforma antes de invalidar el comando de salto.
* **Búfer de Entrada de Salto (`JumpBuffer = 0.2f`):** Ventana de anticipación en segundos que registra una pulsación de salto justo antes de tocar el suelo, ejecutándolo automáticamente en el primer fotograma de contacto válido.