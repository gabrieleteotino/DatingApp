CREATE TABLE `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` longtext NULL,
    `PasswordSalt` longblob NULL,
    `PasswordHash` longblob NULL,
    `Gender` longtext NULL,
    `DateOfBirth` datetime(6) NOT NULL,
    `KnownAs` longtext NULL,
    `Created` datetime(6) NOT NULL,
    `LastActive` datetime(6) NOT NULL,
    `Introduction` longtext NULL,
    `LookingFor` longtext NULL,
    `Interests` longtext NULL,
    `City` longtext NULL,
    `Country` longtext NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
);

CREATE TABLE `Values` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext NULL,
    CONSTRAINT `PK_Values` PRIMARY KEY (`Id`)
);

CREATE TABLE `Likes` (
    `FromId` int NOT NULL,
    `ToId` int NOT NULL,
    CONSTRAINT `PK_Likes` PRIMARY KEY (`FromId`, `ToId`),
    CONSTRAINT `FK_Likes_Users_FromId` FOREIGN KEY (`FromId`) REFERENCES `Users` (`Id`) ON DELETE NO ACTION,
    CONSTRAINT `FK_Likes_Users_ToId` FOREIGN KEY (`ToId`) REFERENCES `Users` (`Id`) ON DELETE NO ACTION
);

CREATE TABLE `Messages` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SenderId` int NOT NULL,
    `RecipientId` int NOT NULL,
    `Content` longtext NULL,
    `SentDate` datetime(6) NOT NULL,
    `IsRead` bit NOT NULL,
    `ReadDate` datetime(6) NULL,
    `SenderDeleted` bit NOT NULL,
    `RecipientDeleted` bit NOT NULL,
    CONSTRAINT `PK_Messages` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Messages_Users_RecipientId` FOREIGN KEY (`RecipientId`) REFERENCES `Users` (`Id`) ON DELETE NO ACTION,
    CONSTRAINT `FK_Messages_Users_SenderId` FOREIGN KEY (`SenderId`) REFERENCES `Users` (`Id`) ON DELETE NO ACTION
);

CREATE TABLE `Photos` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Url` longtext NULL,
    `Description` longtext NULL,
    `DateAdded` datetime(6) NOT NULL,
    `IsMain` bit NOT NULL,
    `PublicId` longtext NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_Photos` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Photos_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_Likes_ToId` ON `Likes` (`ToId`);

CREATE INDEX `IX_Messages_RecipientId` ON `Messages` (`RecipientId`);

CREATE INDEX `IX_Messages_SenderId` ON `Messages` (`SenderId`);

CREATE INDEX `IX_Photos_UserId` ON `Photos` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20180807144520_InitialMySql', '2.1.1-rtm-30846');

