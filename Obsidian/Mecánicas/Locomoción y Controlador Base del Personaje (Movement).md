Este componente actúa como el núcleo motor y gestor de estados físicos de Agnes. Centraliza la recolección de entradas del jugador (*Input Gathering*), el procesamiento de colisiones contra superficies, la gestión dinámica de la gravedad, la locomoción horizontal y el enrutamiento del sistema de audio maestro de movimiento.

#### Inicialización y Restricciones del Cuerpo Físico
Durante el ciclo `Awake()`, el controlador parametriza de forma obligatoria las dependencias físicas para asegurar la precisión del movimiento en dos dimensiones:
* **Bloqueo de Rotación:** Se configuran las restricciones del cuerpo rígido (`FreezeRotation`) para impedir que fuerzas externas alteren el ángulo de inclinación vertical del personaje, manteniendo el vector superior fijo.
* **Modo de Colisión Continuo:** Se establece la propiedad `collisionDetectionMode` en `Continuous`. Esto mitiga el error de tunelización física (*tunneling*), garantizando que el personaje no atraviese plataformas a altas velocidades mientas se aplican las magias elementales.

#### Ciclo de Simulación y Sincronización Estructural
El script separa la recolección de lecturas lógicas de la aplicación de fuerzas físicas mediante el uso síncrono de los métodos nativos de Unity:
1. **Fase Lógica (`Update`):** Procesa frame a frame de manera insensible a las caídas de fotogramas la captura de vectores de control (`GatherInput`), la modulación del bucle auditivo maestro y fuerza una rotación de identidad neutra (`Quaternion.identity`) para corregir desajustes visuales.
2. **Fase Física (`FixedUpdate`):** Sincroniza la velocidad interna del búfer local (`_frameVelocity`) con el estado cinemático real del `Rigidbody2D`. Una vez procesados los sub-módulos de salto, fricción y gravedad, inyecta el vector modificado de regreso al motor físico:
   $$\vec{V}_{\text{final}} = \vec{V}_{\text{calculada}}$$

#### Sistema de Detección de Superficie (Grounding)
La comprobación de contacto con el suelo emplea una proyección de volumen capsular (`Physics2D.CapsuleCast`) que replica la geometría del colisionador de Agnes de manera ligeramente más estrecha para evitar fricciones falsas con paredes verticales:
* **Filtro del Ángulo de Superficie:** Una colisión solo se valida como superficie transitable si el vector normal del plano de impacto apunta hacia arriba superando el umbral de tolerancia angular:
  $$\text{hit.normal.y} > 0.7 \rightarrow (\text{Ángulo} < 45^\circ)$$
* **Tiempo de Tolerancia (Coyote Time):** Si el personaje abandona una plataforma sin saltar, la bandera `_grounded` retiene el estado verdadero durante una ventana de gracia configurable (`_groundedGracePeriod = 0.05s`), permitiendo ejecutar saltos en el vacío de manera permisiva.

#### Lógica de Locomoción Horizontal y Frenado
El movimiento horizontal computa de forma independiente los estados de aceleración y desaceleración utilizando interpolaciones lineales temporales (`Mathf.MoveTowards`):
* **Fuerza Impulsora:** Al registrar entrada en el eje x, la velocidad actual tiende hacia la velocidad límite definida por el asset de configuración global (`_stats.MaxSpeed`):
  $$V_x \rightarrow V_{\text{target}} \quad \text{vía} \quad \text{Acceleration} \times \Delta t_{\text{fixed}}$$
* **Fricción Dinámica:** Cuando la lectura de entrada horizontal es neutra, el sistema aplica una fuerza de arrastre diferenciada según el entorno físico del personaje (`GroundDeceleration` en tierra o `AirDeceleration` en suspensión aérea) para arrastrar la inercia del vector horizontal de forma progresiva hacia cero.

#### Control del Vector Gravitatorio
Para contrarrestar el efecto de flotabilidad residual (*hovering*) provocado por las aproximaciones numéricas del motor de físicas, el script altera la gravedad en base al estado de contacto:
* **Anclaje en Suelo:** Si el personaje está posicionado sobre una superficie estable y su velocidad vertical es descendente o nula, se fuerza una velocidad negativa constante de $-0.1\text{f}$ para "pegar" el cuerpo a la plataforma.
* **Aceleración de Caída:** En estado de suspensión, el vector vertical acelera de forma matemática hacia el límite de caída libre establecido por el archivo de configuración global:
  $$V_y \rightarrow -V_{\text{fall.max}} \quad \text{vía} \quad \text{FallAcceleration} \times \Delta t_{\text{fixed}}$$

#### Sistema de Gestión de Audio Maestro (HandleMasterAudio)
El script implementa una máquina de estados auditiva que prioriza de manera jerárquica los sonidos continuos del personaje en función de las banderas elementales activas mediante la siguiente matriz de resolución:

```
[Prioridad 1] Magia de Viento Activa  --> Loop: _windGlideClip
[Prioridad 2] Magia de Fuego (En Aire) --> Loop: _fireCannonballClip
[Prioridad 3] Locomoción en Tierra   --> Loop: _walkClip
[Cualquier Otro Estado]              --> Detener Canal (AudioSource.Stop)
```

Al inicializar un clip de audio de forma persistente (`loop = true`), el sistema aplica variaciones aleatorias controladas de tono (*pitch*) y volumen dentro de un rango del $20\%$ en cada ciclo de reproducción. Esto rompe la monotonía acústica del muestreo digital y aporta naturalidad a las ráfagas elementales y los pasos del personaje.