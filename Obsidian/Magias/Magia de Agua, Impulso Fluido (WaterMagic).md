# 1.4 Magia de Agua: Impulso Fluido (WaterMagic)

Esta habilidad permite a Agnes canalizar magia elemental de agua para ejecutar un desplazamiento horizontal rápido e invulnerable a la gravedad (*dash*), utilizable tanto en tierra como en el aire para mejorar la movilidad y esquivar amenazas.

#### Condiciones de Uso y Control de Tiempo de Espera (Cooldown)
La ejecución de la habilidad se vincula al mapa de acciones de `PlayerInput` bajo la acción `"Water"`. El sistema valida de forma estricta las condiciones físicas para evitar la saturación de impulsos:
* **Filtro de Activación:** El *dash* solo se ejecuta si la acción es presionada y el temporizador interno de enfriamiento ha finalizado (`_cooldownTimer <= 0`).
* **Restricción de Impulso Aéreo:** Si Agnes está en el suelo (`plymov.isGrounded()`), puede realizar el *dash* siempre que el *cooldown* esté listo. Si se encuentra en el aire, solo se le permite un único desplazamiento por salto mediante el control de la bandera `DashUsed`.
* **Prevención de Spam en Tierra:** La bandera `DashUsed` regresa a `false` únicamente cuando el personaje está tocando el suelo **Y** el tiempo de enfriamiento ha terminado por completo. Esto impide encadenar múltiples impulsos seguidos a ras de suelo de forma descontrolada.

#### Ejecución Física e Inyección de Velocidad (DashRoutine)
Cuando se cumplen los requisitos, se inicia una corrutina asíncrona (`DashRoutine`) que toma el control absoluto de las físicas del personaje durante una ventana temporal fija en segundos definida por `dashDuration`:
1. **Interrupción de Estados Concluyentes:** Al arrancar, se frena por completo la inercia previa (`linearVelocity = Vector2.zero`) y se desactiva temporalmente el script de la habilidad de fuego (`fire.enabled = false`) para evitar interrupciones o solapamientos de comandos.
2. **Direccionamiento Dinámico:** El script consulta la dirección de entrada actual del jugador. Si la lectura es neutra ($0$), la física calcula la orientación del impulso basándose en la escala local del sprite en el eje x:
   * Si $\text{localScale.x} > 0 \rightarrow \text{Dirección} = 1.0$ (Derecha).
   * Si $\text{localScale.x} < 0 \rightarrow \text{Dirección} = -1.0$ (Izquierda).
3. **Bucle de Fuerza Bloqueante:** Durante el desplazamiento, se fuerza frame a frame en el ciclo físico (`WaitForFixedUpdate`) una velocidad constante anulando por completo el eje y. Esto asegura que Agnes flote horizontalmente sin verse afectada por la gravedad:
   $$V_x = \text{dashPower} \times \text{Dirección}$$
   $$V_y = 0.0$$

#### Salida del Estado de Impulso
Una vez completado el tiempo de la animación de desplazamiento (`elapsed >= dashDuration`), el sistema detiene en seco al personaje fijando su velocidad a cero. Se reactiva el componente de fuego (`fire.enabled = true`) y se limpian las banderas de estado elemental del controlador de animaciones (`usingWaterMagic = false`, `usingFireMagic = false`), regresando a la locomoción neutra.