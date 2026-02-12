# AI Model Input and Output Specification

The application supports integration of artificial intelligence models saved in the `Open Neural Network Exchange` - `.ONNX` format. To ensure proper compatibility with the system, the loaded model must meet the technical specification described below.

## Input Data

The model should accept a single grayscale ultrasound image as input, stored as a tensor with dimensions $(1, 680, 560, 1)$. The data in the tensor should be of type float32, with values in the range [0-1].

## Output Data

The model should return four heatmaps describing the positions of four points, which are used to calculate the optic nerve width. The heatmaps should be returned as a tensor with shape $(1, 680, 560, 4)$ containing float32 values in the range [0-1]. The values should represent the probability that a given pixel is the location of the corresponding point. Each heatmap should describe a different point, as follows:

1. Point marking the edge of the retina along the nerve axis
2. Point 3 mm below point 1 along the nerve axis
3. Left edge of the optic nerve sheath at the height of point 2
4. Right edge of the optic nerve sheath at the height of point 2

This corresponds to the points marked by the operator during the examination to calculate the optic nerve width.
