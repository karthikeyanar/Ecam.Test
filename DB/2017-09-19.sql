CREATE TABLE `tra_holding` (
  `holding_id` INTEGER(11) NOT NULL AUTO_INCREMENT,
  `symbol` VARCHAR(50) NOT NULL,
  `trade_date` DATE NOT NULL,
  `quantity` INTEGER(11) NOT NULL,
  `avg_price` DECIMAL(13,4) NOT NULL,
  PRIMARY KEY (`holding_id`)
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