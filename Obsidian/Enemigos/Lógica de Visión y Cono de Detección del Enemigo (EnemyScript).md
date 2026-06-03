Este componente implementa el comportamiento de detección e inteligencia artificial básica para las entidades enemigas. Gobierna el área de visión cónica (*Field of View*), comprueba la línea de visión directa mediante oclusión de capas, gestiona el filtrado del estado de ocultamiento del jugador y genera dinámicamente una malla poligonal en 2D para la visualización del cono de alerta en el juego.

Para separar la lógica de ejecución del flujo de renderizado en las herramientas de diseño, el sistema se divide en un script de comportamiento dinámico (`EnemyScript`) y un script de extensión de editor (`EnemyEditor`). Esto permite editar, depurar y observar de forma gráfica toda la información desde el propio inspector de Unity, garantizando que cada instancia en el mapa funcione de manera completamente independiente de las demás según las necesidades del nivel.

#### Inicialización de Variables y Atributos Geométricos
Antes de iniciar cualquier función, el componente expone e inicializa una serie de variables críticas para modular las propiedades geométricas que conforman el volumen del cono:
* **`viewRadius`:** Flotante que define el radio o distancia de alcance visual máxima a la que la entidad puede ver de forma efectiva.
* **`viewAngle`:** Controla la apertura angular o amplitud del campo de visión respecto al círculo que conforma el origen ($0^\circ$ a $360^\circ$).
* **`fovRotation`:** Ángulo de desfase que determina la orientación o dirección hacia la que apunta el cono, permitiendo que el enemigo vigile diferentes rutas sin depender de la rotación espacial de su propio objeto.

Adicionalmente, se declaran los elementos del buffer de mallas (`MeshFilter` y `Mesh`) con los que se genera y pinta el polígono en el escenario bidimensional, lo que permite alterar dinámicamente sus materiales cosméticos (como color, opacidad de alerta o texturas). Por último, se expone una lista interna de objetivos (`visibleTargets`) que almacena las referencias de las entidades detectadas tras superar los filtros físicos.

![[Cono_Detección.png]]

#### Ciclo de Muestreo Optimizado
Para mitigar el impacto sobre el rendimiento que conllevan los cálculos de trigonometría y proyecciones físicas complejas, el script separa la generación gráfica de la lógica de alerta:
* **Frecuencia Lógica Desacoplada:** La detección de objetivos no se calcula frame a frame dentro de los bucles nativos de Unity. En su lugar, el método `Start()` arranca una rutina asíncrona (`FindTargetsWithDelay`) que procesa la búsqueda de forma intermitente mediante un bucle infinito que pausa la ejecución durante una fracción de segundo controlada con `WaitForSeconds(delay)`.
* **Frecuencia Gráfica (`FixedUpdate`):** La reconstrucción de los vértices de la malla del cono (`DrawFieldOfView`) se computa al ritmo del motor de físicas, asegurando que la representación visual de la luz o cono de visión se mantenga sincronizada con las colisiones del entorno.

#### Matriz de Resolución de Detección (Línea de Visión)
El método `FindVisibleTargets` determina si Agnes ha sido descubierta utilizando un filtrado geométrico y físico de tres fases secuenciales:

```
[Fase 1: Rango]     Filtro Radial   --> OverlapCircleAll (Radio de visión)
[Fase 2: Ángulo]    Filtro Cónico   --> Vector2.Angle < (Ángulo de visión / 2)
[Fase 3: Oclusión]  Filtro Raycast  --> RaycastAll (Obstáculos vs Jugador)
```
1. **Filtro Radial:** Se realiza un escaneo de área circular (`Physics2D.OverlapCircleAll`) centrado en las coordenadas $(X,Y)$ exactas del enemigo con un radio delimitado por `viewRadius`, aislando los objetos que pertenezcan exclusivamente a la capa `targetMask`.
2. **Filtro Cónico (Slice of Pie):** Para cada objetivo detectado en el radio, calcula el vector de dirección ($\vec{D}$) y mide su apertura angular respecto al eje de rotación paramétrico del cono (`fovRotation`):
   $$\text{Ángulo} = \arccos(\vec{F}_{\text{fov}} \cdot \vec{D}) < \frac{\text{viewAngle}}{2}$$
3. **Filtro de Oclusión de Línea:** Al entrar en el sector angular, se traza una ráfaga de rayos (`Physics2D.RaycastAll`) hacia el jugador. Al usar `RaycastAll` en lugar de un `Raycast` simple, el hilo físico ignora el propio colisionador del enemigo si este se encuentra superpuesto.
   * **Bloqueo por Entorno:** Si el rayo impacta primero contra una entidad de la capa `obstacleMask` (paredes, coberturas), el bucle se interrumpe inmediatamente (`break`), asumiendo que la línea de visión está obstruida.
   * **Validación de Estado de Ocultamiento:** Si el rayo impacta la capa del jugador (`targetMask`), el script extrae el componente de movimiento de Agnes y verifica el estado de la bandera `charMovement.isHiding`. Si la bandera es **falsa**, la detección se convalida con éxito, anulando instantáneamente la inercia del jugador e invocando el reinicio de la escena:
     $$\vec{V}_{\text{player.rb}} = \vec{0}$$
     $$\text{LevelManager.Instance.ResetOnDeath()}$$

#### Sistema de Captura por Contacto Directo
Como salvaguarda de diseño, el script complementa el cono visual con una directiva de impacto físico inmediato a través del método `OnCollisionEnter2D`. Si el jugador entra en contacto directo con las físicas del cuerpo del enemigo (incluso si se aproxima por la espalda fuera del rango del cono), una operación de máscara de bits por capas convalida el contacto de forma instantánea e invoca el método de muerte:
$$(1 \ll \text{collision.layer}) \ \& \ \text{targetMask} \neq 0 \rightarrow \text{LevelManager.Instance.ResetOnDeath()}$$

#### Generación Procedural de Mallas (Mesh Generation)
El método `DrawFieldOfView` proyecta rayos de control a lo largo del arco del ángulo visual para mapear el contorno de los obstáculos:
* **Resolución de la Malla:** El número total de rayos proyectados se calcula multiplicando el ángulo por la densidad de la resolución (`viewAngle * meshResolution`) mediante el método `ViewCast()`.
* **Algoritmo de Definición de Bordes (Edge Resolution):** Cuando dos rayos consecutivos difieren en su estado de colisión (uno impacta contra una pared y el otro se proyecta hasta el infinito), el script invoca el método constructivo `FindEdge`. Este realiza una búsqueda binaria de forma iterativa (`edgeResolveIterations = 4`) calculando el ángulo medio para localizar con precisión matemática el vértice exacto de la esquina del obstáculo, mitigando el parpadeo gráfico de la malla en las esquinas.
* **Ensamblado del Polígono:** Con el conjunto de puntos de impacto recolectados, el script genera un array de vértices locales en base al origen del enemigo (`transform.InverseTransformPoint`) y un array de triángulos indexados que parten siempre desde el origen relativo cero (`vertices[0] = Vector3.zero`), actualizando finalmente las normales geométricas del objeto `Mesh`.
