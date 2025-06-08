USE master;
GO
CREATE DATABASE OrdenCompraDB;
GO
USE OrdenCompraDB;
GO

-- Tabla: Usuarios (para autenticación)
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Rol NVARCHAR(20) NOT NULL DEFAULT 'Usuario',
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Tabla: Ordenes (Cabecera)
CREATE TABLE Ordenes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    Cliente NVARCHAR(100) NOT NULL,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    UsuarioId INT NOT NULL,
    CONSTRAINT FK_Ordenes_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT CK_Ordenes_Total_Positivo CHECK (Total >= 0)
);

-- Tabla: OrdenDetalles (Detalle)
CREATE TABLE OrdenDetalles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdenId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrdenDetalles_Ordenes FOREIGN KEY (OrdenId) REFERENCES Ordenes(Id) ON DELETE CASCADE,
    CONSTRAINT CK_OrdenDetalles_Cantidad_Positiva CHECK (Cantidad > 0),
    CONSTRAINT CK_OrdenDetalles_PrecioUnitario_Positivo CHECK (PrecioUnitario >= 0),
    CONSTRAINT CK_OrdenDetalles_Subtotal_Positivo CHECK (Subtotal >= 0)
);
GO

-- Índice para relación orden-detalle
CREATE INDEX IX_OrdenDetalles_OrdenId ON OrdenDetalles (OrdenId);
GO

-- Insertar usuarios de prueba
INSERT INTO Usuarios (NombreUsuario, Email, PasswordHash, Rol) VALUES
('admin', 'admin@dominio.com', '$2a$11$/yVbCk1LR89fUO1Ox/LDpuQq8WZOuCxjC956LrNp3kyPVgaNxaStu', 'Administrador'),
('usuario1', 'usuario1@dominio.com', '$2a$11$6FyRTrmvW.FfJljEHQ.6MOnYG4LNoEAD0Pbyf2K3csgnic8oOEUC6', 'Usuario')
GO

-- Insertar órdenes de ejemplo
DECLARE @Usuario1Id INT = (SELECT Id FROM Usuarios WHERE NombreUsuario = 'usuario1');

INSERT INTO Ordenes (FechaCreacion, Cliente, Total, UsuarioId) VALUES
('2025-06-04 10:30:00', 'Empresa ABC S.A.', 1250.00, @Usuario1Id),
('2025-06-05 14:45:00', 'Comercial XYZ Ltda.', 875.50, @Usuario1Id),
('2025-06-06 09:15:00', 'Distribuidora 123', 2100.75, @Usuario1Id);

-- Insertar detalles de ejemplo
DECLARE @Orden1Id INT = (SELECT Id FROM Ordenes WHERE Cliente = 'Empresa ABC S.A.');
DECLARE @Orden2Id INT = (SELECT Id FROM Ordenes WHERE Cliente = 'Comercial XYZ Ltda.');
DECLARE @Orden3Id INT = (SELECT Id FROM Ordenes WHERE Cliente = 'Distribuidora 123');

INSERT INTO OrdenDetalles (OrdenId, Producto, Cantidad, PrecioUnitario, Subtotal) VALUES
-- Detalles Orden 1
(@Orden1Id, 'Laptop Dell Inspiron 15', 2, 500.00, 1000.00),
(@Orden1Id, 'Mouse Inalámbrico', 5, 25.00, 125.00),
(@Orden1Id, 'Teclado Mecánico', 1, 125.00, 125.00),

-- Detalles Orden 2
(@Orden2Id, 'Monitor LED 24"', 3, 225.00, 675.00),
(@Orden2Id, 'Cable HDMI', 10, 15.00, 150.00),
(@Orden2Id, 'Adaptador USB-C', 2, 25.25, 50.50),

-- Detalles Orden 3
(@Orden3Id, 'Impresora Multifuncional', 1, 350.00, 350.00),
(@Orden3Id, 'Papel Bond A4', 20, 12.50, 250.00),
(@Orden3Id, 'Cartuchos de Tinta', 8, 187.59, 1500.72),
(@Orden3Id, 'Grapadora Industrial', 1, 0.03, 0.03);

GO
