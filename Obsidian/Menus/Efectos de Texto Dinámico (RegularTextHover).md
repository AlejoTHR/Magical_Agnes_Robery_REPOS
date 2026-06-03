Este componente se encarga de modificar de forma directa los datos de los vértices de una interfaz de texto basada en `TextMeshProUGUI`. El script genera una animación combinada de movimiento ondulatorio y un desplazamiento cromático (arcoíris) sin alterar la posición real del objeto en la UI. Su renderizado es insensible a las pauses del juego (`Time.unscaledTime`).

#### Manipulación de Vértices del Texto
Para evitar costes innecesarios de rendimiento, el sistema cachea los datos originales del texto y trabaja directamente sobre la malla geométrica interna:
* **Inicialización y Caché:** Al activarse (`OnEnable`) y en el ciclo inicial, se fuerza la generación de la malla (`ForceMeshUpdate()`). El script realiza una copia estructural (`CopyMeshInfoVertexData()`) que almacena las posiciones originales de los cuatro vértices que componen el cuadrilátero (*quad*) de cada carácter individual.
* **Ciclo de Actualización:** La lógica se ejecuta durante el método `LateUpdate()`. Esto garantiza que las deformaciones cosméticas se apliquen únicamente después de que el motor haya calculado las posiciones físicas de los elementos de la interfaz.

#### Animación del Movimiento Ondulatorio (Wave Effect)
El desplazamiento vertical se procesa letra por letra para dar la sensación de una ola fluida y continua:
* **Fórmula de Desplazamiento:** Cada carácter visible calcula un desfase en el eje y utilizando una función senoidal basada en su índice de posición ($i$) en la cadena de texto:
  $$\text{Offset}_y = \sin(\text{Time.unscaledTime} \times \text{speed} + (i \times \text{waveOffset})) \times \text{bounceAmount}$$
* **Aplicación Geométrica:** El vector resultante se suma de manera individual a los cuatro vértices correspondientes del carácter en base a los datos originales indexados.

#### Desplazamiento Cromático Espacial (Rainbow Shift)
Si la variable `_enableRainbow` está activa, el script reescribe los datos del array de color de la malla geométrica (`colors32`):

* **Cálculo de Tono Espacio-Temporal (Hue):** Para lograr que el degradado de color se mueva de forma continua a lo largo de las letras, el script calcula la posición del espectro cromático combinando la coordenada horizontal de los vértices (espacio) con el tiempo transcurrido (movimiento):
  $$\text{Hue} = \left(\text{Source}_{\text{hue}} \times \frac{\text{Frecuencia}}{100}\right) + (\text{Time.unscaledTime} \times \text{Velocidad})$$

  En esta ecuación, la frecuencia actúa como un factor de escala sobre la distancia: un valor más alto comprime el espectro haciendo que quepan más ciclos de arcoíris en el mismo espacio, mientras que la velocidad desplaza de manera constante dicho espectro a lo largo de la cadena de texto a través del tiempo.

* **Normalización de Color:** El valor de tono se normaliza en un rango estricto de $0$ a $1$ mediante una operación de módulo (`hue % 1.0f`). Si el resultado arroja un valor negativo, se le añade un entero para estabilizar el signo.
* **Inyección en Malla:** La métrica normalizada se convierte a formato RGB (`Color.HSVToRGB`) fijando la saturación y el brillo al máximo ($1.0$). El color resultante se inyecta directamente en los índices correspondientes del buffer de la malla antes de llamar a `UpdateGeometry()`.