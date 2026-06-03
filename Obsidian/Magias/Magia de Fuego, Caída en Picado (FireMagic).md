Esta habilidad permite a Agnes canalizar magia elemental de fuego  mientras se encuentra en el aire. Al activarse, incrementa drásticamente las físicas de descenso para ejecutar un impacto contra el suelo (*fire slam*) capaz de quebrar obstáculos destructibles.

#### Activación y Entrada de Comandos (Mando / Teclado)
La habilidad se vincula al mapa de acciones de `PlayerInput` bajo la acción `"Fire"`, asignada de forma predeterminada al botón superior derecho del mando (*right shoulder*):
* **Condición de Entrada:** El estado de caída mágica (`_isFireMagicToggled`) solo puede activarse si el personaje se encuentra suspendido en el aire (`!plymov._grounded`). Si está tocando el suelo, la pulsación se ignora.
* **Alternancia (Toggle):** Si el botón es presionado consecutivamente durante la misma caída, el estado conmuta de forma inversa, permitiendo al jugador cancelar la habilidad antes de impactar.
* **Feedback Sonoro de Inicio:** En el instante en que el estado pasa de falso a verdadero de manera efectiva, se reproduce un efecto de sonido de disparo único (`_startFallClip`) mediante `PlayOneShot` para enfatizar el impulso físico.

#### Alteración del Perfil de Físicas y Caída
Mientras la habilidad permanezca activa, se reescriben los parámetros del contenedor de configuración de físicas globales (`ScriptableStats`) durante la fase de simulación de físicas (`FixedUpdate`):
* **Modificación de Gravedad:** Los valores predeterminados de velocidad límite de caída ($40$) y de aceleración descendente ($80$) son sustituidos por las métricas de diseño configuradas en las variables del cañón de fuego:
  $$V_{\text{fall.max}} = \text{CannonSpeed}$$
  $$\text{Aceleración}_{\text{fall}} = \text{CannonAcceleration}$$
* **Compatibilidad de Estados:** El script activa la bandera `plymov.usingFireMagic = true` para forzar las transiciones de postura visual en el controlador de animación y apaga explícitamente cualquier rastro de otras magias en conflicto para evitar el solapamiento (`plymov.usingWindMagic = false`).
* **Restauración Neutra:** Al tocar tierra o cancelarse la acción, los datos de físicas de `ScriptableStats` regresan automáticamente a sus valores neutros ($40$ y $80$).

#### Sistema de Impacto y Detección de Destructibles
El script monitoriza constantemente un volumen de colisión en forma de caja (`OverlapBoxAll`) anclado al punto de comprobación inferior del personaje (`_detectTrasnform`):

* **Interacción con Entornos Destructibles:** Si la caja de colisión intersecta elementos marcados con la etiqueta física `"Destroyable"`, se procesan las siguientes acciones en cadena:
  1. Se instancia un efecto visual animado (`_breakEffectPrefab`) centrado en la posición del objeto destruído, con un temporizador automático de limpieza en segundos definido por `_effectDestroyDelay`.
  2. El objeto obstáculo es eliminado de la escena física (`Destroy`).
  3. Se reinicia el impulso vertical negativo de Agnes reinyectando de forma manual la fuerza de velocidad máxima (`-CannonSpeed`) en el componente físico, manteniendo su movimiento horizontal inalterado para permitir romper estructuras compuestas de varios bloques consecutivos sin frenar al personaje.

* **Impacto Final contra el Suelo:** Si Agnes colisiona contra un suelo regular que no posee la etiqueta de destrucción, la diferencia de estados entre fotogramas (`!_wasGroundedLastFrame`) detecta el aterrizaje forzoso. Esto ejecuta el método `TriggerLandingEffect()`, instanciando el prefab de impacto visual en sus pies, apaga de forma definitiva el bucle de fuego (`_isFireMagicToggled = false`) y restablece las físicas normales de movimiento.