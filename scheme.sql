-- --------------------------------------------------------
-- Хост:                         127.0.0.1
-- Версия сервера:               10.4.32-MariaDB - mariadb.org binary distribution
-- Операционная система:         Win64
-- HeidiSQL Версия:              12.6.0.6765
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Дамп структуры базы данных tolyabase
CREATE DATABASE IF NOT EXISTS `tolyabase` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci */;
USE `tolyabase`;

-- Дамп структуры для таблица tolyabase.catalog
CREATE TABLE IF NOT EXISTS `catalog` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Data_priema` datetime DEFAULT NULL,
  `Data_vidachi` datetime DEFAULT NULL,
  `Data_predoplaty` datetime DEFAULT NULL,
  `surname` varchar(255) DEFAULT NULL,
  `phone` varchar(50) DEFAULT NULL,
  `AboutUs` text DEFAULT NULL,
  `WhatRemont` varchar(255) DEFAULT NULL,
  `brand` varchar(100) DEFAULT NULL,
  `model` varchar(100) DEFAULT NULL,
  `SerialNumber` varchar(100) DEFAULT NULL,
  `sostoyanie` text DEFAULT NULL,
  `komplektonst` text DEFAULT NULL,
  `polomka` text DEFAULT NULL,
  `kommentarij` text DEFAULT NULL,
  `predvaritelnaya_stoimost` double DEFAULT NULL,
  `Predoplata` double DEFAULT NULL,
  `Zatrati` double DEFAULT NULL,
  `okonchatelnaya_stoimost_remonta` double DEFAULT NULL,
  `Skidka` double DEFAULT NULL,
  `Status_remonta` varchar(100) DEFAULT NULL,
  `master` varchar(100) DEFAULT NULL,
  `vipolnenie_raboti` text DEFAULT NULL,
  `Garanty` varchar(100) DEFAULT NULL,
  `wait_zakaz` varchar(50) DEFAULT NULL,
  `Adress` varchar(255) DEFAULT NULL,
  `Image_key` varchar(255) DEFAULT NULL,
  `AdressSC` varchar(255) DEFAULT NULL,
  `DeviceColour` varchar(50) DEFAULT NULL,
  `ClientId` int(11) DEFAULT NULL,
  `Barcode` varchar(20) DEFAULT NULL,
  `Deleted` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.catalog: ~0 rows (приблизительно)

-- Дамп структуры для таблица tolyabase.clientsmap
CREATE TABLE IF NOT EXISTS `clientsmap` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `FIO` varchar(255) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Adress` text DEFAULT NULL,
  `Primechanie` text DEFAULT NULL,
  `Blist` varchar(50) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  `aboutUs` text DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.clientsmap: ~0 rows (приблизительно)

-- Дамп структуры для таблица tolyabase.groupdostup
CREATE TABLE IF NOT EXISTS `groupdostup` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `grName` varchar(255) DEFAULT NULL,
  `delZapis` varchar(10) DEFAULT NULL,
  `addZapis` varchar(10) DEFAULT NULL,
  `saveZapis` varchar(10) DEFAULT NULL,
  `graf` varchar(10) DEFAULT NULL,
  `sms` varchar(10) DEFAULT NULL,
  `stock` varchar(10) DEFAULT NULL,
  `clients` varchar(10) DEFAULT NULL,
  `stockAdd` varchar(10) DEFAULT NULL,
  `stockDel` varchar(10) DEFAULT NULL,
  `stockEdit` varchar(10) DEFAULT NULL,
  `clientAdd` varchar(10) DEFAULT NULL,
  `clientDel` varchar(10) DEFAULT NULL,
  `clientConcat` varchar(10) DEFAULT NULL,
  `settings` varchar(10) DEFAULT NULL,
  `dates` varchar(10) DEFAULT NULL,
  `editDates` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.groupdostup: ~0 rows (приблизительно)

-- Дамп структуры для таблица tolyabase.historybd
CREATE TABLE IF NOT EXISTS `historybd` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `WHO` varchar(255) DEFAULT NULL,
  `WHAT` varchar(255) DEFAULT NULL,
  `FULLWHAT` text DEFAULT NULL,
  `data` datetime DEFAULT NULL,
  `IDINCATALOG` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.historybd: ~1 rows (приблизительно)
INSERT INTO `historybd` (`id`, `WHO`, `WHAT`, `FULLWHAT`, `data`, `IDINCATALOG`) VALUES
	(1, 'ADMIN', 'ДОБАВЛЕНИЕ НОВОЙ ЗАПИСИ', '', '0000-00-00 00:00:00', 1),
	(2, 'ADMIN', 'ДОБАВЛЕНИЕ НОВОЙ ЗАПИСИ', '', '0000-00-00 00:00:00', 2);

-- Дамп структуры для таблица tolyabase.statesmap
CREATE TABLE IF NOT EXISTS `statesmap` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `clientId` int(11) DEFAULT NULL,
  `State` varchar(255) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.statesmap: ~1 rows (приблизительно)
INSERT INTO `statesmap` (`id`, `clientId`, `State`, `date`) VALUES
	(1, 1, '21-05-2025 13-08', NULL),
	(2, 2, '21-05-2025 13-19', NULL);

-- Дамп структуры для таблица tolyabase.stock
CREATE TABLE IF NOT EXISTS `stock` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Naimenovanie` varchar(255) DEFAULT NULL,
  `Kategoriya` varchar(255) DEFAULT NULL,
  `Podkategoriya` varchar(255) DEFAULT NULL,
  `Colour` varchar(50) DEFAULT NULL,
  `Brand` varchar(100) DEFAULT NULL,
  `Model` varchar(100) DEFAULT NULL,
  `CountOf` int(11) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `Napominanie` text DEFAULT NULL,
  `Photo` text DEFAULT NULL,
  `Primechanie` text DEFAULT NULL,
  `Photo2` text DEFAULT NULL,
  `Photo3` text DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.stock: ~0 rows (приблизительно)

-- Дамп структуры для таблица tolyabase.stockmap
CREATE TABLE IF NOT EXISTS `stockmap` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `clientId` int(11) DEFAULT NULL,
  `ZIPId` int(11) DEFAULT NULL,
  `countOfZIP` int(11) DEFAULT NULL,
  `priceOfZIP` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.stockmap: ~0 rows (приблизительно)

-- Дамп структуры для таблица tolyabase.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(50) NOT NULL,
  `name` varchar(100) NOT NULL,
  `id_gruppi_dostupa` int(11) NOT NULL,
  `user_pwd` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Дамп данных таблицы tolyabase.users: ~0 rows (приблизительно)

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
