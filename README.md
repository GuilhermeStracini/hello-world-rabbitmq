# Hello World RabbitMQ

📚 A repository to learn and explore the basics of **[RabbitMQ](https://www.rabbitmq.com/)** with C# .NET.

[![wakatime](https://wakatime.com/badge/github/GuilhermeStracini/hello-world-rabbitmq.svg)](https://wakatime.com/badge/github/GuilhermeStracini/hello-world-rabbitmq)
[![Maintainability](https://api.codeclimate.com/v1/badges/87fcff4ea25008d0c3f4/maintainability)](https://codeclimate.com/github/GuilhermeStracini/hello-world-rabbitmq/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/87fcff4ea25008d0c3f4/test_coverage)](https://codeclimate.com/github/GuilhermeStracini/hello-world-rabbitmq/test_coverage)
[![CodeFactor](https://www.codefactor.io/repository/github/GuilhermeStracini/hello-world-rabbitmq/badge)](https://www.codefactor.io/repository/github/GuilhermeStracini/hello-world-rabbitmq)
[![GitHub license](https://img.shields.io/github/license/GuilhermeStracini/hello-world-rabbitmq)](https://github.com/GuilhermeStracini/hello-world-rabbitmq)
[![GitHub last commit](https://img.shields.io/github/last-commit/GuilhermeStracini/hello-world-rabbitmq)](https://github.com/GuilhermeStracini/hello-world-rabbitmq)

---

## About

This repository demonstrates how to integrate **RabbitMQ**, a powerful message broker, with .NET applications. It provides simple examples of publishing and consuming messages to help you understand the core concepts of RabbitMQ.

---

## Features

- **Message Publishing**: Learn how to publish messages to RabbitMQ exchanges.
- **Message Consumption**: Understand how to create consumers for RabbitMQ queues.
- **Basic Concepts**: Explore exchanges, queues, bindings, and routing keys.
- **C# Integration**: Examples are implemented using **C#** with the **RabbitMQ.Client** library.

---

## Prerequisites

To follow along with this repository, you'll need:

1. **RabbitMQ Server**: Download and install RabbitMQ from [here](https://www.rabbitmq.com/download.html). Ensure the server is running locally or accessible remotely.
2. **.NET SDK**: Install the latest .NET SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/).
3. **Docker (Optional)**: If you prefer, you can run RabbitMQ via Docker:

   ```bash
   docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

---

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/GuilhermeStracini/hello-world-rabbitmq.git
   cd hello-world-rabbitmq
   ```

2. Build and run the solution:

   ```bash
   dotnet build
   dotnet run
   ```

3. Follow the console output to understand how messages are sent to and received from RabbitMQ.

---

## Resources

- [RabbitMQ Official Documentation](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
- [RabbitMQ .NET Client Library](https://github.com/rabbitmq/rabbitmq-dotnet-client)

---

## License

This repository is licensed under the [MIT License](LICENSE).

---

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests to improve this project.
