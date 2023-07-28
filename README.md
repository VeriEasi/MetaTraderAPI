<a name="readme-top"></a>

<h1 align="center" style="display: block; font-size: 2.5em; font-weight: bold; margin-block-start: 1em; margin-block-end: 1em;">
<img align="center" src="https://avatars.githubusercontent.com/u/111559946?s=400&u=ce865a376c82f7c69c0c2ad443ef8a2d3767c4a6&v=4" alt="MetaTrader API Net Core" style="width:%;height:%"/></a>
  <br /><br /><strong>MetaTraderAPINetCore</strong>



<img src="https://img.shields.io/badge/Solution-.net6-blue?style=flat&logo=C-sharp&logoColor=b0c0c0&labelColor=363D44" alt="C# solution"/>
<img src="https://img.shields.io/badge/OS-windows-blue??style=flat&logo=Windows&logoColor=b0c0c0&labelColor=363D44" alt="Operating systems"/>
<img src="https://img.shields.io/badge/CPU-x86%20%7C%20x86__64-blue?style=flat&logo=Intel&logoColor=b0c0c0&labelColor=363D44" alt="CPU Architect"/>
</h1>

# Overview

MetaTraderAPINetCore provides a .NET Core API for working with famous trading platfrom MetaTrader 4 and 5. It is not API for connection to MT servers directly. It is just a bridge between MT terminal and .NET Core applications designed by developers.

MetaTraderAPINetCore executes MQL commands and functions by MtApi's expert linked to chart of MetaTrader. Most of the API's functions duplicates MQL interface.

<br/>

# Build environment

*	The project is supported by Visual Studio 2017, 2019 and 2022.
*	It requires WIX Tools for preparing project's installers.
You can install WIX extension in the Visual Studio extensions marketplace (In the “Extension” tab select “Manage Extensions” and then search for “Wix v3” in the “Visual Studio Marketplace”) or you can download it from https://wixtoolset.org/docs/wix3/ and install it manualy.
*	Use “MetaEditor” to working with MQL files.

<br/>

# How to Build Solution

To build the solution for MT4, you need to choose the configuration to build for “Release”,  “x86” and start with building the MtApiInstaller. This will build all projects related to MT4:

*	MtApi
*	MTApiService
*	MTConnector

For building the solution for MT5, you need to choose the configuration to build for “Release”, “x64” (or “x86” for the 32-bit MT5) and start build MtApi5Installer. This will build all projects related to MT5:

*	MtApi5
*	MTApiService
*	MT5Connector

All binaries are placed in the project root folder, in the build directory: 

    ../build/.

The installers (*.msi, *.exe) will be found under: 

    ../build/installers/.

All the DLL library binaries (*.dll) in: 

    ../bin/.

MQL files can be found under: 

    ../mq4/. 
    ../mq5/. 

Changing the source code of the MQL Expert Advisor (EA), requires recompilation with MetaEditor. Before you can recompile the EA, you need to add/place the following MQL library files, in the MetaEditor **../Include/** folder.

    ./hash.mqh
    ./json.mqh

The MetaEditor include folder is usually located here:

    C:\Users\<username>\AppData\Roaming\MetaQuotes\Terminal\<terminal-hash>\MQL5\Include\.



<br/>


# Project Structure



*   MTApiService: (C#, .dll)

    The common engine communication project of the API. It contains the implementations of client and server sides.
    
*   MTApiServiceNetCore: (C#, .dll)

    Port main MtApi project to .Net Core

*   MTConnector, MT5Connector: (C++/CLI, .dll)

    The libraries that are working as proxy between MQL and C# layers. They provides the interfaces.

*   MtApi, MtApi5: (C#, .dll)

    The client side libraries that are using in user's projects.

*   (MQL4/MQL5, .ex4)

    MT4 and MT5 Expert Advisors linked to terminal's charts. They executes API's functions and provides trading events.

*   MtApiInstaller, MtApi5Installer (WIX, .msi)

    The project's installers.

*   MtApiBootstrapper, MtApi5Bootstrapper (WIX, .exe)

    The installation package bundles. There are wrappers for installers that contains the vc_redist libraries (Visual C++ runtime) placed in ../vcredist/.


<br/>


# Installation

Use the installers to setup all libraries automatically.

*   For MT4, use: 

        MtApiInstaller.msi

*   For x86 MT5, use: 

        MtApi5Installer_x86.msi

*   For x64 MT5, use: 

        MtApi5Installer_x64.msi

The installers place the MTApiService.dll into the Windows GAC (Global Assembly Cache) and copies MTConnector.dll and MT5Connector.dll into the Windows's system folder, whose location depend on your Windows OS.

The installers place the MTApiService.dll into the Windows GAC (Global Assembly Cache) and copies MTConnector.dll and MT5Connector.dll into the Windows's system folder, whose location depend on your Windows OS.

After installation, the MtApi.ex4 (or MtApi5.ex5) EA, must be copied into your Terminal data folder for Expert Advisors, which is normally located in:

    C:\Users\<username>\AppData\Roaming\MetaQuotes\Terminal\<terminal-hash>\MQL5\ Experts \.

To quickly navigate to the trading platform data folder, click: **File >> "Open data folder"** in your MetaTrader Terminal.





<br />

<!-- LICENSE -->
## License
---

Distributed under the MIT License. See `LICENSE.txt` for more information.



<br />

<!-- CONTACT -->
## Contact
---

VeriEasi - verieasi2020@gmail.com

Project Link: https://github.com/VeriEasi/MetaTraderAPI.git
