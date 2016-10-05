# MatrixLibrary
Written as semestral work at _Programming in C#_ course on _MFF_.

## Description
**MatrixLibrary** is project of library which handles basic work with matrixes. It provides representation of matrix object as well as interface for representations of elements which will be stored in matrix. There are also some particular implementations of matrix element interface which can be used. 

### Implemented operations 
* **Altering operations:** Transposition, Symmetric, Gauss, Gauss-Jordan, Inverse, Adjugate, Orthogonal
* **Computations:** Determinant, Cramer, Linear equations system
* **Decompositions:** Cholesky decomposition, QR decomposition
* **Characteristics:** Eigenvalues, Eigenvectors, Diagonal matrix
* **Classic operations:** Addition, Subtraction, Multiplication, Multiply with number, Strassen-Winograd, Exponentiation
* **Properties:** IsInvertible, Rank, IsOrthogonal, Definity

### Drawbacks
* Pretty slow compared to matrixes which uses only primitive types (_int_, _double_)
* No tests, only example application
* Not ideal examples with advanced usage
* Sometimes Czech comments

## Compilation
Compilation is possible with provided **Visual Studio** project. If you want to generate documentation note that you will have to install **[Sandcastle](https://github.com/EWSoftware/SHFB)** for this.

Test application has to be run in **x64** configuration and can use more than **4GB** of RAM.

## License
Licensed under [MIT license](https://opensource.org/licenses/MIT).