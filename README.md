# DatingApp
DatingApp is a SPA dating application written in Angular 5 with a webapi backend made in C# using .net core 2.1

The application is my opinionated version of this [Udemy course](https://www.udemy.com/build-an-app-with-aspnet-core-and-angular-from-scratch/)

The course used a previous version of .net core I fixed everything to use 2.1.0-rc1-final.

# Configuration

## JWT TokenSecret

Create a key for **TokenSecret**.

dotnet user-secrets set "AppSettings:TokenSecret" "super duper secret key"