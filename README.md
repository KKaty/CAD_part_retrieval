# MySolidWorkAddin
SolidWorks addin for partial and global retrieval of CAD parts (Master thesis project)

## Usage
CAD_PatternComputation needs to be built referring the following SolidWorks dependences:
* solidworkstools.dll
* SolidWorks.Interop.sldworks.dll
* SolidWorks.Interop.swconst.dll
* SolidWorks.Interop.swcommands.dll
* SolidWorks.Interop.swdimxpert.dll
* SolidWorks.Interop.swpublished.dll
and adding (by NuGet) the following packages:
*Accord
*Accord.Math

After built the solution, register the DLL (placed in \bin\Release) using the following commands in the cmd (run as administrator):
* cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
* regasm.exe "PATH OF THE AngelSix-MenusAndToolbars.dll" /codebase
Now the plug-in can be included into SolidWorks,
* In the SolidWorks application, go under Tools-->Add-ins...
* Check "Menus and Toolbars"


## Supported formats
The following CAD formats are supported:
* .SLDPRT (part format of SolidWorks)


## Authors
* Katia Lupinetti (CNR IMATI), Franca Giannini (CNR IMATI), Marina Monti (CNR IMATI)


## Acknowldegment
If you use this project in your academic projects, please consider citing the project using the following 
BibTeX entry:

```bibtex
@article{giannini2017identification,
  title={Identification of similar and complementary subparts in B-rep mechanical models},
  author={Giannini, Franca and Lupinetti, Katia and Monti, Marina},
  journal={Journal of Computing and Information Science in Engineering},
  volume={17},
  number={4},
  year={2017},
  publisher={American Society of Mechanical Engineers Digital Collection}
}
```