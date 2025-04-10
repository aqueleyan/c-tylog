<a id="readme-top"></a>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Unlicense License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<br />
<div align="center">
  <a href="https://github.com/aqueleyan/c-tylog">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Tylog in C#</h3>

  <p align="center">
    A simple and asynchronous logger written in C#.
    <br />
    <br />
    <a href="https://github.com/aqueleyan/c-tylog/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/aqueleyan/c-tylog/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li>
      <a href="#built-with">Built With</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#props">Props</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

## About The Project

This project is an **asynchronous logger in C#** with support for multiple log levels (Debug, Info, Warning, Error, Fatal), separate file outputs for errors and fatal logs, an optional prefix for new logger sessions, and the ability to create directories for your log files on the fly.

**Key features** include:

- Define a **minimum log level** to discard lower-priority messages  
- **Separate files** for error and fatal logs  
- **Optional prefix** appended when a new session starts on an existing file  
- **Auto-create directories** for the specified log file paths  
- Thread-safe logging with **asynchronous queue processing**

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With

- C# (targeting .NET 6+ or similar)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started

### Prerequisites

- .NET 6 SDK or higher (or a compatible runtime)
- Basic understanding of C# projects and namespaces

### Installation

1. **Clone** the repository:
   ```bash
   git clone https://github.com/aqueleyan/c-tylog.git
   cd c-tylog
   ```

2. **Build** the project (using .NET CLI):
   ```bash
   dotnet build
   ```

3. **Run** the example in `Program.cs`:
   ```bash
   dotnet run
   ```

Alternatively, you can **add** the `Logger.cs` file to any existing C# project, ensuring it shares the correct namespace (e.g., `namespace Logger;`). Then, call its static configuration method and use the logger instance as needed.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Usage

Below is a minimal sample from `Program.cs`. You can customize log levels, choose a custom log name prefix, and decide whether to treat new sessions differently if the same log file already exists.

```csharp
using Logger;
using System;

namespace Logger
{
    public class Program
    {
        public static void Main()
        {
            Logger.Configure(LogLevel.Warning, "MyLogPrefix", false);
            Logger.Instance.Debug("Debug message.");
            Logger.Instance.Info("Info message.");
            Logger.Instance.Warning("Warning message.");
            Logger.Instance.Error("Error message.");
            Logger.Instance.Fatal("Fatal message.");
            Logger.Instance.Dispose(); // Make sure to dispose on shutdown
            Console.WriteLine("Done.");
        }
    }
}
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Props

| Method Signature                                            | Description                                                                                            | Default                                   |
|-------------------------------------------------------------|--------------------------------------------------------------------------------------------------------|-------------------------------------------|
| `Logger.Configure(LogLevel minLogLevel, string? logName, bool newSessionOnExistingFile)` | Sets up logger's minimum level, log name prefix, and new-session prefix behavior                       | `LogLevel.Debug, "", false`               |
| `Logger.Instance.Log(LogLevel level, string message)`       | Logs a message of the specified level                                                                  |                                           |
| `Logger.Instance.Debug(string message)`                     | Logs a **Debug** message (lowest priority)                                                             |                                           |
| `Logger.Instance.Info(string message)`                      | Logs an **Info** message                                                                               |                                           |
| `Logger.Instance.Warning(string message)`                   | Logs a **Warning** message                                                                             |                                           |
| `Logger.Instance.Error(string message)`                     | Logs an **Error** message                                                                              |                                           |
| `Logger.Instance.Fatal(string message)`                     | Logs a **Fatal** message                                                                               |                                           |
| `Logger.Instance.Dispose()`                                 | Ensures all logs are flushed and the logging task is cleaned up. Called typically when the app ends    |                                           |

**Notes**  
- If `newSessionOnExistingFile` is `true`, the logger will prepend `--- NEW LOGGER SESSION: yourPrefix started at [DATE/TIME] ---` to existing log files.  
- Separate files (`errors.log` and `fatal.log`) are automatically managed based on messages at **Error** or **Fatal** levels.  
- The logger automatically creates directories if they do not exist, preventing path issues.  

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Roadmap

- [x] Support for creating log path directories automatically
- [x] Separate files for Error and Fatal logs

See the [open issues][issues-url] for more proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contributing

Contributions are what make the open source community so amazing. Any contribution is appreciated!

If you have suggestions that would make this better, please fork the repo and create a pull request. You can also open an issue with the "enhancement" label. Don't forget to give the project a ⭐ if you like it!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/MyCSharpFeature`)
3. Commit your Changes (`git commit -m 'Add something awesome'`)
4. Push to the Branch (`git push origin feature/MyCSharpFeature`)
5. Open a Pull Request

### Main Contributors
<a href="https://github.com/aqueleyan/c-tylog/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=aqueleyan/c-tylog" alt="contrib.rocks image" />
</a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## License

Distributed under the MIT License. See [`LICENSE`](https://github.com/aqueleyan/c-tylog/blob/master/LICENSE.txt) for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contact

[Twitter](https://twitter.com/aqueleNag) - fekieh35@gmail.com

Project Link: [https://github.com/aqueleyan/c-tylog](https://github.com/aqueleyan/c-tylog)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/aqueleyan/c-tylog.svg?style=for-the-badge
[contributors-url]: https://github.com/aqueleyan/c-tylog/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/aqueleyan/c-tylog.svg?style=for-the-badge
[forks-url]: https://github.com/aqueleyan/c-tylog/network/members
[stars-shield]: https://img.shields.io/github/stars/aqueleyan/c-tylog.svg?style=for-the-badge
[stars-url]: https://github.com/aqueleyan/c-tylog/stargazers
[issues-shield]: https://img.shields.io/github/issues/aqueleyan/c-tylog.svg?style=for-the-badge
[issues-url]: https://github.com/aqueleyan/c-tylog/issues
[license-shield]: https://img.shields.io/github/license/aqueleyan/c-tylog.svg?style=for-the-badge
[license-url]: https://github.com/aqueleyan/c-tylog/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/aqueleyan
