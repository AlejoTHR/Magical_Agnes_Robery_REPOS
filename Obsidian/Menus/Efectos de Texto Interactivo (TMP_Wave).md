Este componente controla la deformación dinámica y localizada de los vértices de un elemento `TextMeshProUGUI`, generando una animación de onda física que actúa de manera interactiva como feedback visual cuando un botón o texto de la interfaz es enfocado.

#### Activación Condicional por Estado (Hover)
A diferencia de los sistemas de ondas constantes, este script vincula la actualización geométrica directamente al estado de interacción del componente padre:
* **Filtro de Optimización:** Al inicio de cada `LateUpdate()`, el script comprueba el estado del booleano `isHovered`. Si este se encuentra en `false`, la ejecución se interrumpe de inmediato sin forzar cálculos en la malla de texto.
* **Función de Restablecimiento (`ResetText`):** Cuando el elemento pierde el foco de la interfaz, se invoca externamente este método. Este apaga la bandera `isHovered` y ejecuta de inmediato `ForceMeshUpdate()`, lo que obliga a la malla gráfica de TextMeshPro a regresar a su estado plano original de manera instantánea.

#### Animación del Movimiento Ondulatorio Desincronizado
Cuando la animación se encuentra activa (`isHovered = true`), el sistema altera individualmente la posición vertical de los cuatro vértices que conforman el cuadrilátero (*quad*) de cada letra:
* **Caché de Malla:** El script realiza un volcado de respaldo de las posiciones base del texto (`CopyMeshInfoVertexData()`) para asegurar que la deformación senoidal se aplique siempre partiendo desde la geometría neutra del carácter.
* **Fórmula Senoidal de Desfase:** El desplazamiento en el eje y para cada letra visible se determina en función de su índice posicional ($i$), asegurando que el ciclo sea completamente ajeno a las ralentizaciones o pausas del juego mediante el uso de `Time.unscaledTime`:
  $$\text{Offset}_y = \sin(\text{Time.unscaledTime} \times \text{speed} + (i \times \text{waveOffset})) \times \text{bounceAmount}$$
* **Reescritura de Vértices:** El vector de compensación resultante se inyecta directamente sobre los buffers de vértices de la malla procesada (`destVertices`) antes de aplicar los cambios mediante `UpdateGeometry()`.