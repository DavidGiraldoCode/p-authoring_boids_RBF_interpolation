# Authoring Boids using RBF interpolation - DH2323 Final project at KTH
<div style = "display: flex">
    <img width="100%" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Images/rbf_boids_cover.jpg"/>
<div/>
    
### **Abstract**

*Authoring the behavior of many virtual agents is time-consuming, involving multiple parameters and context-specific needs. Some steering algorithms use vector fields to influence agents' global paths. Jin's [JXJ*09] method stands out due to its use of Radial Basis Functions for gridless vector interpolation. This paper extends Jin's method to 3D vector fields for controlling the Boids algorithm by Reynolds (1998) and uses SteerBench test cases to evaluate this approach. Simulations showed Boids maneuvering through S shapes and shrinking to pass through narrow spaces. Implementation details and source code are available online.*

[Video DEMO](https://youtu.be/nZEUKUlAuHc)

[Full Report](https://drive.google.com/file/d/1k3AKPGAwXgeN48xWnjw-gUNlBZpsrc0d/view)

You may cite this work as:

Giraldo D, Authoring Boids using RBF interpolation (2024). C#. Available: https://github.com/DavidGiraldoCode/p-authoring_boids_RBF_interpolation

# Implementation

This study’s implementation introduces an approach that combines Jin’s flow field with Raynolds’ Boids algorithm, aiming to create a more plausible and visually appealing result for bird-like agents. The system ensures that the Boids maintain a balanced distance from each other, avoiding both excessive spreading and collisions while following a predefined path. The simulation used C# in Unity 2022.3 LTS without the use of any additional package or third-party API.

## Process and updates highlights
2024 May 20
<div style = "display: flex">
    <img width="100%" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Images/RBF_step_by_step_boids.jpg"/>
<div/>
2024 May 6
<br/>
<div style = "display: flex">
    <img width="320px" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Images/image.png"/>
    <img width="320px" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Images/vf1.png"/>
<div/>
<br/>
2024 April
<br/>
<img width="640px" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Flow_fields_test.gif"/>

# Radial Basis Functions (RBF)

How do we interpolate when there is no grid? Having no sample grid is a scattered data problem for which traditional linear interpolation does not suffice. Thus, Radial Basis Functions exist as a solution for this problem by defining a function capable of interpolating any given discrete value in space, given all the values at source points.

<div style = "display: flex">
    <img width="50%" src="https://github.com/DavidGiraldoCode/p-bois_steering_behaviors/blob/develop/Assets/Art/Images/rbf_equations.jpg"/>
<div/>


$$
S(\mathbf{x}) = \sum_{i=1}^{n} \lambda_i \phi(||\mathbf{x} - \mathbf{x}_i||), \quad \mathbf{x} \in \mathbb{R}^d.
$$

$$
\Phi(r) = ||\mathbf{x} - \mathbf{x}_i||
$$

```csharp
double Phi(Vector3 vector_j, Vector3 vector_i) //RBF
{
    Vector3 distance = vector_j - vector_i;
    float r = distance.magnitude;
    double GSkernel = Math.Exp(-0.001 * Math.Pow(r, 2)); //Gaussian (GS)
    double Skernel = r;  //Spline (S)
    return Skernel;
}
```

 Relationship between source points

```csharp
void ComputeInterpolationMatricesXY(List<Vector3> points, List<Vector3> vectors)
{
    int rows = points.Count;
    int columns = points.Count + 1;
    double[,] matrixX = new double[rows, columns];
    double[,] matrixY = new double[rows, columns];

    for (int j = 0; j < rows; j++)
    {
        for (int i = 0; i < columns; i++)
        {
            if (i < rows)
            {
                matrixX[j, i] = Phi(points[j], points[i]);
                matrixY[j, i] = Phi(points[j], points[i]);
            }
            else
            {
                matrixX[j, i] = vectors[j].x;
                matrixY[j, i] = vectors[j].z;
            }
        }
    }

    matrixPHIforX = matrixX;
    matrixPHIforY = matrixY;
}
```

$$
[\Phi]*[\lambda] = [f]
$$

Jin (2009) presented an application of crow authoring relaying on path-planning components. They incorporated radial basis function interpolation of vector fields to guide pedestrians' flow in a grid-less setup. I am implementing and applying their paper to author the flow of a flock of boids.

## More about crowd simulations

Representing multiple living entities in a virtual world is used in a number of fields, from movies and video games to urban planning and architecture. And [LBC*22] Lemori’s extensive categorization proves how committed computer graphics practitioners are to achieving plausible results. Rendering several virtual agents is known as crow simulation, a branch of computer graphics animation, and it deals with representing non-verbal behaviors of virtual beings and their relations with their environment and others, characterized by the change of their position over time [CFV*22].

Raynolds’ Boids model is a well-known steering behavior algorithm within the crow simulation field that simulates a flock of entities in a 3D digital environment. It has set a benchmark for what a user can do in terms of simulated animal behavior. He stated three main rules: collision avoidance, velocity matching, and flock centering, concepts that then were independently defined by Raynorlds as Separation, Alignment, and Cohesion [Rey02]. There are several strategies to steer and author these boids that focus on the agent's local movement. However, as computer processing improves, game titles and movies strive for increasingly complex scenes where multiple agents interact and follow scripted behavior and paths.

Defining a virtual agent’s behavior is known as authoring simulations, a multi-layer task that enables users to achieve creative intents and satisfy application-specific characteristics [LBC*22]. Six categories encompass the aspects that can be authored in a crow simulation: Hih-level behaviors, path-planning, local movements, body animation, visualization, and post-processing [LBC*22].

This paper focuses on Path-planing, which refers to authoring agents on a global scale, ideal when seeking to control large, endless crowds in complex environments not limited to a time window [LBC*22]. For global planning to happen, techniques such as Flows leverage vector fields that influence the path agents take without specifying individual paths explicitly. [LBC*22]

## References
Pulsar Bytes provided the bird mesh at the Unity Assets Store. Sound provided by SilentSeason at freesound.com.

Please refer to the section: [References](https://github.com/DavidGiraldoCode/p-authoring_boids_RBF_interpolation/blob/develop/Refences.md)
