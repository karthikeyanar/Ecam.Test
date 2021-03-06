CREATE TABLE `tra_intra_day_profit_calc` (
  `profit_calc_id` INTEGER(11) NOT NULL AUTO_INCREMENT,
  `trade_date` DATETIME(6) NOT NULL,
  `profit` DECIMAL(13,4) DEFAULT NULL,
  `loss` DECIMAL(13,4) DEFAULT NULL,
  PRIMARY KEY (`profit_calc_id`)
) ENGINE=InnoDB
CHECKSUM=0
DELAY_KEY_WRITE=0
PACK_KEYS=0
AUTO_INCREMENT=0
AVG_ROW_LENGTH=0
MIN_ROWS=0
MAX_ROWS=0
ROW_FORMAT=DEFAULT
KEY_BLOCK_SIZE=0;