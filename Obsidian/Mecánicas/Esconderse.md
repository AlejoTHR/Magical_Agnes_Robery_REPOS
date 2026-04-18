El sistema de esconderse del personaje funcionará de la siguiente manera:
El jugador podrá esconderse al interactuar con un objeto (hiding spot) cuando esté dentro de su rango.
Al esconderse, el personaje se moverá automáticamente a la posición del escondite.

Mientras esté escondido:
- No podrá moverse
- No será visible (se desactiva el sprite)
- No colisionará de forma normal (se activa como trigger)
- Los enemigos no podrán detectarlo

Al volver a interactuar, el personaje saldrá del escondite y recuperará su estado normal.
Si el jugador sale del área del escondite, automáticamente dejará de estar escondido.

****
### Variables

**Variables Públicas**
- Movement
	- Usa el script de movimiento del jugador. Lo usamos para acceder a isHiding.
- PlayerInput
	- Permite detectar cuándo el jugador pulsa el botón de interactuar.
- RigidBody2D (Agnes)
	- Se usa para mover al personaje, fijar su posición y bloquear su movimiento.
- RigidBody2D (hidespot)
	- Define la posición exacta donde se colocará el jugador al esconderse.

**Variables Privadas**
- playerInRange
	- Bolean que indica si el jugador esta dentro del rango del escondite. 
- CapsuleCollider2D
	- Collider
- SpriteRenderer
	- Controla la visibilidad del personaje.

****
### Como funciona el código
1. Al iniciar, el script busca automáticamente al jugador si no está asignado
2. Obtiene sus componentes (Rigidbody, Collider, Movement, Sprite, Input)
3. Cuando el jugador entra en el área del escondite:
	- Se activa **playerInRange = true**
4. En cada frame (Update):
	- Se comprueba si el jugador pulsa el botón de interactuar
	- Si está en rango:
	    - Si NO está escondido → se ejecuta **Hide()**
	    - Si YA está escondido → se ejecuta **Unhide()**
5. Cuando el jugador está escondido:
	- Se mueve automáticamente a la posición del escondite
	- Se bloquea su movimiento (FreezePosition)
	- Se mantiene fijo en ese punto
6. Función **Hide()**:
	- Activa **isHiding = true**
	- Cambia el collider a trigger (sin colisión física)
	- Oculta el sprite del jugador
7. Función **Unhide()**:
	- Desactiva **isHiding = false**
	- Restaura la colisión normal
	- Vuelve a mostrar el sprite
	- Permite movimiento (solo bloquea rotación)
8. Cuando el jugador sale del área del escondite:
	- Si estaba escondido → se ejecuta **Unhide()** automáticamente
	- Se desactiva **playerInRange = false**
9. El sistema se repite continuamente