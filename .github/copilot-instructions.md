# Enterprise Code Intelligence Platform (ECIP)

## Project Overview

Enterprise Code Intelligence Platform (ECIP) is an AI-powered code comprehension platform that helps software architects and developers understand large enterprise repositories.

The platform analyzes source code, builds semantic understanding, generates architecture views, detects business flows, assists onboarding, performs impact analysis, and generates code using AI.

This project is being developed as an Enterprise AI Architecture Capstone.

---

# Technology Stack

## Backend

- ASP.NET Core Web API (.NET 8 LTS)
- ASP.NET Core MVC (.NET 8 LTS)
- C#
- Dependency Injection
- Clean Architecture

## AI Layer

- FastAPI
- Python 3.10+
- LangChain (future)
- FAISS Vector Database
- OpenAI / Azure OpenAI
- Configurable LLM Provider

## Repository Intelligence

- Roslyn
- Tree-sitter
- LibGit2Sharp

## Storage

- SQLite (Development)
- SQL Server (Future)
- Redis (Future)

## UI

- Bootstrap 5
- Razor Views
- Responsive Enterprise Dashboard

---

# Architecture Principles

Always follow

- Clean Architecture
- SOLID Principles
- Repository Pattern
- Dependency Injection
- Single Responsibility Principle
- Separation of Concerns

Never introduce unnecessary complexity.

---

# Target Framework

Target Framework is

.NET 8 LTS

Always generate .NET 8 compatible code.

Never generate .NET 9 specific APIs.

Do NOT use

- AddOpenApi()
- MapOpenApi()
- MapStaticAssets()
- WithStaticAssets()

Use .NET 8 compatible middleware.

---

# Project Structure

ECIP.sln

src

Frontend

ECIP.Web

Backend

ECIP.API

Services

ECIP.RepositoryService

Libraries

ECIP.Core

ECIP.Infrastructure

ECIP.Shared

Python

ECIP.AI

configuration

docs

prompt-registry

repositories

workspace

cache

logs

scripts

tests

---

# ECIP Features

Implement the platform incrementally.

Modules

Dashboard

Repository Manager

Repository Scanner

Architecture Explorer

Flow Explorer

Knowledge Graph

Onboarding Assistant

Change Impact Analyzer

Code Generator

Prompt Registry

AI Gateway

Settings

---

# AI Architecture

The AI layer consists of

Planner Agent

Repository Intelligence Agent

Validation Agent

Future agents may be added.

Do not implement AI logic until requested.

---

# Coding Standards

Always

Generate production-quality code.

Use constructor dependency injection.

Keep methods small.

Use async/await where appropriate.

Prefer interfaces.

Use meaningful names.

Use XML documentation for public classes.

Avoid duplicate code.

---

# Logging

Use

Serilog

Console logging

Structured logging

---

# Error Handling

Use

Global exception middleware

Meaningful HTTP status codes

Centralized error responses

---

# Configuration

Read configuration from

appsettings.json

Do not hardcode

URLs

API Keys

Connection Strings

Model Names

Embedding Models

Prompt Templates

Everything must be configurable.

---

# Security

Never hardcode secrets.

Use configuration.

Validate inputs.

Escape user data.

Do not expose stack traces.

---

# Git Workflow

Current working branch

feature/sprint1-foundation

Do not modify Git configuration.

---

# Development Workflow

Before making changes

1. Analyze the existing solution.

2. Reuse existing architecture.

3. Build only the requested module.

4. Avoid unrelated refactoring.

After making changes

1. Ensure solution builds.

2. Fix compilation errors.

3. Summarize

Files Created

Files Modified

Build Status

Manual Steps

---

# UI Guidelines

Professional Enterprise Dashboard

Use Bootstrap 5

Corporate look and feel

Responsive

Simple

Modern

No unnecessary animations.

---

# Performance

Prefer efficient algorithms.

Avoid unnecessary allocations.

Lazy load where possible.

Use async I/O.

---

# Future Integrations

GitHub

Azure DevOps

Bitbucket

GitLab

Azure OpenAI

OpenAI

Local LLM

MCP Server

Prompt Registry

Knowledge Graph

These integrations should be loosely coupled.

---

# Important

Do not redesign the solution.

Do not rename projects.

Do not change project structure.

Do not upgrade packages.

Do not upgrade framework.

Keep compatibility with .NET 8.

Always preserve existing architecture.

The solution must compile successfully after every requested change.