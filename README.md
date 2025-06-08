# Proyecto: Orden de Compra

Este es un proyecto de práctica que consiste en desarrollar una solución funcionales de Gestión de Órdenes de Compra desarrollado con .NET 8.0 con arquitectura Clean Architecture.
La solución está estructurada siguiendo una arquitectura por capas, lo que permite una mayor separación de responsabilidades, escalabilidad y mantenibilidad.

## Estructura del Proyecto
OrdenCompra/
 - OrdenCompra.API/ # Proyecto de API REST para exposición de servicios
 - OrdenCompra.Core/ # Entidades del dominio y contratos (interfaces)
 - OrdenCompra.Infraestructura/ # Implementación de la lógica de acceso a datos y servicios externos
 - OrdenCompra.Web/ # Interfaz Web MVC responsivo con Bootstrap 5
 - OrdenCompra.Tests/ # Proyecto de pruebas unitarias

## Tecnologías
.NET 8.0, ASP.NET Core API/MVC
Entity Framework Core, SQL Server
JWT Bearer Authentication
FluentValidation, AutoMapper
Bootstrap 5, Swagger/OpenAPI

## Base de Datos
Ejecutar el archivo ScriptsBD.sql que se encuentra en la raíz.
