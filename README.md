# Racing Coach

A personal project for analyzing and coaching racing/sim racing drivers through telemetry data.

## About

Racing Coach is a telemetry analysis platform designed to help racing drivers improve their performance. It captures real-time telemetry data from racing games, normalizes it into a common model, analyzes sessions, laps, and sectors, and provides actionable insights for improvement.

**This is a personal hobby project** - all contributions, suggestions, and improvements are welcome!

## Supported Games

- Gran Turismo 7 (PS5)
- F1 25 (planned)
- Other simulators (planned)

## Features

- Real-time telemetry capture via UDP
- Live dashboard with speed, RPM, gear, throttle, and brake
- Interactive telemetry chart
- Session persistence with SQLite
- Pause/resume functionality for data analysis

## Prerequisites

- .NET 10 SDK
- A supported racing game with telemetry enabled

## Running the Application

```bash
dotnet run --project src/RacingCoach.Api
```

The application will start and listen for UDP telemetry on port **33740**.

Open your browser and navigate to `http://localhost:5034/capture` to see the telemetry dashboard.

## Running Tests

```bash
dotnet test
```

## Contributing

This is a personal project, but **all contributions are welcome!** Bug fixes, new features, documentation improvements, code refactoring, test coverage, feature suggestions - everything helps.

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Make your changes and write tests
4. Ensure all tests pass: `dotnet test`
5. Commit using [Conventional Commits](https://www.conventionalcommits.org/)
6. Submit a Pull Request

All code, comments, and documentation must be in English.

## License

This is a personal project. Feel free to use the code for learning purposes or your own projects.
