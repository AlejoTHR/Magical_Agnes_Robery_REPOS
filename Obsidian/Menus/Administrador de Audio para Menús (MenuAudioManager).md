Este componente actúa como un controlador de audio especializado para los menús, optimizando la reproducción de efectos de sonido redundantes mediante un sistema de protección temporal por código. Utiliza un patrón de diseño *Singleton* (`Instance`) para centralizar las peticiones de audio de la interfaz.

#### Filtro de Prioridad y Protección contra Solapamiento (Click Protection)
El sistema resuelve el problema común en interfaces de usuario donde el sonido de enfoque (*hover*) interrumpe o ensucia el sonido de confirmación (*click*) debido al movimiento residual del cursor tras pulsar un botón.

* **Registro de Tiempo Global:** El script monitoriza el tiempo del juego de forma independiente a las pausas utilizando la métrica `Time.unscaledTime` en segundos.
* **Lógica de Bloqueo Temporal:** Cuando se procesa un sonido de confirmación (`isClick = true`), el sistema actualiza la variable de marca de tiempo `lastClickTime`. A partir de ese instante, se activa una ventana de protección con una duración fija de $0.25$ segundos.
* **Descarte de Peticiones:** Cualquier petición de sonido de enfoque (`isClick = false`) que intente ejecutarse dentro de la ventana de protección es destruida de inmediato si se cumple la siguiente condición física:
  $$\text{Time.unscaledTime} < \text{lastClickTime} + \text{clickProtectionDuration}$$

#### Control del Canal de Reproducción
Si la petición de sonido supera el filtro de protección o se trata de una pulsación directa, el componente limpia el canal antes de proceder:
1. Se detiene por completo cualquier audio en reproducción mediante `source.Stop()` para evitar la saturación o la mezcla de frecuencias.
2. Se asigna el nuevo recurso de audio (`AudioClip`) al canal físico.
3. Se ejecuta la reproducción inmediata del sonido mediante `source.Play()`.