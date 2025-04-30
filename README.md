# Revit Fake Rebar Annotator Plugin

## Description

This plugin for Autodesk Revit automatically annotates "fake" rebar elements when a project is opened. It searches for elements in the `OST_Rebar` category that are **not** of type `Rebar` or `RebarInSystem`, retrieves the `Rebar Diameter` and `L` parameters, and writes the following formatted string into the `Comments` parameter:

Ø<Diameter>, Grade 60, L=<Length>

## Installation

Copy the `.dll` and `.addin` files into the following folder:

C:\ProgramData\Autodesk\Revit\Addins\20XX\

Replace `20XX` with your installed Revit version (e.g., `2023`).

Make sure the path to the `.dll` file is correctly specified in the `.addin` file.

## Build Instructions

1. Open the `.sln` solution in Visual Studio.
2. Ensure the following references are added:
   - `RevitAPI.dll`
   - `RevitAPIUI.dll`
3. Build the project in `Release` mode.

## Usage

Open any Revit project that contains "fake" rebar elements — the plugin will automatically update their `Comments` parameter when the document is opened.

## Requirements

- Autodesk Revit 20XX  
- .NET Framework 4.8 or compatible
