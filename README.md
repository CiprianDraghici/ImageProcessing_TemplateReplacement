# ImageProcessing - TemplateReplacement

A basic example of template replacement in binary images using EmguCV library and TPL Dataflow from .NET

## Overview

#### Problem requirements

Given a set of binary images that may contain some white areas in random position and a template image, fill all the white area/areas with the provided template following this rules:

1. 	The size of the white areas can be different, but the valid areas has exactly the same size as the template and can not contain any black pixels.
2.	The template can not be manipulated. You need to replace all the valid areas with the same width and the same height as the template.
3.	For every input*.bmp from Input folder you need to create a output*.bmp in the Output folder. The format of this file must be .bmp.
4. 	Input folder and template folder are needs to be provided by you and the output folder needs to be created dynamically if does not exist in the same folder with the executable.

#### Technologies

1. C# .NET at least 4.5
2. EmguCV 3.3.0.2824 (check http://www.emgu.com/wiki/index.php/Main_Page for downloading)
3. TPL Dataflow from .NET (https://www.nuget.org/packages/Microsoft.Tpl.Dataflow/)

IDE : Microsoft Visual Studio 2017

## How to build the project ?

1. Install EmguCV and compile the sources (see how to use EmguCV with VS2017)
2. Install TPL Dataflow package from nuget
3. Go to Project tab in Visual Studio and choose Properties
4. In the opened window choose Build tab and change the Platform target:
	- if you choose x86 then : Add reference Emgu.CV.World.dll from bin/x86 folder found to the installed library path (usually in    C:\Emgu\emgucv-windesktop 3.3.0.2824)
	- if you choose x64 then : Add reference Emgu.CV.World.dll from bin/x64 folder found to the installed library path (usually in C:\Emgu\emgucv-windesktop 3.3.0.2824)
5. Right click on the solution name -> Add -> Add Existing Item... -> [Go to the EmguCV installed folder and find the "cvextern.dll"] -> Add
6. Right click on "cvextern.dll" and press "Properties". In the properties window please change the "Copy to Output Directory" property form Advanced section with "Copy always" option
