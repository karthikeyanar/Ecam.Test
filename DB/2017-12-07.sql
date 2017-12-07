CREATE TABLE `tra_category_profit` (
  `category_name` VARCHAR(50) NOT NULL,
  `profit_type` VARCHAR(1) NOT NULL,
  `profit_date` DATE NOT NULL,
  `profit` DECIMAL(13,4) NOT NULL,
  PRIMARY KEY (`category_name`, `profit_type`, `profit_date`)
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