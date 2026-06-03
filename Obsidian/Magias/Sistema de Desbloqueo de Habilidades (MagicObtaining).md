Este componente gestiona las zonas de interacción físicas distribuidas por el escenario que permiten a Agnes adquirir o desbloquear de forma permanente los tres tipos de magias elementales (Viento, Fuego y Agua). Actúa como un interruptor lógico de progresión que altera la disponibilidad de los scripts del jugador al interactuar con ellos.

#### Estructura de Clasificación de Magias (Enum)
Para flexibilizar el uso del mismo script en diferentes altares u objetos coleccionables de la escena, el sistema implementa una enumeración pública (`magicToGrant`). Esto permite seleccionar directamente desde el Inspector de Unity qué tipo de poder va a otorgar la instancia:

* **`Wind`:** Vinculado al componente de planeo (`WindMagic`).
* **`Fire`:** Vinculado al componente de caída en picado (`FireMagic`).
* **`Water`:** Vinculado al componente de impulso fluido (`WaterMagic`).

#### Detección de Proximidad y Asignación de Controles
El script monitoriza la presencia del personaje jugador mediante el sistema de físicas en dos dimensiones:
* **Entrada en Zona (`OnTriggerEnter2D`):** Al detectar la colisión con el volumen del sensor, la bandera `playerInRange` se establece en `true`. Se cachea una referencia directa al objeto (`player`) y se localiza dinámicamente el mapa de acciones de su componente `PlayerInput`, indexando de forma específica la acción de control `"Interact"`.
* **Salida de Zona (`OnTriggerExit`):** Cuando el jugador abandona el área del sensor, la bandera de rango regresa a `false`. 
  * *Nota de depuración: El método de salida utiliza `OnTriggerExit` (3D) en lugar de `OnTriggerExit2D`, lo que provocará que la bandera `playerInRange` no se restablezca correctamente a falso si el proyecto opera exclusivamente en un entorno bidimensional.*

#### Lógica de Desbloqueo e Intercambio de Componentes
Durante la ejecución del ciclo de actualización (`Update`), el componente comprueba simultáneamente el cumplimiento de los requisitos de interacción:

1. **Validación de Comando:** Se verifica que el jugador esté dentro del rango de acción y que haya presionado el botón asignado a la acción de interacción durante el fotograma actual (`InputAction.WasPressedThisFrame()`).
2. **Evaluación por Conmutador (`Switch`):** Al procesarse la interacción, un bloque de control evalúa el valor seleccionado en la propiedad `givenMagic`:

| Tipo Seleccionado | Componente Activado en el Jugador | Comportamiento en Escena |
| :--- | :--- | :--- |
| `magicToGrant.Wind` | `WindMagic.enabled = true` | El altar se desactiva de la jerarquía (`SetActive(false)`). |
| `magicToGrant.Fire` | `FireMagic.enabled = true` | El altar se desactiva de la jerarquía (`SetActive(false)`). |
| `magicToGrant.Water` | `WaterMagic.enabled = true` | El altar se desactiva de la jerarquía (`SetActive(false)`). |

3. **Consumo de Objeto:** Tras activar con éxito el script correspondiente en el cuerpo de Agnes, el objeto interactivo de la escena se oculta de forma definitiva desactivando su entidad en la jerarquía del motor para evitar que el jugador pueda interactuar de nuevo con él de manera redundante.