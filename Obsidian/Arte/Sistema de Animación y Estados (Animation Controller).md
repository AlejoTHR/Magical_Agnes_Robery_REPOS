
El sistema de animación de Agnes funciona de forma automatizada a partir de las físicas del personaje (velocidad, si está tocando el suelo) y las acciones que realiza (magias elementales). Esto asegura que la respuesta visual sea inmediata según lo que ocurra en el juego.

#### Volteo del Sprite (Flipping)
* El personaje cambia de dirección automáticamente según su velocidad en el eje x.
* Si el movimiento es hacia la derecha ($V_x > 0.1$) y miraba a la izquierda, se voltea.
* Si el movimiento es hacia la izquierda ($V_x < -0.1$) y miraba a la derecha, se voltea.
* Esto evita que el sprite parpadee o gire bruscamente cuando el personaje está casi parado.

#### Variables de Animación (Animator)
El control de las animaciones usa 6 parámetros booleanos principales:

- **Ground**: `true` cuando Agnes toca el suelo. Cambia entre las animaciones de estar en el aire o en tierra.
- **Walk**: `true` si está en el suelo y moviéndose ($\lvert V_x \rvert > 0.1$). Activa la animación de caminar.
- **Jump**: `true` si está en el aire y subiendo ($V_y > 0.1$). Activa la animación de salto. Se apaga al caer.
- **Fire**: `true` cuando se activa o canaliza la Magia de Fuego. Cambia la postura del personaje.
- **Wind**: `true` cuando se activa o canaliza la Magia de Viento. Cambia la postura del personaje.
- **Water**: `true` cuando se activa o canaliza la Magia de Agua. Cambia la postura del personaje.