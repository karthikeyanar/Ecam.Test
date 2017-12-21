-- phpMyAdmin SQL Dump
-- version 4.0.4
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Dec 21, 2017 at 11:46 AM
-- Server version: 5.6.20
-- PHP Version: 5.4.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `ecamtest_backup`
--
CREATE DATABASE IF NOT EXISTS `ecamtest_backup` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `ecamtest_backup`;

-- --------------------------------------------------------

--
-- Table structure for table `tra_holding`
--

CREATE TABLE IF NOT EXISTS `tra_holding` (
  `holding_id` int(11) NOT NULL AUTO_INCREMENT,
  `symbol` varchar(50) NOT NULL,
  `trade_date` date NOT NULL,
  `quantity` int(11) NOT NULL,
  `avg_price` decimal(13,4) NOT NULL,
  PRIMARY KEY (`holding_id`) USING BTREE
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=11 ;

--
-- Dumping data for table `tra_holding`
--

INSERT INTO `tra_holding` (`holding_id`, `symbol`, `trade_date`, `quantity`, `avg_price`) VALUES
(1, 'PANAMAPET', '2017-12-18', 63, '234.4000'),
(2, 'FCONSUMER', '2017-12-18', 219, '67.8000'),
(3, 'HSCL', '2017-12-18', 95, '156.0000'),
(4, 'RADICO', '2017-12-18', 53, '279.2500'),
(5, 'BOMDYEING', '2017-12-18', 50, '294.9000'),
(6, 'TIRUMALCHM', '2017-12-18', 7, '2084.3000'),
(7, 'V2RETAIL', '2017-12-18', 29, '499.7000'),
(8, 'PROVOGE', '2017-12-18', 1631, '9.1500'),
(9, 'RAIN', '2017-12-18', 40, '367.0100'),
(10, 'APEX', '2017-12-18', 16, '899.5500');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
