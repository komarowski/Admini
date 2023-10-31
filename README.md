# Admini

**Admini** is a web service for posting custom notes with geolocation. The service can be used for different purposes, for example, for running a travel blog or posting real estate advertisements.

## Demo

![](https://github.com/komarowski/Admini/blob/main/images/demo.gif)

## Description

A lightweight application with a backend on ASP.NET Core and a frontend on React.

### Features

 - The Markdown markup language is used for writing notes.
 - Displaying notes with geolocation on an interactive map.
 - Search by note titles and tags.
 - Simple admin panel.
 - Responsive design with custom css.

### Structure

 - adminirontend - React with [Pigeon Maps](https://pigeon-maps.js.org/) (Application layer)
 - AdminiBackend - ASP.NET Core Razor Pages with minimal API endpoints (Application layer)
 - AdminiDomain - Business logic (Domain model layer)
 - AdminiInfrastructure - Context for MS SQL Server (Infrastructure layer)
 - AdminiTests - Unit and integration testing
