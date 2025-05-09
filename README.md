# Semantic Kernel Chat Application

A .NET 9 console application that implements a chat interface using Microsoft's Semantic Kernel and Azure OpenAI services.

## Features
- Console-based chat interface with Azure OpenAI
- Real-time response streaming
- Conversation history management
- Git integration for repository management and commit history
- Release notes generation and storage
- AI-powered release notes generation from Git commits

## Setup
1. Configure Azure OpenAI credentials in `appsettings.Development.json`:
`{ "ModelName": "your-model-name", "Endpoint": "your-azure-openai-endpoint", "ApiKey": "your-azure-openai-key" }`

## Git Integration
The application provides Git repository integration via the GitPlugin:
- View recent commits from a local repository
- Set and validate repository paths

## Release Management
The ReleaseStoragePlugin allows you to:
- Generate release notes from Git commit history using AI
- Save version information with release notes
- Retrieve the latest release information

## Usage
Run the application and use natural language to interact with Git repositories and manage releases. Example commands:
- "Show me the last 10 commits"
- "Generate release notes for version 1.2.0"
- "Save this as release 1.2.0"
