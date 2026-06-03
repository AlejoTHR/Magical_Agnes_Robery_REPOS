Esta habilidad permite a Agnes canalizar magia elemental de viento para suspenderse en el aire, reduciendo drásticamente su velocidad de caída para llanear a través de grandes distancias o corrientes de aire.

#### Entrada Sostenida y Condiciones de Vuelo
La habilidad se vincula al mapa de acciones de `PlayerInput` bajo la acción `"Wind"`. A diferencia de otras disciplinas de activación única, el planeo requiere una pulsación continua:
* **Filtro de Activación Aérea:** El planeo solo puede iniciarse si el personaje se encuentra suspendido en el aire (`!plymov._grounded`) y el botón correspondiente es activado y mantenido presionado (`IsPressed()`).
* **Cacheado de Estado Previo:** Mientras la magia de viento no esté activa, el script registra frame a frame si la habilidad de fuego se encontraba operativa (`_wasFireActiveBeforeWind = fireExtinguisher.enabled`). Esto permite recordar si el jugador venía ejecutando una caída en picado.

#### Modificación de Atributos Físicos y Flote
Al activarse la sustentación, el sistema reescribe los límites dinámicos del contenedor de configuración física global (`ScriptableStats`):
* **Freno de Gravedad y Desplazamiento Lateral:** La velocidad de caída se limita de forma estricta al valor de diseño de la variable `fallspeed`, mientras que el desplazamiento horizontal máximo se ajusta al valor de la variable `slowmo` para ofrecer un movimiento más lento y controlado:
  $$V_{\text{fall.max}} = \text{fallspeed}$$
  $$V_{\text{horizontal.max}} = \text{slowmo}$$
* **Interrupción de Caída Inercial:** En el instante en que se despliega el planeo, el script anula por completo cualquier acumulación de fuerza en el eje y fijando de manera manual la velocidad vertical a cero (`linearVelocity = new Vector2(..., 0)`). Esto detiene en seco cualquier caída libre o descenso acelerado.
* **Control de Conflictos:** Se desactiva el componente de fuego (`fireExtinguisher.enabled = false`) y se actualizan los booleanos de animación del movimiento (`plymov.usingFireMagic = false`, `plymov.usingWindMagic = true`).

#### Finalización del Planeo y Restauración
En el momento en que el jugador suelta el botón de la acción (`!_input.actions["Wind"].IsPressed()`) o el personaje toca tierra firme de forma física, se invoca el método `StopGliding()` para restablecer el entorno neutro:
1. **Devolución de Control:** Si la habilidad de fuego estaba operativa antes de iniciar el vuelo, el script restaura su estado reactivando el componente (`fireExtinguisher.enabled = true`).
2. **Restauración de Parámetros de Diseño:** Los valores límite de la hoja `ScriptableStats` regresan automáticamente a sus configuraciones estándar de diseño de juego: velocidad de caída máxima a $40$ y velocidad de movimiento horizontal máxima a $14$.
3. **Reinicio de Recursos:** Al cerrar el ciclo de viento, el script limpia de forma automática la bandera de la magia de agua (`waterMagic.DashUsed = false`), permitiendo al personaje recuperar el uso del *dash* aéreo tras haber planeado.