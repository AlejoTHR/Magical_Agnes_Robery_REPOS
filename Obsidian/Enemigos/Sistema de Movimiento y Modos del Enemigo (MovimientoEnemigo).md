Este componente gobierna el desplazamiento espacial y el comportamiento de orientación de las entidades enemigas en el escenario. Implementa un sistema dual conmutabledos mediante el Inspector de Unity (`isCameraMode`), permitiendo alternar entre una lógica de patrulla lineal entre dos puntos focales o un comportamiento estático de cámara de seguridad con barrido angular oscilatorio. Además, centraliza la sincronización visual de los sprites y los disparadores del controlador de animaciones (*Animator*).

#### Conmutación de Modos de Funcionamiento
A través del campo booleano `isCameraMode`, el script bifurca sus rutinas operativas durante los ciclos de inicialización y actualización lógica (`Update`):
* **Modo Patrulla (`false`):** Diseñado para enemigos móviles. Inicializa el componente `Animator`, calcula el vector de traslación hacia los nodos de la ruta y reproduce animaciones de caminata.
* **Modo Cámara (`true`):** Diseñado para dispositivos de vigilancia estáticos o centinelas fijos. Anula por completo el desplazamiento físico del objeto y activa un bucle de rotación trigonométrica para el cono de visión adjunto.

#### Lógica de Patrulla Lineal (HandlePatrol)
Cuando el componente opera en modo estándar, calcula traslaciones rectilíneas frame a frame de forma insensible a las caídas de fotogramas mediante interpolaciones vectoriales:
1. **Desplazamiento hacia el Objetivo:** Modifica la posición del `Transform` local utilizando el método lineal `Vector2.MoveTowards` en dirección al nodo activo (`currentTarget`), condicionado por la variable de velocidad constante `speed`.
2. **Detección de Arribo y Pausa:** Al alcanzar un umbral de proximidad inferior a $0.1$ unidades de distancia respecto al destino, se detiene el avance invocando el método `StartWait(waitTime)`. Esto desactiva el booleano `IsMoving` del *Animator* para transicionar el personaje a su estado de reposo (*Idle*).
3. **Alternancia de Nodos (`UpdateFacing`):** Al agotarse el temporizador de espera (`waitTimer`), el script conmuta el objetivo entre los extremos paramétricos `pointA` y `pointB`, calcula el nuevo vector de dirección normalizado y actualiza la orientación del cono visual y del renderizador gráfico.

#### Modo Cámara y Barrido Angular (HandleCameraRotation)
Al activarse la propiedad de vigilancia fija, el comportamiento emula el barrido mecánico de un sensor volumétrico mediante interpolaciones angulares flotantes (`Mathf.Lerp`):
* **Cálculo del Factor de Inclinación:** La variable interna `lerpFactor` acumula o drena el tiempo delta multiplicado por la velocidad de rotación (`rotationSpeed`). Dependiendo de la bandera de dirección `movingForward`, el factor oscila de forma estricta entre los límites normados de $0$ y $1$.
* **Inversión de Sentido:** Al alcanzar los extremos del espectro ($1.0$ o $0.0$), el script bloquea el factor en sus límites absolutos, invoca la rutina de espera estática `StartWait(pauseTime)` e invierte el valor de `movingForward` para iniciar el viaje de retorno angular.
* **Modulación del Espectro Cromático/Direccional:** Permite configurar el barrido a favor o en contra de las agujas del reloj mediante el campo `rotateClockwise`, alterando el orden de asignación de las variables de entrada del ángulo:
  $$\text{currentAngle} = \text{Mathf.Lerp}(\text{actualStart}, \text{actualEnd}, \text{lerpFactor})$$
  El valor resultante se inyecta directamente al campo `fovRotation` del componente `EnemyScript` para desplazar el cono de detección procedural.

#### Sincronización Gráfica y Orientación del Sprite
Para asegurar que la representación visual del enemigo mantenga coherencia estética con la dirección del movimiento o el cono de alerta, el script implementa dos sistemas diferenciados de orientación:

* **Inversión de Espejo por Traslación (`Sprite Flip`):** Durante la patrulla móvil, el método `UpdateFacing` analiza el signo del componente horizontal del vector de dirección ($\vec{D}_x$). Si el desplazamiento ocurre hacia el flanco derecho ($\vec{D}_x > 0.01\text{f}$), desactiva la propiedad `flipX` del `SpriteRenderer`; si ocurre hacia la izquierda ($\vec{D}_x < -0.01\text{f}$), activa el volteado horizontal. Simultáneamente, el cono visual se reorienta calculando el arcotangente del vector:
  $$\text{enemyScript.fovRotation} = \arctan(\vec{D}_x, \vec{D}_y) \times \text{Mathf.Rad2Deg}$$
* **Rotación de Matriz Rígida (`SyncSpriteRotation`):** Si la bandera `rotateSpriteWithCone` está activa (esencial para el modo cámara), el script anula el volteado por espejo y acopla la rotación del transform en el eje z de forma síncrona al cono, aplicando un factor de desfase angular para corregir la orientación nativa del asset gráfico:
  $$\text{Rotación}_z = -\text{enemyScript.fovRotation} - 110^\circ$$
  $$\text{transform.eulerAngles} = (0, 0, \text{Rotación}_z)$$