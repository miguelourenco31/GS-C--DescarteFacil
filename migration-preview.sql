CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Ecopontos` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nome` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `Endereco` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Bairro` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
    `Latitude` double NOT NULL,
    `Longitude` double NOT NULL,
    `OcupacaoPercentual` int NOT NULL,
    `UltimaLeituraEm` datetime(6) NULL,
    CONSTRAINT `PK_Ecopontos` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Doacoes` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `TipoItem` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `Descricao` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
    `NomeDoador` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Retirada` tinyint(1) NOT NULL,
    `CriadaEm` datetime(6) NOT NULL,
    `EcopontoId` int NULL,
    CONSTRAINT `PK_Doacoes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Doacoes_Ecopontos_EcopontoId` FOREIGN KEY (`EcopontoId`) REFERENCES `Ecopontos` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `Solicitacoes` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `TipoItem` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `Descricao` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
    `Bairro` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
    `NomeSolicitante` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `CriadaEm` datetime(6) NOT NULL,
    `EcopontoId` int NULL,
    CONSTRAINT `PK_Solicitacoes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Solicitacoes_Ecopontos_EcopontoId` FOREIGN KEY (`EcopontoId`) REFERENCES `Ecopontos` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

INSERT INTO `Ecopontos` (`Id`, `Bairro`, `Endereco`, `Latitude`, `Longitude`, `Nome`, `OcupacaoPercentual`, `UltimaLeituraEm`)
VALUES (1, 'Sé', 'Rua Exemplo, 100', -23.5505, -46.633299999999998, 'Ecoponto Exemplo Sé', 20, NULL),
(2, 'Pinheiros', 'Av. Exemplo, 200', -23.561399999999999, -46.691800000000001, 'Ecoponto Exemplo Pinheiros', 95, NULL),
(3, 'Mooca', 'Rua Exemplo, 300', -23.558, -46.598999999999997, 'Ecoponto Exemplo Mooca', 55, NULL);

CREATE INDEX `IX_Doacoes_EcopontoId` ON `Doacoes` (`EcopontoId`);

CREATE INDEX `IX_Ecopontos_Bairro` ON `Ecopontos` (`Bairro`);

CREATE INDEX `IX_Solicitacoes_EcopontoId` ON `Solicitacoes` (`EcopontoId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261118193642_InitialCreate', '10.0.15');

COMMIT;

