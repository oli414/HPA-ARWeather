-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server versie:                10.1.28-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win32
-- HeidiSQL Versie:              9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Structuur van  tabel arweather.weather_cache wordt geschreven
CREATE TABLE IF NOT EXISTS `weather_cache` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `latitude` int(11) NOT NULL DEFAULT '0',
  `longitude` int(11) NOT NULL DEFAULT '0',
  `epoch` int(11) NOT NULL DEFAULT '0',
  `temperature` int(11) NOT NULL DEFAULT '0',
  `rain` int(11) NOT NULL DEFAULT '0',
  `precipitation` int(11) NOT NULL DEFAULT '0',
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1780 DEFAULT CHARSET=latin1;

-- Dumpen data van tabel arweather.weather_cache: ~262 rows (ongeveer)
/*!40000 ALTER TABLE `weather_cache` DISABLE KEYS */;
/*!40000 ALTER TABLE `weather_cache` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
