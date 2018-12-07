# Inspiral, Coalescence, Ringdown

This is a rather grandiosely named hobby project to implement a MUD from scratch in C# with minimal dependencies or reliance on existing libraries. Several have been used because it doesn't seem terribly reasonable to also roll a from-scratch database, encryption or serialization suite.

## Getting Started

### Prerequisites

[Microsoft.NET Framework](https://www.microsoft.com/en-us/download/details.aspx?id=55168)

### Installing

* Clone the repository to the desired location.
* `cd InspiralC`
* `dotnet run inspiral.csproj`
* Connect to `127.0.0.1` on port `9090` or `2323` using Telnet or your preferred MUD client (like [Mudlet](https://mudlet.org/download/)).

## Built With

* [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
* [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
* [System.Data.SQLite](https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on the repository code of conduct, guides to common tasks, and the process for submitting pull requests to the project.

## Authors

See the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under zlib/libpng - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

* Thanks to /r/MUD and Fish Guts for feedback, discussion and consistent, near white-noise shitposting.
* Thanks to [Aetolia: The Midnight Age](http://aetolia.com) for eating a solid third of my life.
* Thank you to [PurpleBooth](https://gist.github.com/PurpleBooth/109311bb0361f32d87a2) for the `README.md` template.
