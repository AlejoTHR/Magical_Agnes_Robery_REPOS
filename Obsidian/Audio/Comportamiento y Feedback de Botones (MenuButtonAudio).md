
Este sistema centraliza el comportamiento visual y sonoro de los botones de la interfaz de usuario, garantizando una respuesta consistente tanto al usar el ratón como al navegar con mando. Es compatible con los componentes de gestión `MainMenu`, `MenuPausa` y `StartMenuManager`.

#### Animación Visual y Estados
El botón altera su escala y su posición en el eje y dependiendo de si está seleccionado (enfocado) o en reposo:

* **Efecto de Flote Inactivo (Bobbing):** Cuando el botón NO está seleccionado, flota continuamente en vertical para dinamizar la interfaz. Su posición se calcula en base a una onda senoidal:
$$Pos_y = PosInicial_y + (\sin((\text{Time.unscaledTime} + Offset) \times Velocidad) \times Cantidad)$$
* **Desincronización:** Cada botón calcula un $Offset$ aleatorio al inicio (entre $0$ y $5$) para evitar que todos los elementos del menú floten en perfecto unísono.
* **Escalado Dinámico:** Al seleccionarse, el botón detiene su balanceo, regresa de forma fluida a su posición inicial y aumenta su tamaño multiplicando su escala por $1.15$. Esta transición se realiza mediante una interpolación lineal (`Lerp`) insensible a las pauses del juego (`Time.unscaledDeltaTime`).

#### Feedback de Audio e Interacción
El script gestiona de forma interactiva el paso del cursor y la pulsación a través de los eventos del `EventSystem`:

* **Navegación Interactiva:** Al pasar el puntero sobre el botón (`OnPointerEnter`), este se fuerza automáticamente como el objeto seleccionado del sistema de eventos. Al salir (`OnPointerExit`), pierde el foco si mantenía la selección.
* **Sonido de Enfoque (Hover):** Al seleccionarse (`OnSelect`), se reproduce el clip `hoverSound`, siempre y cuando se detecte la presencia activa de cualquiera de los tres mánager del juego (`MainMenu`, `MenuPausa` o `StartMenuManager`). Además, si el botón contiene un componente de texto con efectos (`TMP_Wave`), se activa su animación de ondulación.
* **Sonido de Pulsación (Click):** Al presionar el botón (`OnPointerDown`), se detiene de forma inmediata el audio de enfoque para limpiar el canal y se reproduce el clip `hoverSelcectSound` mediante `PlayOneShot` para darle prioridad absoluta. Simultáneamente, se invoca el método `UI_PlayClick()` en el mánager que corresponda.
* **Deselección:** Al perder el foco (`OnDeselect`), el botón restablece su estado `isSelected` a `false` para reanudar el flote e indica al componente de texto que reinicie su estado.