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
