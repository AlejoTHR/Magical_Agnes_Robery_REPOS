Esta habilidad permite a Agnes canalizar magia elemental de fuego de forma ofensiva mientras se encuentra en el aire. Al activarse, incrementa drásticamente las físicas de descenso para ejecutar un impacto contra el suelo (*fire slam*) capaz de quebrar obstáculos destructibles.

#### Activación y Entrada de Comandos (Mando / Teclado)
La habilidad se vincula al mapa de acciones de `PlayerInput` bajo la acción `"Fire"`, asignada de forma predeterminada al botón superior derecho del mando (*right shoulder*):
* **Condición de Entrada:** El estado de caída mágica (`_isFireMagicToggled`) solo puede activarse si el personaje se encuentra suspendido en el aire (`!plymov._grounded`). Si está tocando el suelo, la pulsación se ignora.
* **Alternancia (Toggle):** Si el botón es presionado consecutivamente durante la misma caída, el estado conuta de forma inversa, permitiendo al jugador cancelar la habilidad antes de impactar.
* **Feedback Sonoro de Inicio:** En el instante en que el estado pasa de falso a verdadero de manera efectiva, se reproduce un efecto de sonido de disparo único (`_startFallClip`) mediante `PlayOneShot` para enfatizar el impulso físico.

#### Alteración del Perfil de Físicas y Caída
Mientras la habilidad permanezca activa, se reescriben los parámetros del contenedor de configuración de físicas globales (`ScriptableStats`) durante la fase de simulación de físicas (`FixedUpdate`):
* **Modificación de Gravedad:** Los valores de velocidad límite de caída y de aceleración descendente del asset son sustituidos por las métricas de diseño configuradas en las variables del cañón de fuego:
  $$V_{\text{fall.max}} = \text{CannonSpeed}$$
  $$\text{Aceleración}_{\text{fall}} = \text{CannonAcceleration}$$
* **Compatibilidad de Estados:** El script activa la bandera `plymov.usingFireMagic = true` para forzar las transiciones de postura visual en el controlador de animación y apaga explícitamente cualquier rastro de otras magias en conflicto para evitar el solapamiento (`plymov.usingWindMagic = false`).
* **Nota de Persistencia de Asset:** Dado que `ScriptableStats` es un contenedor de datos persistente en disco (*ScriptableObject*), las modificaciones hechas a sus propiedades físicas durante la ejecución de esta habilidad alteran el estado global del asset. Al tocar tierra o cancelarse la acción, el sistema sobreescribe de nuevo los valores límites regresándolos a constantes estáticas del sistema ($40$ para velocidad máxima y $80$ para aceleración).

#### Sistema de Impacto y Detección de Destructibles
El script monitoriza constantemente un volumen de colisión en forma de caja (`OverlapBoxAll`) anclado al punto de comprobación inferior del personaje (`_detectTrasnform`):
* **Interacción con Entornos Destructibles:** Si la caja de colisión intersecta elementos marcados con la etiqueta física `"Destroyable"`, se instancia un efecto visual animado (`_breakEffectPrefab`) centrado en el objeto destruido (con un retraso de borrado `_effectDestroyDelay`), se elimina el obstáculo de la escena y se reinyecta la velocidad vertical negativa (`-CannonSpeed`) para permitir atravesar estructuras compuestas de bloques consecutivos sin frenar la inercia.
* **Impacto Final contra el Suelo:** Si Agnes colisiona contra un suelo regular, la diferencia de estados entre fotogramas (`!_wasGroundedLastFrame`) detecta el aterrizaje, ejecutando un efecto visual local (`TriggerLandingEffect`), apagando el bucle de fuego y restableciendo la locomoción neutra.